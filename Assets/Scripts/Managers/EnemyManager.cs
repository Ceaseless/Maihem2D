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
        [SerializeField] private ObjectBoundSettings boundSettings;
        [SerializeField] private int spawnRate;
        [SerializeField] private bool periodicSpawn;
        [SerializeField] private PickupManager pickupManager;

        public Action AllEnemiesPerformedTurn;
        
        private List<Enemy> _aliveEnemies;
        private List<Enemy> _deadEnemies;

        private int _spawnTimer;
        private int _enemiesTakingTurn;
        private bool _dispatchingEnemies;
        private Transform _cameraTransform;

       

        public void Initialize()
        {
            _aliveEnemies = new List<Enemy>();
            _deadEnemies = new List<Enemy>();
            GameManager.Instance.PlayerInput.ToggleEnemyMarkersAction += ToggleEnemyMarkers;
            _cameraTransform = Camera.main?.transform;
        }

        private void OnDestroy()
        {
            GameManager.Instance.PlayerInput.ToggleEnemyMarkersAction -= ToggleEnemyMarkers;
        }

        private void RegisterEnemy(Enemy newEnemy, bool active = true)
        {
            newEnemy.Died += EnemyDied;
            newEnemy.TurnStarted += EnemyStartedTurn;
            newEnemy.TurnCompleted += EnemyCompletedTurn;
            newEnemy.Initialize();
            
            newEnemy.gameObject.SetActive(active);
            _aliveEnemies.Add(newEnemy);
        }

        public void RegisterEnemies(IEnumerable<Enemy> enemies)
        {
            foreach (var newEnemy in enemies)
            {
                RegisterEnemy(newEnemy, false);
            }
        }

        public void Reset()
        {
            _deadEnemies?.Clear();
            
            for (var i = _aliveEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _aliveEnemies[i];
                _aliveEnemies.RemoveAt(i);
                Destroy(enemy.gameObject);
            }
            _aliveEnemies.Clear();
            _spawnTimer = 0;
            _enemiesTakingTurn = 0;
            _dispatchingEnemies = false;
        }

        public void UpdateEnemiesActiveState()
        {
            var cameraPosition = _cameraTransform.position;
            foreach (var enemy in _aliveEnemies)
            {
                if(enemy.gameObject.activeInHierarchy) continue;
                if(!boundSettings.IsInActivateDistance(cameraPosition, enemy.transform.position)) continue;
                enemy.gameObject.SetActive(true);
            }
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
                _aliveEnemies.Remove(deadEnemy);
                Destroy(deadEnemy.gameObject);
            }
            
            _deadEnemies.Clear();
        }

        private void KillLeftBehindEnemies()
        {
            var playerPosition = GameManager.Instance.Player.transform.position;
            foreach (var enemy in _aliveEnemies)
            {
                if (boundSettings.IsOutsideOfCullDistance(playerPosition, enemy.transform.position))
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
            if (_aliveEnemies.Count > 0)
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
            var checkPosition = _cameraTransform.position;
            _dispatchingEnemies = true;
            foreach (var enemy in _aliveEnemies)
            {
                if (!enemy.gameObject.activeInHierarchy && boundSettings.IsInActivateDistance(checkPosition, enemy.transform.position))
                {
                    enemy.gameObject.SetActive(true);
                }
                if(!boundSettings.IsInUpdateDistance(checkPosition, enemy.transform.position)) continue;
                
                if(enemy.gameObject.activeInHierarchy)
                    enemy.TakeTurn();
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
            return _aliveEnemies.Any(enemy => enemy.GridPosition == gridPosition);
        } 
        
        public bool TryGetEnemyOnCell(Vector2Int gridPosition, out Enemy enemy)
        {
            foreach (var e in _aliveEnemies)
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
            return _aliveEnemies.Where(enemy => Vector2Int.Distance(origin,enemy.GridPosition) <= range).ToList();
        }

        private void HideEnemyMarkers()
        {
            foreach (var e in _aliveEnemies)
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
