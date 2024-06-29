using System.Collections.Generic;
using Maihem.Actors;
using Maihem.Effects;
using Maihem.Extensions;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    public class AttackSystem : MonoBehaviour
    {
        [SerializeField] private Color markerColor;
        private Actor _systemOwner;
        private bool _isPlayerOwned;
        private List<TargetMarker> _targetMarkerPool;
        public AttackStrategy currentAttackStrategy;

        private void Awake()
        {
            _systemOwner = GetComponent<Actor>();
            _isPlayerOwned = _systemOwner.GetType() == typeof(Player);
        }

        public bool CanTargetBeHit(Vector2Int target)
        {
            if (!currentAttackStrategy) return false;
            return currentAttackStrategy.GetPossibleTiles(_systemOwner.GridPosition).Contains(target);
        }
        
        public bool Attack()
        {
            if(currentAttackStrategy) {
                return currentAttackStrategy.Attack(_systemOwner.GridPosition, _systemOwner.CurrentFacing.GetFacingVector(), _isPlayerOwned);
            }
            return false;            
        }
        
        public void PlayHitVisualEffect()
        {
            var positions = currentAttackStrategy.GetAffectedTiles(_systemOwner.GridPosition, _systemOwner.CurrentFacing.GetFacingVector(), _isPlayerOwned);
            foreach (var (tile,_) in positions)
            {
                VisualEffectsPool.Instance.PlayVisualEffect(currentAttackStrategy.visualEffect, MapManager.Instance.CellToWorld(tile));
            }
        }
        
        public void ShowTargetMarkers()
        {
            var positions = currentAttackStrategy.GetAffectedTiles(_systemOwner.GridPosition, _systemOwner.CurrentFacing.GetFacingVector(), _isPlayerOwned);
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

        public void UpdateTargetMarkerPositions()
        {
            if (!IsShowingTargetMarkers()) return;
            var newPositions = currentAttackStrategy.GetAffectedTiles(_systemOwner.GridPosition, _systemOwner.CurrentFacing.GetFacingVector(), _isPlayerOwned);
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
