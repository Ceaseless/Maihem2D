using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maihem.Managers
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] pickupPrefabs;
        [SerializeField] private int spawnRate;
        private List<Pickup> _activePickups;
        private int _spawnTimer;

        private void Start()
        {
            _activePickups = new List<Pickup>();
        }
        
        public void Reset()
        {
            for (var i = _activePickups.Count - 1; i >= 0; i--)
            {
                var enemy = _activePickups[i];
                _activePickups.RemoveAt(i);
                Destroy(enemy.gameObject);
            }
            _activePickups.Clear();
        }
        public void PlayerOnPickup(Vector2Int gridPosition)
        {
            foreach (var pickup in _activePickups.ToList().Where(pickup => pickup.GridPosition == gridPosition))
            {
                _activePickups.Remove(pickup);
                pickup.PickUp();
                Destroy(pickup.gameObject);
            }
        }

        public void SpawnPickup()
        {
            var randomCell = MapManager.Instance.GetFreeCell();
            var randomPosition = MapManager.Instance.CellToWorld(randomCell);
            
            var pickup = Instantiate(pickupPrefabs[0], randomPosition, Quaternion.identity,transform).GetComponent<Pickup>();
            pickup.GridPosition = randomCell;
            
            _activePickups.Add(pickup);
        }
        
        
    }
}