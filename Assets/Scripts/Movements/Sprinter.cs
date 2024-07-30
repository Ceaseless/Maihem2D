using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Movements
{
    [CreateAssetMenu(menuName = "Movement Strategies/Sprinter")]
    public class Sprinter : MovementStrategy
    {
        public override List<Vector2Int> ActivatedMove(Vector2Int gridPosition, int range)
        {
            var player = GameManager.Instance.Player;
            var shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, player.GridPosition);

            if (shortestPath.Count <= 2)
            {
                shortestPath.RemoveRange(0, shortestPath.Count - 1);
                return shortestPath;
            }

            shortestPath.RemoveRange(0, shortestPath.Count - 2);
            return shortestPath;
        }
    }
}