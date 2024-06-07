using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem
{
    public class Enemy : Actor
    {
        
        
        public void TakeTurn()
        {
            OnTurnStarted?.Invoke(this, EventArgs.Empty);
            var player = GameManager.Instance.Player;
            var neighbours = MapManager.GetNeighbourPositions(GridPosition);
            if (!neighbours.Contains(player.GridPosition))
            {
                if (!TryMove())
                {
                    OnTurnCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                Debug.Log("Attacking player!", this);
                player.TakeDamage(1);
                OnTurnCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
        
        
        
        private bool TryMove()
        {
            var player = GameManager.Instance.Player;
            var shortestPath = MapManager.Instance.FindShortestDistance(MapManager.Instance.GetGridPositionFromWorldPosition(transform.position), MapManager.Instance.GetGridPositionFromWorldPosition(player.transform.position));
            
            if (shortestPath != null )
            {
                var newPosition = MapManager.Instance.GetWorldPositionFromGridPosition(shortestPath.Last());
                
                StartMoveAnimation(newPosition);
                UpdateGridPosition(newPosition);
                return true;
            }
            return false;
        }

        public override void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth > 0) return;
            OnDied?.Invoke(this, new DeathEventArgs { DeadGameObject = gameObject });
        }

        protected override void OnMoveAnimationEnd()
        {
            OnTurnCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
    
}