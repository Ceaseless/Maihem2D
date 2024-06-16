using System;
using System.Collections.Generic;
using System.Linq;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Movements
{
    [CreateAssetMenu(menuName = "Movement Strategies/Gunner")]
    public class Gunner : MovementStrategy
    {
        public override Vector2Int ActivatedMove(Vector2Int gridPosition, int attackRange)
        {
            var inRange = attackRange - 1;
            var player = GameManager.Instance.Player;
            
            var maxRangePositions = new List<Vector2Int>
            {
                new (player.GridPosition.x + inRange, player.GridPosition.y),
                new (player.GridPosition.x - inRange, player.GridPosition.y),
                new (player.GridPosition.x,player.GridPosition.y + inRange),
                new (player.GridPosition.x,player.GridPosition.y - inRange)
            };

            foreach (var position in maxRangePositions.ToList().Where(position => MapManager.Instance.IsCellBlocking(position)))
            { 
                maxRangePositions.Remove(position);
            }

            var checkTile = maxRangePositions[0];
            var tileDistance = GetDistance(checkTile, player.GridPosition);
            
            foreach (var position in maxRangePositions)
            {
                var positionDistance = GetDistance(position, player.GridPosition);
                if (positionDistance >= tileDistance) continue;
                checkTile = position;
                tileDistance = GetDistance(checkTile, player.GridPosition);

            }
            var shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, checkTile);
      
            
            return shortestPath[^1];
        }

        private static int GetDistance(Vector2Int a, Vector2Int b)
        {
            if (Math.Abs(a.x) == Math.Abs(b.x) && Math.Abs(a.y) == Math.Abs(b.x)) return Math.Abs(a.x);
            return Math.Abs(a.x + b.x) + Math.Abs(a.y + b.y);
        }
    }
}
