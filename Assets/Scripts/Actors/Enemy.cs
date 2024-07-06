using Maihem.Extensions;
using Maihem.Managers;
using Maihem.Movements;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maihem.Actors
{
    public class Enemy : Actor
    {
        
        [SerializeField] private EnemyHealthDisplay healthDisplay;
        [SerializeField] private MovementSystem movementSystem;
        private static readonly int AnimatorAttack = Animator.StringToHash("Attack");
        

        public override void Initialize()
        {
            base.Initialize();
            healthDisplay.SetMaxHealth(healthSystem.MaxHealth);
            healthDisplay.SetHealth(healthSystem.CurrentHealth);
        }
        public void TakeTurn()
        {
            var player = GameManager.Instance.Player;
            OnTurnStarted();
            if (attackSystem.CanTargetBeHit(GameManager.Instance.Player.GridPosition))
            {
                var dir = new Vector2Int(math.clamp(player.GridPosition.x - GridPosition.x, -1, 1),
                    math.clamp(player.GridPosition.y - GridPosition.y, -1, 1));
                UpdateFacing(dir);
                
                IsPerformingAction = true;
                animator.SetTrigger(AnimatorAttack);
                //attackSystem.PerformAttack();
                //StartAttackAnimation();
            }
            else
            {
                if (!TryMove())
                {
                    OnTurnCompleted();
                }
            }
            attackSystem.UpdateTargetMarkerPositions();
        }

        public void ShowAttackMarkers(bool show)
        {
            if(show)
                attackSystem.ShowTargetMarkers();
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

            if (movementSystem.TryMove(GridPosition, range, out var movePath))
            {
                var newFacingDirection = movePath[^1] - GridPosition;
                UpdateFacing(newFacingDirection);
                StartMoveAnimation(movePath);
                UpdateGridPosition(movePath[^1]);
                return true;
            }

            var randomFacing = MapManager.CellNeighborOffsets[Random.Range(0, MapManager.CellNeighborOffsets.Length)];
            UpdateFacing(randomFacing);
            return false;
            
            
            
            
        }
      

        protected override void OnAnimationEnd()
        {
            IsPerformingAction = false;
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