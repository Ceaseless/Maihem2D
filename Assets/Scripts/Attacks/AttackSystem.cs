using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    public class AttackSystem : MonoBehaviour
    {
        public Color markerColor;
        private List<TargetMarker> _targetMarkerPool;

        public AttackStrategy currentAttackStrategy;

        public bool CanTargetBeHit(Vector2Int target, Vector2Int position)
        {
            if (!currentAttackStrategy) return false;
            return currentAttackStrategy.GetPossibleTiles(position).Contains(target);
        }
        
        public bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            if(currentAttackStrategy) {
                return currentAttackStrategy.Attack(position, direction, isPlayerAttack);
            }
            return false;            
        }

        public void ShowTargetMarkers(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var positions = currentAttackStrategy.GetAffectedTiles(position, direction, isPlayerAttack);
            _targetMarkerPool?.Clear();
            _targetMarkerPool = MarkerPool.Instance.GetMarkers(positions.Count);
            for (var i = 0; i < positions.Count; i++)
            {
                var marker = _targetMarkerPool[i];
                var (markerPosition, markerDamage) = positions[i];
                marker.transform.position = MapManager.Instance.CellToWorld(markerPosition);
                marker.SetMarkerVisuals(markerColor, markerDamage.ToString());
                marker.ShowMarker();
            }
        }

        public void UpdateTargetMarkerPositions(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            if (!IsShowingTargetMarkers()) return;
            var newPositions = currentAttackStrategy.GetAffectedTiles(position, direction, isPlayerAttack);
            for (var i = 0; i < newPositions.Count; i++)
            {
                var marker = _targetMarkerPool[i];
                var (markerPosition, markerDamage) = newPositions[i];
                marker.transform.position = MapManager.Instance.CellToWorld(markerPosition);
                
                marker.SetMarkerText(markerDamage.ToString());
                
            }
        }
        public void HideTargetMarkers()
        {
            if (_targetMarkerPool == null) return;
            
            foreach (var marker in _targetMarkerPool)
            {
                marker.HideMarker();
            }
            _targetMarkerPool.Clear();
        }

        public bool IsShowingTargetMarkers()
        {
            if (_targetMarkerPool == null) return false;
            return _targetMarkerPool.Count > 0;
        }

        
    }
}
