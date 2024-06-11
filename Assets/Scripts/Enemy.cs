using System.Linq;
using UnityEngine;

namespace Maihem
{
    public class Enemy : Actor
    {
        public void TakeTurn()
        {
            OnTurnStarted();
            var player = GameManager.Instance.Player;
            var neighbours = MapManager.GetNeighbourPositions(GridPosition);
            if (!neighbours.Contains(player.GridPosition))
            {
                if (!TryMove())
                {
                    OnTurnCompleted();
                }
            }
            else
            {
                Debug.Log("Attacking player!", this);
                player.TakeDamage(1);
                OnTurnCompleted();
            }
        }
        
        
        
        private bool TryMove()
        {
            var player = GameManager.Instance.Player;
            var shortestPath = MapManager.Instance.FindShortestDistance(MapManager.Instance.GetGridPositionFromWorldPosition(transform.position), MapManager.Instance.GetGridPositionFromWorldPosition(player.transform.position));

            if (shortestPath == null) return false;
            var newPosition = MapManager.Instance.GetWorldPositionFromGridPosition(shortestPath.Last());
                
            StartMoveAnimation(newPosition);
            UpdateGridPosition(newPosition);
            return true;
        }

        public override void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth > 0) return;
            UIManager.Instance.AdjustKillCount(1);
            OnDied(new DeathEventArgs { DeadGameObject = gameObject });
        }

        protected override void OnMoveAnimationEnd()
        {
            OnTurnCompleted();
        }
    }
}