using System.Collections.Generic;
using System.Linq;
using Maihem.Pickups;
using UnityEngine;

namespace Maihem.Managers
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] pickupPrefabs;
        [SerializeField] private ObjectBoundSettings boundSettings;
        private List<Pickup> _activePickups;
        private int _spawnTimer;
        private Transform _cameraTransform;

        public void Initialize()
        {
            _activePickups = new List<Pickup>();
            _cameraTransform = Camera.main?.transform;
        }
        
        public void ResetState()
        {
            for (var i = _activePickups.Count - 1; i >= 0; i--)
            {
                var enemy = _activePickups[i];
                _activePickups.RemoveAt(i);
                Destroy(enemy.gameObject);
            }
            _activePickups.Clear();
        }
        
        public void UpdatePickupsActiveState()
        {
            var checkPosition = _cameraTransform.position;
            foreach (var pickup in _activePickups)
            {
                if(pickup.gameObject.activeInHierarchy) continue;
                if (!boundSettings.IsInActivateDistance(checkPosition, pickup.transform.position)) continue;
                pickup.gameObject.SetActive(true);
            }
        }

        public void Tick()
        {
            CullLeftBehindPickups();
            CullUsedPickups();
            UpdatePickupsActiveState();
        }

        private void RegisterPickup(Pickup newPickup)
        {
            newPickup.gameObject.SetActive(false);
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
            var checkPosition = _cameraTransform.position;
            foreach (var pickup in _activePickups)
            {
                if (boundSettings.IsOutsideOfCullDistance(checkPosition, pickup.transform.position))
                {
                    pickup.IsUsed = true;
                }
            }
        }

        public void TrySpawnPickup(Vector3 position, LootTable table)
        {
            if (!table) return;
            var pickupPosition = position;

            var pickupSpawn = table.rollOnLootTable();
            if (!pickupSpawn) return;
            
            if (_activePickups.Any(activePickup => activePickup.transform.position == position))
            {
                var allNeighbours = MapManager.GetNeighbourPositions(MapManager.Instance.WorldToCell(position));
                var possibleNeighbours = new List<Vector2Int>();
                foreach (var neighbour in allNeighbours)
                {
                    var worldPosition = MapManager.Instance.CellToWorld(neighbour);
                    if (GameManager.Instance.Player.GridPosition != neighbour && !MapManager.Instance.IsCellBlocking(neighbour) && _activePickups.All(activePickup => activePickup.transform.position != worldPosition))
                    {
                        possibleNeighbours.Add(neighbour);
                    }
                }

                if (possibleNeighbours.Count == 0) return;
                
                var randomPosition = possibleNeighbours[Random.Range(0, possibleNeighbours.Count)];
                pickupPosition = MapManager.Instance.CellToWorld(randomPosition);
            }
            
            var newPickup = Instantiate(pickupSpawn, pickupPosition, Quaternion.identity, transform).GetComponent<Pickup>();
            _activePickups.Add(newPickup);
        }
    }
}