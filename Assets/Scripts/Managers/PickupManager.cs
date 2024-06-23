using System.Collections.Generic;
using System.Linq;
using Maihem.Pickups;
using UnityEngine;

namespace Maihem.Managers
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] pickupPrefabs;
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

        private void RegisterPickup(Pickup newPickup)
        {
            _activePickups.Add(newPickup);
        }
        
        public void RegisterPickups(IEnumerable<Pickup> pickups)
        {
            foreach (var newPickup in pickups)
            {
                RegisterPickup(newPickup);
            }
        }

        public void CullUsedPickups()
        {
            for (var i = _activePickups.Count - 1; i >= 0; i--)
            {
                var pickup = _activePickups[i];
                if(!pickup.IsUsed) continue;
                _activePickups.RemoveAt(i);
                Destroy(pickup.gameObject);
            }
        }

        public void TrySpawnPickup(Vector3 position)
        {
            var pickup = pickupPrefabs[0];

            if (_activePickups.Any(activePickup => activePickup.transform.position == position))
            {
                return;
            }

            if (!(pickup.GetComponent<Pickup>().SpawnChance >= Random.Range(0,100))) return;
            var newPickup = Instantiate(pickup, position, Quaternion.identity, transform).GetComponent<Pickup>();
            _activePickups.Add(newPickup);
        }
    }
}