using System;
using System.Collections.Generic;
using System.Linq;
using Maihem.Actors;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maihem.Managers
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private int spawnRate;
        
        private List<Enemy> _activeEnemies;

        private int _spawnTimer;
        private int _enemiesTakingTurn;

        private void Start()
        {
            _activeEnemies = new List<Enemy>();
        }

        public void Reset()
        {
            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _activeEnemies[i];
                _activeEnemies.RemoveAt(i);
                Destroy(enemy.gameObject);
            }
            _activeEnemies.Clear();
            _spawnTimer = 0;
            _enemiesTakingTurn = 0;
        }

        private void SpawnEnemy()
        {
            var randomCell = MapManager.Instance.GetFreeCell();
            var randomPosition = MapManager.Instance.CellToWorld(randomCell);
            var newEnemy = Instantiate(enemyPrefabs[Random.Range(0,enemyPrefabs.Length)], randomPosition, Quaternion.identity, transform).GetComponent<Enemy>();
            
            newEnemy.Died += EnemyDied;
            newEnemy.TurnStarted += EnemyStartedTurn;
            newEnemy.TurnCompleted += EnemyCompletedTurn;
            _activeEnemies.Add(newEnemy);
        }

        public bool AreAllActionsPerformed() => _enemiesTakingTurn == 0;
        
        private void CullDeadEnemies()
        {
            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _activeEnemies[i];
                if (enemy.CurrentHealth > 0) continue;
                _activeEnemies.RemoveAt(i);
                Destroy(enemy.gameObject);
            }
           
        }

        public void Tick()
        {
            _spawnTimer++;
            if (_spawnTimer >= spawnRate)
            {
                SpawnEnemy();
                _spawnTimer = 0;
            }
            CullDeadEnemies();
            _enemiesTakingTurn = 0;
            foreach (var enemy in _activeEnemies)
            {
                enemy?.TakeTurn();
            }
        }

        private void EnemyStartedTurn(object sender, EventArgs args)
        {
            _enemiesTakingTurn++;
        }
        
        private void EnemyCompletedTurn(object sender, EventArgs args)
        {
            _enemiesTakingTurn--;
        }
        
        private void EnemyDied(object sender, DeathEventArgs eventArgs )
        {
            //?
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
    }
}
