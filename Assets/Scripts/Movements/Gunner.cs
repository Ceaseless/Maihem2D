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
        public override Vector2Int ActivatedMove(Vector2Int gridPosition, int attackRange)
        {
            var player = GameManager.Instance.Player;
            List<Vector2Int> shortestPath;
            
            var maxRangePositions = new List<Vector2Int>
            {
                new (player.GridPosition.x + attackRange, player.GridPosition.y),
                new (player.GridPosition.x - attackRange, player.GridPosition.y),
                new (player.GridPosition.x,player.GridPosition.y + attackRange),
                new (player.GridPosition.x,player.GridPosition.y - attackRange)
            };

            foreach (var position in maxRangePositions.ToList().Where(position => MapManager.Instance.IsCellBlocking(position) || !MapManager.Instance.IsInDirectLine(position, player.GridPosition,attackRange)))
            { 
                maxRangePositions.Remove(position);
            }

            if (maxRangePositions.Count > 0)
            {
                var maxRangeTarget = maxRangePositions.OrderBy(x => x.ManhattanDistance(player.GridPosition)).First();
                shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, maxRangeTarget);
            }
            else
            {
                shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, player.GridPosition);
            }
            
            return shortestPath[^1];
        }

        
        

    }
}
