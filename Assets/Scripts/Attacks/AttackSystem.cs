using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    public class AttackSystem : MonoBehaviour
    {
        [SerializeField] private GameObject targetMarkerPrefab;
        [SerializeField] private int poolStartSize = 10;
        private List<GameObject> _targetMarkerPool;
        private Transform _poolParent;

        public AttackStrategy currentAttackStrategy;
        
        private void Start()
        {
            InitializeMarkerPool();
        }

        private void InitializeMarkerPool()
        {
            if (_targetMarkerPool?.Count > 0)
            {
                foreach (var marker in _targetMarkerPool)
                {
                    Destroy(marker);
                }
                _targetMarkerPool.Clear();
            }
            
            _targetMarkerPool = new List<GameObject>();
            var parentObject = new GameObject("Aim Markers");
            _poolParent = parentObject.transform;
            _poolParent.parent = transform;

            for (var i = 0; i < poolStartSize; i++)
            {
                var newMarker = Instantiate(targetMarkerPrefab, _poolParent);
                newMarker.SetActive(false);
                _targetMarkerPool.Add(newMarker);
            }
        }

        public void Attack(Vector2Int position, Vector2Int direction)
        {
            currentAttackStrategy?.Attack(position, direction);
        }

        public void ShowTargetMarkers(Vector2Int position, Vector2Int direction)
        {
            var positions = currentAttackStrategy.GetAffectedTiles(position, direction);
            if (positions.Count > _targetMarkerPool.Count)
            {
                ExpandPool(positions.Count - _targetMarkerPool.Count);
            }

            foreach (var markerPosition in positions)
            {
                var marker = _targetMarkerPool.Find(o => !o.activeInHierarchy);
                marker.transform.position = MapManager.Instance.CellToWorld(markerPosition);
                marker.SetActive(true);
            }
        }

        public void UpdateTargetMarkerPositions(Vector2Int position, Vector2Int direction)
        {
            var newPositions = currentAttackStrategy.GetAffectedTiles(position, direction);
            foreach (var markerPosition in newPositions)
            {
                var marker = _targetMarkerPool.Find(o => o.activeInHierarchy);
                marker.transform.position = MapManager.Instance.CellToWorld(markerPosition);
            }
        }
        
        public void HideTargetMarkers()
        {
            foreach (var marker in _targetMarkerPool)
            {
                marker.SetActive(false);
            }
        }

        private void ExpandPool(int amount)
        {
            if (amount <= 0)
            {
                //Debug.LogError($"Passed invalid amount $({amount}) to pool expansion");
                return;
            }
            for (var i = 0; i < amount; i++)
            {
                var newMarker = Instantiate(targetMarkerPrefab, _poolParent);
                newMarker.SetActive(false);
                _targetMarkerPool.Add(newMarker);
            }
        }
    }
}
