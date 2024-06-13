using System.Linq;
using Maihem.Extensions;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Actors
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
                CurrentFacing = CurrentFacing.GetFacingFromDirection(player.GridPosition - GridPosition);
                player.TakeDamage(1);
                OnTurnCompleted();
            }
        }
        
        
        
        private bool TryMove()
        {
            var player = GameManager.Instance.Player;
            var shortestPath = MapManager.Instance.FindShortestDistance(MapManager.Instance.WorldToCell(transform.position), MapManager.Instance.WorldToCell(player.transform.position));

            if (shortestPath == null) return false;
            var targetCell = shortestPath.Last();
            var newPosition = MapManager.Instance.CellToWorld(targetCell);
            CurrentFacing = CurrentFacing.GetFacingFromDirection(targetCell - GridPosition);
                
            StartMoveAnimation(newPosition);
            UpdateGridPosition(newPosition);
            return true;
        }

        public override void TakeDamage(int damage)
        {
            if(IsDead) return;
            CurrentHealth -= damage;
            
            if (CurrentHealth > 0) return;
            IsDead = true;
            OnDied(new DeathEventArgs { DeadGameObject = gameObject });
        }

        protected override void OnMoveAnimationEnd()
        {
            OnTurnCompleted();
        }
    }
}