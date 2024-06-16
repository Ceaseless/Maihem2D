using System.Linq;
using Maihem.Attacks;
using Maihem.Extensions;
using Maihem.Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Actors
{
    public class Enemy : Actor
    {
        [SerializeField] private EnemyHealthDisplay healthDisplay;
        public override void Initialize()
        {
            base.Initialize();
            healthDisplay.SetMaxHealth(MaxHealth);
            healthDisplay.SetHealth(CurrentHealth);
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
            healthDisplay.SetHealth(CurrentHealth);
            
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