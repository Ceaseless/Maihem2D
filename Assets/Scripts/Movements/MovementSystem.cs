using System.Collections.Generic;
using Maihem.Actors;
using Maihem.Extensions;
using UnityEngine;

namespace Maihem.Movements
{
    public class MovementSystem : MonoBehaviour
    {
        [SerializeField] private float waitChanceWhileIdle = 0.5f;
        [SerializeField] private MovementStrategy currentStrategy;

        private Actor _parentActor;
        private bool _isActivated;

        private void Awake()
        {
            _parentActor = GetComponent<Actor>();
            _isActivated = false;
        }

        private void CheckAlert()
        {
            if (_isActivated) return;
            _isActivated = currentStrategy.CheckAlert(_parentActor.GridPosition);
        }

        public bool TryMove(int range, out List<Vector2Int> path)
        {
            CheckAlert();
            if (!_isActivated)
            {
                // Wait instead of trying to idle move
                if (waitChanceWhileIdle >= Random.Range(0, 1f))
                {
                    path = null;
                    return false;
                }

                // Idle move found a position
                if (MovementStrategy.TryIdleMove(_parentActor.GridPosition, _parentActor.CurrentFacing.GetFacingVector(), out path))
                {
                    return true;
                }
                
                // Idle move didn't find a position -> Wait
                path = null;
                return false;
            }

            // We are active -> Move
            path = currentStrategy.ActivatedMove(_parentActor.GridPosition, range);
            return true;
        }
    }
}
