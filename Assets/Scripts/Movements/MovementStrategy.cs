using System.Linq;
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
            var neighbours = MapManager.GetNeighbourPositions(gridPosition);

            foreach (var neighbour in neighbours.ToList())
            {
                if (MapManager.Instance.IsCellBlocking(neighbour) || MapManager.Instance.IsCellBlockedDiagonal(neighbour,gridPosition))
                {
                    neighbours.Remove(neighbour);
                }
            }

            var randomNumber = Random.Range(0, neighbours.Count);
            return neighbours[randomNumber];
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
