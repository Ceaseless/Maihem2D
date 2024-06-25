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
                UpdateFacing(dir);
                animator.SetTrigger(Attack);
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
            
            UpdateFacing(newFacingDirection);
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
        
        private void UpdateFacing(Vector2Int newFacingVector)
        {
            animator.SetInteger(AnimatorHorizontal, newFacingVector.x);
            animator.SetInteger(AnimatorVertical, newFacingVector.y);
            CurrentFacing = CurrentFacing.GetFacingFromDirection(newFacingVector);
        }
    }
}