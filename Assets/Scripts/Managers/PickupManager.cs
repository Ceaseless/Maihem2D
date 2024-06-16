using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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

        public void CullUsedPickups()
        {
            foreach (var pickup in _activePickups.ToList())
            {
                if (pickup.Used)
                {
                    _activePickups.Remove(pickup);
                    Destroy(pickup.gameObject);
                }
                
            }
        }

        public void SpawnPickup(Vector3 position)
        {

            var pickup = pickupPrefabs[0];

            if (_activePickups.Any(activePickup => activePickup.transform.position == position))
            {
                return;
            }
            
            var random = new Random();
            if (!(pickup.GetComponent<Pickup>().spawnChance >= random.Next(1, 100))) return;
            var newPickup = Instantiate(pickup, position, Quaternion.identity, transform).GetComponent<Pickup>();
            _activePickups.Add(newPickup);

        }
    }
}