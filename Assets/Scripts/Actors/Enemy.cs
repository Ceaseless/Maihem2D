using Maihem.Extensions;
using Maihem.Managers;
using Maihem.Movements;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Actors
{
    public class Enemy : Actor
    {
        
        [SerializeField] private EnemyHealthDisplay healthDisplay;
        [SerializeField] private MovementSystem movementSystem;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Move = Animator.StringToHash("Move");

        public override void Initialize()
        {
            base.Initialize();
            healthDisplay.SetMaxHealth(healthSystem.MaxHealth);
            healthDisplay.SetHealth(healthSystem.CurrentHealth);
            animator = GetComponentInChildren<Animator>();
        }
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
                animator.SetInteger(AnimatorHorizontal, dir.x);
                animator.SetInteger(AnimatorVertical, dir.y);
                animator.SetTrigger(Attack);
                StartAttackAnimation(GridPosition, CurrentFacing.GetFacingVector(), false);
            }
            else
            {
                if (!TryMove())
                {
                    OnTurnCompleted();
                }
            }
            attackSystem.UpdateTargetMarkerPositions(GridPosition, CurrentFacing.GetFacingVector(), false);
        }

        public void ShowAttackMarkers(bool show)
        {
            if(show)
                attackSystem.ShowTargetMarkers(GridPosition, CurrentFacing.GetFacingVector(), false);
            else
                attackSystem.HideTargetMarkers();
        }

        protected override void HealthChanged(object sender, HealthChangeEvent healthChangeEvent)
        {
            base.HealthChanged(sender, healthChangeEvent);
            healthDisplay.SetHealth(healthSystem.CurrentHealth);
        }

        private bool TryMove()
        {
            if (!movementSystem) return false;
            var range = attackSystem.currentAttackStrategy.GetRange();
            var targetCell = movementSystem.Move(GridPosition,range);
            var newPosition = MapManager.Instance.CellToWorld(targetCell);
            var newFacingDirection = targetCell - GridPosition;
            CurrentFacing = CurrentFacing.GetFacingFromDirection(newFacingDirection);
            
            animator.SetInteger(AnimatorHorizontal, newFacingDirection.x);
            animator.SetInteger(AnimatorVertical, newFacingDirection.y);
            animator.SetTrigger(Move);
            StartMoveAnimation(newPosition);
            UpdateGridPosition(newPosition);
            return true;

        }
      

        protected override void OnAnimationEnd()
        {
            OnTurnCompleted();
        }

        private void OnDestroy()
        {
            attackSystem?.HideTargetMarkers();
        }
    }
}