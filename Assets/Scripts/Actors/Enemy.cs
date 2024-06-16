using System.Linq;
using Maihem.Attacks;
using Maihem.Extensions;
using Maihem.Managers;
using Maihem.Movements;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Maihem.Actors
{
    public class Enemy : Actor
    {
        [SerializeField] protected MovementSystem movementSystem;
        public void TakeTurn()
        {
            var player = GameManager.Instance.Player;
            OnTurnStarted();
            if (attackSystem.CanTargetBeHit(GameManager.Instance.Player.GridPosition, GridPosition))
            {
                var dir = new Vector2Int(math.clamp(player.GridPosition.x - GridPosition.x, -1, 1),
                    math.clamp(player.GridPosition.y - GridPosition.y, -1, 1));
                CurrentFacing = CurrentFacing.GetFacingFromDirection(dir);
                attackSystem.Attack(GridPosition, dir, false);
                StartAttackAnimation(GridPosition, CurrentFacing.GetFacingVector(), false);
            }
            else
            {
                if (!TryMove())
                {
                    OnTurnCompleted();
                }
            }
        }

        public void ShowAttackMarkers(bool show)
        {
            if(show)
                attackSystem.ShowTargetMarkers(GridPosition, CurrentFacing.GetFacingVector(), false);
            else
                attackSystem.HideTargetMarkers();
        }

        
        
        private bool TryMove()
        {
            if (!movementSystem) return false;
            var range = attackSystem.currentAttackStrategy.getRange();
            var targetCell = movementSystem.Move(GridPosition,range);
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

        protected override void OnAnimationEnd()
        {
            OnTurnCompleted();
        }
        
    }
}