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


        public void Initialize()
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

        public void Tick()
        {
            CullLeftBehindPickups();
            CullUsedPickups();
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

        private void CullLeftBehindPickups()
        {
            var playerPosition = GameManager.Instance.Player.transform.position;
            foreach (var pickup in _activePickups)
            {
                var distance = playerPosition.x - pickup.transform.position.x; // > 0 => Player is on the right
                if (distance > GameManager.Instance.ObjectHorizontalCullDistance)
                {
                    pickup.IsUsed = true;
                }
            }
        }

        public void TrySpawnPickup(Vector3 position)
        {
            if (_activePickups.Any(activePickup => activePickup.transform.position == position))
            {
                return;
            }

            var randomSpawn = Random.Range(0, pickupPrefabs.Length - 1);

            var pickupSpawn = pickupPrefabs[randomSpawn];
            if (!(pickupSpawn.GetComponent<Pickup>().SpawnChance >= Random.Range(0,100))) return;
            var newPickup = Instantiate(pickupSpawn, position, Quaternion.identity, transform).GetComponent<Pickup>();
            _activePickups.Add(newPickup);
        }
    }
}