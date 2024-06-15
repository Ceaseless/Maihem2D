using System;
using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    public class AttackSystem : MonoBehaviour
    {
        public Color markerColor;
        private List<GameObject> _targetMarkerPool;

        public AttackStrategy currentAttackStrategy;

        public bool CanTargetBeHit(Vector2Int target, Vector2Int position)
        {
            if (!currentAttackStrategy) return false;
            return currentAttackStrategy.GetPossibleTiles(position).Contains(target);
        }
        
        public bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            if(currentAttackStrategy) {
                PlayAttackAnimation(position, direction);
                return currentAttackStrategy.Attack(position, direction, isPlayerAttack);
            }
            return false;            
        }

        public void ShowTargetMarkers(Vector2Int position, Vector2Int direction)
        {
            var positions = currentAttackStrategy.GetAffectedTiles(position, direction);
            _targetMarkerPool?.Clear();
            _targetMarkerPool = MarkerPool.Instance.GetMarkers(positions.Count);
            for (var i = 0; i < positions.Count; i++)
            {
                var marker = _targetMarkerPool[i];
                var markerPosition = positions[i];
                marker.transform.position = MapManager.Instance.CellToWorld(markerPosition);
                marker.GetComponent<SpriteRenderer>().color = markerColor;
                marker.SetActive(true);
            }
        }

        public void UpdateTargetMarkerPositions(Vector2Int position, Vector2Int direction)
        {
            var newPositions = currentAttackStrategy.GetAffectedTiles(position, direction);
            for (var i = 0; i < newPositions.Count; i++)
            {
                var marker = _targetMarkerPool[i];
                var markerPosition = newPositions[i];
                marker.transform.position = MapManager.Instance.CellToWorld(markerPosition);
            }
        }
        public void HideTargetMarkers()
        {
            if (_targetMarkerPool == null) return;
            
            foreach (var marker in _targetMarkerPool)
            {
                marker.SetActive(false);
            }
            _targetMarkerPool.Clear();
        }

        private void PlayAttackAnimation(Vector2Int position, Vector2Int direction)
        {
            var positions = currentAttackStrategy.GetAffectedTiles(position, direction);

            foreach (var tile in positions)
            {
                Instantiate(currentAttackStrategy.attackAnimation, MapManager.Instance.CellToWorld(tile),
                    Quaternion.identity);
            }
        }
    }
}
