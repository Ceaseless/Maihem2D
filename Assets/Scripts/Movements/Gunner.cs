using System.Collections.Generic;
using System.Linq;
using Maihem.Extensions;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Movements
{
    [CreateAssetMenu(menuName = "Movement Strategies/Gunner")]
    public class Gunner : MovementStrategy
    {

        private static readonly Vector2Int[] CardinalNeighbors = {
            new(1, 0),
            new(-1, 0),
            new(0, 1),
            new(0, -1),
        };
        
        public override List<Vector2Int> ActivatedMove(Vector2Int gridPosition, int attackRange)
        {
            
            var player = GameManager.Instance.Player;
            List<Vector2Int> shortestPath = null;

            var potentialTargets = new List<Vector2Int>();
            foreach (var offset in CardinalNeighbors)
            {
                var testPosition = player.GridPosition + offset * attackRange;
                if(MapManager.Instance.IsCellBlocking(testPosition) || !MapManager.Instance.IsInDirectLine(testPosition, player.GridPosition, attackRange)) continue;
                potentialTargets.Add(testPosition);
            }
            
            if (potentialTargets.Count > 0)
            {
                foreach (var target in potentialTargets.OrderByDescending(x => x.ManhattanDistance(gridPosition)))
                {
                    if (GameManager.Instance.CellContainsActor(target)) continue;
                    shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, target);
                    break;
                }
            }

            if (shortestPath == null || shortestPath.Count == 0)
            {
                shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, player.GridPosition);
            }
            
            shortestPath.RemoveRange(0,shortestPath.Count-1);
           
            return shortestPath;
        }
    }
}
