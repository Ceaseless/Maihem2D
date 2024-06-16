﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maihem.Attacks;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Actors
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Actor : MonoBehaviour
    {
        [Header("Actor Settings")]
        [SerializeField] private int maxHealth;
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private Facing initialFacing;
        [SerializeField] protected AttackSystem attackSystem;
        public Facing CurrentFacing { get; protected set; }
        public Vector2Int GridPosition { get; protected set; }
        public Collider2D Hurtbox { get; protected set; }
        public int MaxHealth => maxHealth;
        public int CurrentHealth { get; protected set; }
        public bool IsDead { get; protected set; }
        public bool IsPerformingAction { get; private set; }

        public event EventHandler TurnStarted, TurnCompleted;
        public event EventHandler<DeathEventArgs> Died;

        
     

        public virtual void Initialize()
        {
            GridPosition = MapManager.Instance.WorldToCell(transform.position);
            CurrentFacing = initialFacing;
            CurrentHealth = maxHealth;
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

        protected void StartAttackAnimation(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            StartCoroutine(AttackAnimation(position, direction, isPlayerAttack));
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
            OnAnimationEnd();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator AttackAnimation(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            IsPerformingAction = true;
            var currentAttackStrategy = attackSystem.currentAttackStrategy;
            var positions = currentAttackStrategy.GetAffectedTiles(position, direction, isPlayerAttack);
            var activeAnimations = new List<GameObject>();
            
            foreach (var tile in positions)
            {
                activeAnimations.Add(Instantiate(currentAttackStrategy.attackAnimation, MapManager.Instance.CellToWorld(tile),
                    Quaternion.identity));
            }

            while (activeAnimations.Any())
            {
                
                activeAnimations.RemoveAll(s => !s);
                yield return null;
            }

            IsPerformingAction = false;
            OnAnimationEnd();
            
        }

        public abstract void TakeDamage(int damage);
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
        public GameObject DeadGameObject { get; set; }
    }
}