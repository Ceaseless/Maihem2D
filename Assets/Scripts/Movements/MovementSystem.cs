using System.Collections.Generic;
using UnityEngine;

namespace Maihem.Movements
{
    public class MovementSystem : MonoBehaviour
    {
        [SerializeField] private float waitChanceWhileIdle = 0.5f;
        [SerializeField] private MovementStrategy currentStrategy;

        private bool _isActivated;

        private void Awake()
        {
            _isActivated = false;
        }

        private void CheckAlert( Vector2Int gridPosition)
        {
            if (_isActivated) return;
            _isActivated = currentStrategy.CheckAlert(gridPosition);
        }

        public bool TryMove(Vector2Int gridPosition, int range, out List<Vector2Int> path)
        {
            CheckAlert(gridPosition);
            if (!_isActivated)
            {
                // Wait instead of trying to idle move
                if (waitChanceWhileIdle >= Random.Range(0, 1f))
                {
                    path = null;
                    return false;
                }

                // Idle move found a position
                if (MovementStrategy.TryIdleMove(gridPosition, out path))
                {
                    return true;
                }
                
                // Idle move didn't find a position -> Wait
                path = null;
                return false;
            }

            // We are active -> Move
            path = currentStrategy.ActivatedMove(gridPosition, range);
            return true;
        }
    }
}
