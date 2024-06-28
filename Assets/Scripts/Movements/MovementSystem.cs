using System.Collections.Generic;
using UnityEngine;

namespace Maihem.Movements
{
    public class MovementSystem : MonoBehaviour
    {
        [SerializeField] private MovementStrategy currentStrategy;

        private bool _isActivated;

        private void Awake()
        {
            _isActivated = false;
        }

        private void CheckAlert( Vector2Int gridPosition)
        {
            if (_isActivated == false)
            {
                _isActivated = currentStrategy.CheckAlert(gridPosition);
            }
        }

        public List<Vector2Int> Move(Vector2Int gridPosition, int range)
        {
            CheckAlert(gridPosition);
            return _isActivated ? currentStrategy.ActivatedMove(gridPosition,range) : MovementStrategy.IdleMove(gridPosition);
        }
    }
}
