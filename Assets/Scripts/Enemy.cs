using System.Linq;
using Unity.Mathematics;
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
            var xdiff = math.clamp(player.GridPosition.x - GridPosition.x, -1, 1);
            var ydiff = math.clamp(player.GridPosition.y - GridPosition.y, -1, 1);

            if (xdiff != 0 && ydiff != 0)
            {
                var newPosition = transform.position + new Vector3(xdiff, ydiff, 0);
                var newGridPosition = MapManager.Instance.GetGridPositionFromWorldPosition(newPosition);
                if (!GameManager.Instance.CellContainsActor(newGridPosition) &&
                    !MapManager.Instance.IsCellBlocking(newGridPosition))
                {
                    StartMoveAnimation(newPosition);
                    UpdateGridPosition(newPosition);
                    return true;
                }
            }

            if (ydiff != 0)
            {
                var newPosition = transform.position + Vector3.up * ydiff;
                var newGridPosition = MapManager.Instance.GetGridPositionFromWorldPosition(newPosition);
                if (!GameManager.Instance.CellContainsActor(newGridPosition) &&
                    !MapManager.Instance.IsCellBlocking(newGridPosition))
                {
                    StartMoveAnimation(newPosition);
                    UpdateGridPosition(newPosition);
                    return true;
                }
            }

            if (xdiff != 0)
            {
                var newPosition = transform.position + Vector3.right * xdiff;
                var newGridPosition = MapManager.Instance.GetGridPositionFromWorldPosition(newPosition);
                if (!GameManager.Instance.CellContainsActor(newGridPosition) &&
                    !MapManager.Instance.IsCellBlocking(newGridPosition))
                {
                    StartMoveAnimation(newPosition);
                    UpdateGridPosition(newPosition);
                    return true;
                }
            }

            return false;
        }

        public override void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth > 0) return;
            OnDied(new DeathEventArgs { DeadGameObject = gameObject });
        }

        protected override void OnMoveAnimationEnd()
        {
            OnTurnCompleted();
        }
    }
}