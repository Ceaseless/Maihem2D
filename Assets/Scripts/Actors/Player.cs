using System;
using System.Collections.Generic;
using Maihem.Attacks;
using Maihem.Extensions;
using Maihem.Managers;
using Maihem.Pickups;
using Unity.Mathematics;
using UnityEngine;

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
        public Consumable currentConsumable;
        
        [Header("Children References")]
        [SerializeField] private GameObject aimGrid;
        [SerializeField] private GameObject diagonalModeMarker;
       
        [Header("Placeholder/Debug Stuff")] 
        [SerializeField] private AttackStrategy[] startAttacks;

        [SerializeField] private AudioClip selectSoundFX;
        private int _currentAttack;
        private int _currentAttackAnimId;
        private Consumable _emptyConsumable;
        
        

        public event EventHandler OnStatusUpdate;

        public int MaxStamina => maxStamina;
        public int CurrentStamina { get; private set; }

        public AttackStrategy CurrentAttack => attackSystem.currentAttackStrategy;
        

        private static readonly int AnimatorKick = Animator.StringToHash("Kick");
        private static readonly int AnimatorSlam = Animator.StringToHash("Slam");
        private static readonly int AnimatorStomp = Animator.StringToHash("Stomp");

        private List<AttackStrategy> _availableAttacks;
        
        private PlayerControlState _controlState = PlayerControlState.Normal;
        private bool _inDiagonalMode;

        private bool _isPaused;

        protected override void OnAnimationEnd()
        {
            IsPerformingAction = false;
            OnTurnCompleted();
        }

        private void Start()
        {
            ConnectInputs();
        }

        public override void Initialize()
        {
            base.Initialize();
            _emptyConsumable = currentConsumable;
            _isPaused = false;
            _availableAttacks = new List<AttackStrategy>();
            CurrentStamina = maxStamina;
            if (startAttacks.Length > 0)
            {
                foreach (var attack in startAttacks)
                {
                    _availableAttacks.Add(attack);
                }
                attackSystem.currentAttackStrategy = _availableAttacks[0];
                UpdateAttackAnimationId();
            }
            else
            {
                Debug.Log("No start attacks set");
            }
                

            
            
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
            _availableAttacks.Clear();
        }

        private void ChangeAttackStrategy(object sender, SingleAxisEventArgs e)
        {
            if (_isPaused) return;
            _currentAttack += e.AxisValue;
            if (_currentAttack < 0) _currentAttack = _availableAttacks.Count - 1;
            if (_currentAttack >= _availableAttacks.Count) _currentAttack = 0;
            
            attackSystem.currentAttackStrategy = _availableAttacks[_currentAttack];
            if (_controlState == PlayerControlState.Aiming)
            {
                attackSystem.HideTargetMarkers();
                attackSystem.ShowTargetMarkers();
            }

            AudioManager.Instance.PlaySoundFX(selectSoundFX, transform.position, 1f);
            UpdateAttackAnimationId();
            

            OnStatusUpdate?.Invoke(this, EventArgs.Empty);

        }

        public void AddAttackStrategy(AttackStrategy newAttack)
        {
            if (_availableAttacks.Contains(newAttack)) return;
            _availableAttacks.Add(newAttack);
        }

        private void UpdateAttackAnimationId()
        {
            _currentAttackAnimId = attackSystem.currentAttackStrategy.DisplayName switch
            {
                "Kick" => AnimatorKick,
                "Slam" => AnimatorSlam,
                "Stomp" => AnimatorStomp,
                _ => AnimatorKick
            };
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
            
            //attackSystem.PerformAttack();
            IsPerformingAction = true;
            animator.SetTrigger(_currentAttackAnimId);
            //StartAttackAnimation();
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
            attackSystem.UpdateTargetMarkerPositions();
        }

        private void ToggleAim(object sender, ToggleEventArgs args)
        {
            if (args.ToggleValue)
            {
                if (_controlState != PlayerControlState.Normal) return;
                _controlState = PlayerControlState.Aiming;
                attackSystem.ShowTargetMarkers();
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

            
            switch (currentConsumable.type)
            {
                case ConsumableType.Health:
                    healthSystem.RecoverHealth(10);
                    break;
                case ConsumableType.Shield:
                    healthSystem.AddShield(3);
                    break;
                case ConsumableType.Empty:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            currentConsumable.PlayActivateEffects(transform.position);
            currentConsumable = _emptyConsumable;
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