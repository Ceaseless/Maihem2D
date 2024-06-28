using System.Collections.Generic;
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
                animator.SetTrigger(AnimatorAttack);
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
            var newPath = new List<Vector3>();
            foreach (var cell in targetCell)
            {
                newPath.Add(MapManager.Instance.CellToWorld(cell));
            }
            var newFacingDirection = targetCell[^1] - GridPosition;
            
            UpdateFacing(newFacingDirection);
            animator.SetTrigger(AnimatorMove);
            StartMoveAnimation(newPath);
            UpdateGridPosition(newPath[^1]);
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