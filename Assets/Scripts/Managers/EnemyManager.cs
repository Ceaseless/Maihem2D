using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maihem.Actors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maihem.Managers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float minimalTurnTime = 0.25f;
        [Min(1)]
        [SerializeField] private int cullHorizontalDistance;
        [SerializeField] private int spawnRate;
        [SerializeField] private bool periodicSpawn;
        [SerializeField] private PickupManager pickupManager;

        public Action AllEnemiesPerformedTurn;
        
        private List<Enemy> _activeEnemies;
        private List<Enemy> _deadEnemies;

        private int _spawnTimer;
        private int _enemiesTakingTurn;
        private bool _dispatchingEnemies;

       

        public void Initialize()
        {
            _activeEnemies = new List<Enemy>();
            _deadEnemies = new List<Enemy>();
            GameManager.Instance.PlayerInput.ToggleEnemyMarkersAction += ToggleEnemyMarkers;
        }

        private void OnDestroy()
        {
            GameManager.Instance.PlayerInput.ToggleEnemyMarkersAction -= ToggleEnemyMarkers;
        }

        private void RegisterEnemy(Enemy newEnemy)
        {
            newEnemy.Died += EnemyDied;
            newEnemy.TurnStarted += EnemyStartedTurn;
            newEnemy.TurnCompleted += EnemyCompletedTurn;
            newEnemy.Initialize();
            _activeEnemies.Add(newEnemy);
        }

        public void RegisterEnemies(IEnumerable<Enemy> enemies)
        {
            foreach (var newEnemy in enemies)
            {
                RegisterEnemy(newEnemy);
            }
        }

        public void Reset()
        {
            _deadEnemies?.Clear();
            
            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _activeEnemies[i];
                _activeEnemies.RemoveAt(i);
                Destroy(enemy.gameObject);
            }
            _activeEnemies.Clear();
            _spawnTimer = 0;
            _enemiesTakingTurn = 0;
            _dispatchingEnemies = false;
        }

        private void SpawnEnemy()
        {
            var randomCell = MapManager.Instance.GetFreeCell();
            var randomPosition = MapManager.Instance.CellToWorld(randomCell);
            var newEnemy = Instantiate(enemyPrefabs[Random.Range(0,enemyPrefabs.Length)], randomPosition, Quaternion.identity, transform).GetComponent<Enemy>();
            
            RegisterEnemy(newEnemy);
        }

        public bool AreAllActionsPerformed() => !_dispatchingEnemies && _enemiesTakingTurn == 0;
        
        private void CullDeadEnemies()
        {
            foreach (var deadEnemy in _deadEnemies)
            { 
                deadEnemy.Died -= EnemyDied;
                deadEnemy.TurnStarted -= EnemyStartedTurn;
                deadEnemy.TurnCompleted -= EnemyCompletedTurn;
                _activeEnemies.Remove(deadEnemy);
                Destroy(deadEnemy.gameObject);
            }
            
            _deadEnemies.Clear();
        }

        private void KillLeftBehindEnemies()
        {
            var playerPosition = GameManager.Instance.Player.GridPosition;
            foreach (var enemy in _activeEnemies)
            {
                var distance = playerPosition.x - enemy.GridPosition.x; // > 0 => Player is on the right
                if (distance > cullHorizontalDistance)
                {
                    _deadEnemies.Add(enemy);
                }
            }
        }

        private float _turnStartTime;
        public void Tick()
        {
            _turnStartTime = Time.time;
            _spawnTimer++;
            if (periodicSpawn && _spawnTimer >= spawnRate)
            {
                SpawnEnemy();
                _spawnTimer = 0;
            }

            KillLeftBehindEnemies();
            CullDeadEnemies();
            _enemiesTakingTurn = 0;
            if (_activeEnemies.Count > 0)
                StartCoroutine(AmortizedEnemyTurn());
            else
                TryFinishEnemyTurn();
        }


        private void TryFinishEnemyTurn()
        {
            var now = Time.time;
            var elapsedTime = now - _turnStartTime;
            if(elapsedTime > minimalTurnTime) {
                AllEnemiesPerformedTurn();
            }
            else
            {
                StartCoroutine(WaitTillMinimalTurnTime(minimalTurnTime - elapsedTime));
            }
        }

        private IEnumerator WaitTillMinimalTurnTime(float timeDiff)
        {
            yield return new WaitForSeconds(timeDiff);
            AllEnemiesPerformedTurn();
        }
        
        private IEnumerator AmortizedEnemyTurn()
        {
            _dispatchingEnemies = true;
            foreach (var enemy in _activeEnemies)
            {
                enemy?.TakeTurn();
                yield return null;
            }
            _dispatchingEnemies = false;
            if (AreAllActionsPerformed())
            {
                TryFinishEnemyTurn();
            }
            
        }

        private void EnemyStartedTurn(object sender, EventArgs args)
        {
            _enemiesTakingTurn++;
        }
        
        private void EnemyCompletedTurn(object sender, EventArgs args)
        {
            _enemiesTakingTurn--;
           
            if (!_dispatchingEnemies && _enemiesTakingTurn == 0)
            {
                TryFinishEnemyTurn();
            }
        }
        
        private void EnemyDied(object sender, DeathEventArgs eventArgs )
        {
            var enemy = eventArgs.DeadGameObject.GetComponent<Enemy>();
            _deadEnemies.Add(enemy);
            pickupManager.TrySpawnPickup(eventArgs.DeadGameObject.transform.position, enemy.loot);
        }
        
        public bool CellContainsEnemy(Vector2Int gridPosition)
        {
            return _activeEnemies.Any(enemy => enemy.GridPosition == gridPosition);
        } 
        
        public bool TryGetEnemyOnCell(Vector2Int gridPosition, out Enemy enemy)
        {
            foreach (var e in _activeEnemies)
            {
                if (e.GridPosition != gridPosition) continue;
                enemy = e;
                return true;
            }
            enemy = null;
            return false;
        }

        public IList<Enemy> GetEnemiesInProximity(Vector2Int origin, int range)
        {
            return _activeEnemies.Where(enemy => Vector2Int.Distance(origin,enemy.GridPosition) <= range).ToList();
        }

        private void HideEnemyMarkers()
        {
            foreach (var e in _activeEnemies)
            {
                e.ShowAttackMarkers(false);
            }
        }
        
        private void ToggleEnemyMarkers(object sender, ToggleEventArgs args)
        {
            if (args.ToggleValue)
            {
                var enemies = GameManager.Instance.GetEnemiesInProximity(GameManager.Instance.Player.GridPosition, 10);
                foreach (var enemy in enemies)
                {
                    enemy.ShowAttackMarkers(true);
                }
            }
            else
            {
                HideEnemyMarkers();
            }
        }
    }
}
