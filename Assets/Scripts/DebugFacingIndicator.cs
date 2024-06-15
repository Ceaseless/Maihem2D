using System;
using Maihem.Actors;
using UnityEngine;

namespace Maihem
{
    public class DebugFacingIndicator : MonoBehaviour
    {
        private Actor _actor;
        private Facing _lastFrameFacing;
        private void Start()
        {
            _actor = GetComponentInParent<Actor>();
            if (_actor == null)
            {
                Destroy(this);
                return;
            }
            UpdateFacingArrow();
        }

        private void Update()
        {
            if (_lastFrameFacing == _actor.CurrentFacing) return;
            UpdateFacingArrow();
            
        }

        private void UpdateFacingArrow()
        {
            var newZAngle = _actor.CurrentFacing switch
            {
                Facing.East => 0,
                Facing.West => 180f,
                Facing.North => 90f,
                Facing.South => -90,
                Facing.NorthEast => 45f,
                Facing.SouthEast => -45f,
                Facing.SouthWest => -135f,
                Facing.NorthWest => 135f,
                _ => throw new ArgumentOutOfRangeException()
            };
            transform.rotation = Quaternion.Euler(0f,0f,newZAngle);
            _lastFrameFacing = _actor.CurrentFacing;
        }
    }
}
