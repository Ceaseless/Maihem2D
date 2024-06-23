using Maihem.Managers;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Maihem.Movements
{
    public abstract class MovementStrategy : ScriptableObject
    {
        [SerializeField] private float alertRange;
        
        public static Vector2Int IdleMove(Vector2Int gridPosition)
        {
            var offsetLength = MapManager.CellNeighborOffsets.Length;
            for (var i = 0; i < 20; i++)
            {
                var randomOffsetIndex = Random.Range(0, offsetLength);
                var randomNeighbor = gridPosition + MapManager.CellNeighborOffsets[randomOffsetIndex];
                if (!(MapManager.Instance.IsCellBlocking(randomNeighbor) &&
                      MapManager.Instance.IsCellBlockedDiagonal(randomNeighbor, gridPosition)))
                {
                    return randomNeighbor;
                }
            } 
            return Vector2Int.zero;
        }
        
        public bool CheckAlert(Vector2Int gridPosition)
        {
            var player = GameManager.Instance.Player;
            var distance = Vector2Int.Distance(player.GridPosition, gridPosition);

            return distance <= alertRange;
        }

        public abstract Vector2Int ActivatedMove(Vector2Int gridPosition, int range);
    }
}
