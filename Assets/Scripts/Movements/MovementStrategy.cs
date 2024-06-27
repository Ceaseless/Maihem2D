using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Maihem.Movements
{
    public abstract class MovementStrategy : ScriptableObject
    {
        [SerializeField] private float alertRange;
        
        public static List<Vector2Int> IdleMove(Vector2Int gridPosition)
        {
            var randomPath = new List<Vector2Int>();
            var offsetLength = MapManager.CellNeighborOffsets.Length;
            for (var i = 0; i < 20; i++)
            {
                var randomOffsetIndex = Random.Range(0, offsetLength);
                var randomNeighbor = gridPosition + MapManager.CellNeighborOffsets[randomOffsetIndex];
                if (!MapManager.Instance.IsCellBlocking(randomNeighbor) &&
                      !MapManager.Instance.IsCellBlockedDiagonal(randomNeighbor, gridPosition))
                {
                    randomPath.Add(randomNeighbor);
                    return randomPath;
                }
            } 
            Debug.Log("No Neighbour free");
            randomPath.Add(Vector2Int.zero);
            return randomPath;
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
