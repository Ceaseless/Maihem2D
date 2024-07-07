using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Maihem.Movements
{
    public abstract class MovementStrategy : ScriptableObject
    {
        [SerializeField] private float alertRange;
        
        public static bool TryIdleMove(Vector2Int gridPosition, out List<Vector2Int> path)
        {
            var offsetLength = MapManager.CellNeighborOffsets.Length;
            for (var i = 0; i < 20; i++)
            {
                var randomOffsetIndex = Random.Range(0, offsetLength);
                var randomNeighbor = gridPosition + MapManager.CellNeighborOffsets[randomOffsetIndex];
                if (!MapManager.Instance.IsCellBlocking(randomNeighbor) &&
                      !MapManager.Instance.IsCellBlockedDiagonal(randomNeighbor, gridPosition) &&
                      !GameManager.Instance.CellContainsActor(randomNeighbor))
                {
                    path = new List<Vector2Int> { randomNeighbor };
                    return true;
                }
            } 
            path = null;
            return false;
        }
        
        public bool CheckAlert(Vector2Int gridPosition)
        {
            var player = GameManager.Instance.Player;
            var distance = Vector2Int.Distance(player.GridPosition, gridPosition);

            return distance <= alertRange;
        }

        public abstract List<Vector2Int> ActivatedMove(Vector2Int gridPosition, int range);
    }
}
