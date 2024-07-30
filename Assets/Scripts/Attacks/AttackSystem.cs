using System.Collections.Generic;
using Maihem.Actors;
using Maihem.Attacks.AttackMarkers;
using Maihem.Extensions;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    public class AttackSystem : MonoBehaviour
    {
        [SerializeField] private Color markerColor;
        public AttackStrategy currentAttackStrategy;
        private bool _isPlayerOwned;
        private Actor _systemOwner;
        private List<TargetMarker> _targetMarkerPool;

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

        public void PerformAttack()
        {
            if (currentAttackStrategy)
            {
                var hitSomething = currentAttackStrategy.Attack(_systemOwner.GridPosition,
                    _systemOwner.CurrentFacing.GetFacingVector(), _isPlayerOwned);
                if (!hitSomething)
                {
                    // Play whiff sound effect
                }
            }
        }


        public void ShowTargetMarkers()
        {
            var positions = currentAttackStrategy.GetAffectedTiles(_systemOwner.GridPosition,
                _systemOwner.CurrentFacing.GetFacingVector(), _isPlayerOwned);
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
            var newPositions = currentAttackStrategy.GetAffectedTiles(_systemOwner.GridPosition,
                _systemOwner.CurrentFacing.GetFacingVector(), _isPlayerOwned);
            var diff = newPositions.Count - _targetMarkerPool.Count;
            if (diff > 0)
            {
                var newMarkers = MarkerPool.Instance.GetMarkers(diff);
                foreach (var marker in newMarkers)
                {
                    marker.SetMarkerColor(markerColor);
                    marker.ShowMarker();
                }

                _targetMarkerPool.AddRange(newMarkers);
            }

            for (var i = 0; i < newPositions.Count; i++)
            {
                var marker = _targetMarkerPool[i];
                var (markerPosition, markerDamage) = newPositions[i];
                marker.transform.position = MapManager.Instance.CellToWorld(markerPosition);
                marker.SetMarkerText(markerDamage.ToString());
            }

            if (diff < 0)
                for (var i = newPositions.Count; i < _targetMarkerPool.Count; i++)
                    _targetMarkerPool[i].transform.position = Vector3.left;
        }

        public void HideTargetMarkers()
        {
            if (_targetMarkerPool == null) return;

            foreach (var marker in _targetMarkerPool) marker.DisableMarker();
            _targetMarkerPool.Clear();
        }

        public bool IsShowingTargetMarkers()
        {
            if (_targetMarkerPool == null) return false;
            return _targetMarkerPool.Count > 0;
        }
    }
}