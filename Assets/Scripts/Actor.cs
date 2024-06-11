using System;
using System.Collections;
using UnityEngine;

namespace Maihem
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Actor : MonoBehaviour
    {
        [Header("Actor Settings")]
        [SerializeField] private int maxHealth;
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private Facing initialFacing;
        public Facing CurrentFacing { get; protected set; }
        public Vector2Int GridPosition { get; protected set; }
        public Collider2D Hurtbox { get; protected set; }
        public bool IsPerformingAction { get; private set; }

        public event EventHandler TurnStarted, TurnCompleted;
        public event EventHandler<DeathEventArgs> Died;

        public int CurrentHealth { get; protected set; }
        public bool IsDead { get; protected set; }

        protected virtual void Awake()
        {
            GridPosition = MapManager.Instance.WorldToCell(transform.position);
            CurrentFacing = initialFacing;
            CurrentHealth = maxHealth;
        }

        protected virtual void Start()
        {
            Hurtbox = GetComponent<Collider2D>();
        }

        protected void UpdateGridPosition(Vector3 newPosition)
        {
            GridPosition = MapManager.Instance.WorldToCell(newPosition);
        }

        protected void StartMoveAnimation(Vector3 target)
        {
            StartCoroutine(MoveAnimation(target));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MoveAnimation(Vector3 target)
        {
            IsPerformingAction = true;
            var time = 0f;
            var startPosition = transform.position;
            while (time < moveDuration && IsPerformingAction)
            {
                transform.position = Vector3.Lerp(startPosition, target, time / moveDuration);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            IsPerformingAction = false;
            OnMoveAnimationEnd();
        }

        public abstract void TakeDamage(int damage);
        protected abstract void OnMoveAnimationEnd();

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
        public GameObject DeadGameObject { get; set; }
    }
}