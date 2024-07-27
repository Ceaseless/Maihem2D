using System;
using System.Collections;
using System.Collections.Generic;
using Maihem.Attacks;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [Header("Actor Settings")]
        [SerializeField] private int maxHealth;
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private Facing initialFacing;
        [SerializeField] protected AttackSystem attackSystem;
        [SerializeField] protected Animator animator;
        public HealthSystem healthSystem;
        public Facing CurrentFacing { get; protected set; }
        public Vector2Int GridPosition { get; protected set; }

        public bool IsDead { get; protected set; }
        public bool IsPerformingAction { get; protected set; }

        public event EventHandler TurnStarted, TurnCompleted;
        public event EventHandler<DeathEventArgs> Died;

        protected static readonly int AnimatorHorizontal = Animator.StringToHash("Horizontal");
        protected static readonly int AnimatorVertical = Animator.StringToHash("Vertical");
        protected static readonly int AnimatorMoving = Animator.StringToHash("Moving");
        
        public virtual void Initialize()
        {
            GridPosition = MapManager.Instance.WorldToCell(transform.position);
            CurrentFacing = initialFacing;
            healthSystem.OnHealthChange += HealthChanged;
            healthSystem.RecoverFullHealth();
        }
        
        protected virtual void HealthChanged(object sender, HealthChangeEvent healthChangeEvent)
        {
            if (healthSystem.IsDead)
            {
                IsDead = true;
                Died?.Invoke(this, new DeathEventArgs {DeadGameObject = gameObject, Reason = DeathEventArgs.DeathReason.Damage});
            }
        }

        public virtual void Tick()
        {
            healthSystem.Tick();
        }

        protected void UpdateGridPosition(Vector2Int newCellPosition)
        {
            GridPosition = newCellPosition;
        }

        protected void UpdateGridPosition(Vector3 newPosition)
        {
            GridPosition = MapManager.Instance.WorldToCell(newPosition);
        }
        
        protected void StartMoveAnimation(List<Vector2Int> cellPath)
        {
            StartCoroutine(MoveAnimation(cellPath));
        }
        
        protected void StartMoveAnimation(List<Vector3> path)
        {
            StartCoroutine(MoveAnimation(path));
        }
        
        protected void StartMoveAnimation(Vector3 target)
        {
            var singleTarget = new List<Vector3> { target };
            StartCoroutine(MoveAnimation(singleTarget));
        }

        
        
        private IEnumerator MoveAnimation(List<Vector2Int> path)
        {
            IsPerformingAction = true;
            animator.SetBool(AnimatorMoving,true);
            path.Reverse();
            var timePerTarget = moveDuration / path.Count;
            foreach (var nodeInPath in path)
            {
                var time = 0f;
                var startPosition = transform.position;
                var subTarget = MapManager.Instance.CellToWorld(nodeInPath);
                while (time < timePerTarget && IsPerformingAction)
                {
                    transform.position = Vector3.Lerp(startPosition, subTarget, time / timePerTarget);
                    time += Time.deltaTime;
                    yield return null;
                }
                transform.position = subTarget;
            }
            animator.SetBool(AnimatorMoving,false);
            IsPerformingAction = false;
            OnAnimationEnd();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MoveAnimation(List<Vector3> target)
        {
            IsPerformingAction = true;
            animator.SetBool(AnimatorMoving,true);
            target.Reverse();
            var timePerTarget = moveDuration / target.Count;
            foreach (var subTarget in target)
            {
                var time = 0f;
                var startPosition = transform.position;
                while (time < timePerTarget && IsPerformingAction)
                {
                    transform.position = Vector3.Lerp(startPosition, subTarget, time / timePerTarget);
                    time += Time.deltaTime;
                    yield return null;
                }
                transform.position = subTarget;
            }
            animator.SetBool(AnimatorMoving,false);
            IsPerformingAction = false;
            OnAnimationEnd();
        }
        
       
        protected abstract void OnAnimationEnd();

        protected virtual void OnTurnStarted()
        {
            TurnStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTurnCompleted()
        {
            TurnCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDied(DeathEventArgs deathArgs)
        {
            Died?.Invoke(this, deathArgs);
        }
        
    }

    public class DeathEventArgs : EventArgs
    {
        public enum DeathReason
        {
            Default,
            Damage,
            Removed,
        }
        public GameObject DeadGameObject { get; set; }
        public DeathReason Reason { get; set; }
    }
}