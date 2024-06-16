using UnityEngine;
using Random = System.Random;

namespace Maihem.Movements
{
    public class MovementSystem : MonoBehaviour
    {
        [SerializeField] private MovementStrategy currentStrategy;

        private Random _randomNumbers;

        private bool _isActivated;

        private void Awake()
        {
            _isActivated = false;
            _randomNumbers = new Random();
        }

        private void CheckAlert( Vector2Int gridPosition)
        {
            if (_isActivated == false)
            {
                _isActivated = currentStrategy.CheckAlert(gridPosition);
            }
        }

        public Vector2Int Move(Vector2Int gridPosition, int range)
        {
            CheckAlert(gridPosition);
            return _isActivated ? currentStrategy.ActivatedMove(gridPosition,range) : MovementStrategy.IdleMove(gridPosition,_randomNumbers);
        }
    }
}
