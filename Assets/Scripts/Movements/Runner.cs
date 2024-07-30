using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Movements
{
    [CreateAssetMenu(menuName = "Movement Strategies/Runner")]
    public class Runner : MovementStrategy
    {
        public override List<Vector2Int> ActivatedMove(Vector2Int gridPosition, int range)
        {
            var player = GameManager.Instance.Player;
            var shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, player.GridPosition);

            if (shortestPath.Count > 0) shortestPath.RemoveRange(0, shortestPath.Count - 1);
            return shortestPath;
        }
    }
}