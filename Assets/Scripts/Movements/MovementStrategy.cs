using System;
using System.Linq;
using Maihem.Actors;
using Maihem.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Maihem.Movements
{
    public abstract class MovementStrategy : ScriptableObject
    {
        [SerializeField] private float alertRange;
        public static Vector2Int IdleMove(Vector2Int gridPosition, Random randomGenerator)
        {
            var neighbours = MapManager.GetNeighbourPositions(gridPosition);

            foreach (var neighbour in neighbours.ToList())
            {
                if (MapManager.Instance.IsCellBlocking(neighbour) || MapManager.Instance.IsCellBlockedDiagonal(neighbour,gridPosition))
                {
                    neighbours.Remove(neighbour);
                }
            }

            var randomNumber = randomGenerator.Next(0, neighbours.Count - 1);
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
