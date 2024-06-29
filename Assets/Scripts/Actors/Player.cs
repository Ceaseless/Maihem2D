using System;
using System.Collections.Generic;
using Maihem.Attacks;
using Maihem.Extensions;
using Maihem.Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Maihem.Actors
{
    public class Player : Actor
    {
        private enum PlayerControlState
        {
            Normal,
            Aiming,
        }
        
        [Header("Stat Settings")]
        [SerializeField] private int maxStamina;
        [SerializeField] private int moveCost;
        [SerializeField] private int diagonalMoveCost;
        [SerializeField] private int idleStaminaRecovery;
        
        [Header("Children References")]
        [SerializeField] private GameObject aimGrid;
        [SerializeField] private GameObject diagonalModeMarker;
        [SerializeField] private GameObject stickObject;

        [Header("Placeholder/Debug Stuff")] [SerializeField]
        private AttackStrategy[] attackStrategies;
        private int _currentAttack;
        
        public Consumable consumable;

        public event EventHandler OnStatusUpdate;

        public int MaxStamina => maxStamina;
        public int CurrentStamina { get; private set; }

        public AttackStrategy CurrentAttack => attackSystem.currentAttackStrategy;
        
        private PlayerControlState _controlState = PlayerControlState.Normal;
        private bool _inDiagonalMode;

        private bool _isPaused;

        protected override void OnAnimationEnd()
        {
            OnTurnCompleted();
        }

        private void Start()
        {
            ConnectInputs();
        }

        public override void Initialize()
        {
            base.Initialize();
            _isPaused = false;
            CurrentStamina = maxStamina;
            if(attackStrategies.Length > 0)
                attackSystem.currentAttackStrategy = attackStrategies[0];
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void ConnectInputs()
        {
            var playerInput = GameManager.Instance.PlayerInput;
            playerInput.AttackAction += Attack;
            playerInput.ToggleAimAction += ToggleAim;
            playerInput.ToggleDiagonalModeAction += ToggleDiagonalMode;
            playerInput.MoveAction += ProcessMoveInput;
            playerInput.AttackChangeAction += ChangeAttackStrategy;
            playerInput.ConsumableAction += UseConsumable;
        }

        private void OnDestroy()
        {
            var playerInput = GameManager.Instance.PlayerInput;
            playerInput.AttackAction -= Attack;
            playerInput.ToggleAimAction -= ToggleAim;
            playerInput.ToggleDiagonalModeAction -= ToggleDiagonalMode;
            playerInput.MoveAction -= ProcessMoveInput;
            playerInput.AttackChangeAction -= ChangeAttackStrategy;
            playerInput.ConsumableAction -= UseConsumable;
            attackSystem?.HideTargetMarkers();
        }

       

        private void ChangeAttackStrategy(object sender, SingleAxisEventArgs e)
        {
            if (_isPaused) return;
            _currentAttack += e.AxisValue;
            if (_currentAttack < 0) _currentAttack = attackStrategies.Length - 1;
            if (_currentAttack >= attackStrategies.Length) _currentAttack = 0;
            
            attackSystem.currentAttackStrategy = attackStrategies[_currentAttack];
            if (_controlState == PlayerControlState.Aiming)
            {
                attackSystem.HideTargetMarkers();
                attackSystem.ShowTargetMarkers(GridPosition, CurrentFacing.GetFacingVector(), true);
            }
            
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);

        }


        private void Attack(object sender, EventArgs e)
        {
            if (!GameManager.Instance.CanTakeTurn() || _isPaused) return;

            if (CurrentStamina <= 0)
            {
                RecoverStamina(idleStaminaRecovery);
                OnTurnCompleted();
                return;
            }

            if (!TryStaminaConsumingAction(attackSystem.currentAttackStrategy.StaminaCost)) return;
            
            attackSystem.Attack(GridPosition, CurrentFacing.GetFacingVector(), true);
            animator.SetTrigger(AnimatorAttack);
            StartAttackAnimation(GridPosition, CurrentFacing.GetFacingVector(), true);
        }

        private void ProcessMoveInput(object sender, EventArgs e)
        {
            var moveInput = GameManager.Instance.PlayerInput.BufferedMoveInput;
            if (!GameManager.Instance.CanTakeTurn() || !(moveInput.sqrMagnitude > 0f) || _isPaused) return;

            if (CurrentStamina <= 0)
            {
                RecoverStamina(idleStaminaRecovery);
                OnTurnCompleted();
                return;
            }
            
            if (_inDiagonalMode)
            {
                if (moveInput.x == 0 || moveInput.y == 0) return;
                moveInput.x = math.round(moveInput.x);
                moveInput.y = math.round(moveInput.y);
            }
            
            UpdateFacing(moveInput);

            switch (_controlState)
            {
                case PlayerControlState.Normal:
                    TryMove(moveInput);
                    break;
                case PlayerControlState.Aiming:
                    UpdateAimMarker();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void UpdateFacing(Vector2 newFacingVector)
        {
            UpdateFacing(new Vector2Int((int)newFacingVector.x, (int)newFacingVector.y));
        }

        private bool TryMove(Vector2 moveInput)
        {
            var isDiagonal = moveInput.x != 0 && moveInput.y != 0;
            var cost = isDiagonal ? diagonalMoveCost : moveCost;
            if (CurrentStamina < cost) return false;
            
            var targetPosition = transform.position + moveInput.WithZ(0f);
            var targetGridPosition = MapManager.Instance.WorldToCell(targetPosition);
            
            if (MapManager.Instance.IsCellBlocking(targetPosition) ||
                GameManager.Instance.CellContainsActor(targetGridPosition)) return false;


            // Prevent corner cutting
            if (isDiagonal)
            {
                var yNeighbor = transform.position + new Vector3(0,moveInput.y,0);
                var xNeighbor = transform.position + new Vector3(moveInput.x,0,0);
                if (MapManager.Instance.IsCellBlocking(yNeighbor) || MapManager.Instance.IsCellBlocking(xNeighbor))
                {
                    return false;
                }
            }

            TryStaminaConsumingAction(cost); 
            animator.SetTrigger(AnimatorMove);
            StartMoveAnimation(targetPosition);
            UpdateGridPosition(targetPosition);

            return true;
        }

        private void UpdateFacing(Vector2Int newFacingVector)
        {
            var newFacing = CurrentFacing.GetFacingFromDirection(newFacingVector);
            if (CurrentFacing == newFacing) return;
            animator.SetInteger(AnimatorHorizontal, newFacingVector.x);
            animator.SetInteger(AnimatorVertical, newFacingVector.y);
            CurrentFacing = newFacing;
        }

        private void UpdateAimMarker()
        {
            attackSystem.UpdateTargetMarkerPositions(GridPosition, CurrentFacing.GetFacingVector(), true);
        }

        private void ToggleAim(object sender, ToggleEventArgs args)
        {
            if (args.ToggleValue)
            {
                if (_controlState != PlayerControlState.Normal) return;
                _controlState = PlayerControlState.Aiming;
                attackSystem.ShowTargetMarkers(GridPosition, CurrentFacing.GetFacingVector(), true);
                aimGrid.SetActive(true);
            }
            else
            {
                if (_controlState != PlayerControlState.Aiming) return;
                _controlState = PlayerControlState.Normal;
                attackSystem.HideTargetMarkers();
                aimGrid.SetActive(false);
            }
        }

        private void ToggleDiagonalMode(object sender, ToggleEventArgs args)
        {
            _inDiagonalMode = args.ToggleValue;
            diagonalModeMarker.SetActive(args.ToggleValue);
        }
        
        private void UseConsumable(object sender, EventArgs e)
        {
            if (!GameManager.Instance.CanTakeTurn() || _isPaused) return;

            if (!consumable) return;
            consumable.Activate();
            consumable = null;
            OnTurnCompleted();
        }
        

        private bool TryStaminaConsumingAction(int cost)
        {
            if (CurrentStamina < cost) return false;
            CurrentStamina -= cost;
            return true;
        }

        protected override void HealthChanged(object sender, HealthChangeEvent healthChangeEvent)
        {
            base.HealthChanged(sender, healthChangeEvent);
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);
        }


        public void RecoverStamina(int amount)
        {
            if (amount <= 0) return;
            CurrentStamina = math.clamp(CurrentStamina + amount, 0, MaxStamina);
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);
        }
        
        protected override void OnTurnCompleted()
        {
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);
            base.OnTurnCompleted();
        }


        public void PausePlayer()
        {
            _isPaused = true;
        }
    }
}