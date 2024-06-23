using Maihem.Managers;
using UnityEngine;

namespace Maihem.Movements
{
    [CreateAssetMenu(menuName = "Movement Strategies/Runner")]
    public class Runner : MovementStrategy
    {
        public override Vector2Int ActivatedMove(Vector2Int gridPosition, int range)
        {
            var player = GameManager.Instance.Player;
            var shortestPath = MapManager.Instance.FindShortestDistance(gridPosition, player.GridPosition);
            
            return shortestPath[^1];
        }
    }
}
