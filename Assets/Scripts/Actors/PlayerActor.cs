using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maihem.Attacks;
using Maihem.Extensions;
using Maihem.Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Actors
{
    public class PlayerActor : Actor
    {
        private enum PlayerControlState
        {
            Normal,
            Aiming,
            Diagonal
        }
        
        [Header("Stat Settings")]
        [SerializeField] private int maxStamina;
        [SerializeField] private int moveCost;
        [SerializeField] private int diagonalMoveCost;
        
        [Header("System References")]
        [SerializeField] private PlayerInput playerInput;
        
        [Header("Children References")]
        [SerializeField] private GameObject aimGrid;
        [SerializeField] private GameObject diagonalModeMarker;
        [SerializeField] private GameObject stickObject;

        [Header("Placeholder/Debug Stuff")] [SerializeField]
        private AttackStrategy[] attackStrategies;
        private int _currentAttack;

        public EventHandler OnStatusUpdate;

        public int MaxStamina => maxStamina;
        public int CurrentStamina { get; private set; }

        public AttackStrategy CurrentAttack => attackSystem.currentAttackStrategy;
        
        private PlayerControlState _controlState = PlayerControlState.Normal;
        private Animator _animator;
        
        private static readonly int AnimatorHorizontal = Animator.StringToHash("Horizontal");
        private static readonly int AnimatorVertical = Animator.StringToHash("Vertical");

        public override void TakeDamage(int damage)
        {
            if (IsDead) return;
            CurrentHealth -= damage;
            if (CurrentHealth <= 0) IsDead = true;
   
        }

        protected override void OnAnimationEnd()
        {
            EndTurn();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            ConnectInputs();
        }

        public override void Initialize()
        {
            base.Initialize();
            CurrentStamina = maxStamina;
            if(attackStrategies.Length > 0)
                attackSystem.currentAttackStrategy = attackStrategies[0];
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void ConnectInputs()
        {
            playerInput.OnAttackAction += Attack;
            playerInput.OnToggleAimAction += ToggleAim;
            playerInput.OnToggleDiagonalModeAction += ToggleDiagonalMode;
            playerInput.OnMoveAction += ProcessMoveInput;
            playerInput.OnAttackChangeAction += ChangeAttackStrategy;
        }

        private void OnDestroy()
        {
            playerInput.OnAttackAction -= Attack;
            playerInput.OnToggleAimAction -= ToggleAim;
            playerInput.OnToggleDiagonalModeAction -= ToggleDiagonalMode;
            playerInput.OnMoveAction -= ProcessMoveInput;
            playerInput.OnAttackChangeAction -= ChangeAttackStrategy;
            attackSystem?.HideTargetMarkers();
        }

        private void ChangeAttackStrategy(object sender, SingleAxisEventArgs e)
        {
            _currentAttack += e.AxisValue;
            if (_currentAttack < 0) _currentAttack = attackStrategies.Length - 1;
            if (_currentAttack >= attackStrategies.Length) _currentAttack = 0;
            attackSystem.currentAttackStrategy = attackStrategies[_currentAttack];
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);

        }


        private void Attack(object sender, EventArgs e)
        {
            if (!GameManager.Instance.CanTakeTurn()) return;

            if (attackSystem.currentAttackStrategy.StaminaCost > CurrentStamina) return;

            CurrentStamina -= attackSystem.currentAttackStrategy.StaminaCost;
            attackSystem.Attack(GridPosition, CurrentFacing.GetFacingVector(), true);
            StartAttackAnimation(GridPosition, CurrentFacing.GetFacingVector(), true);
        }

        private void ProcessMoveInput(object sender, EventArgs e)
        {
            var moveInput = playerInput.BufferedMoveInput;
            if (!GameManager.Instance.CanTakeTurn() || !(moveInput.sqrMagnitude > 0f)) return;

            var newFacing = new Vector2Int((int)moveInput.x, (int)moveInput.y);
            _animator.SetInteger(AnimatorHorizontal, newFacing.x);
            _animator.SetInteger(AnimatorVertical, newFacing.y);
            CurrentFacing = CurrentFacing.GetFacingFromDirection(newFacing);

            

            switch (_controlState)
            {
                case PlayerControlState.Normal:
                    ProcessMovement(moveInput);
                    break;
                case PlayerControlState.Aiming:
                    ProcessAim(moveInput);
                    break;
                case PlayerControlState.Diagonal:
                    ProcessDiagonalMovement(moveInput);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessDiagonalMovement(Vector2 moveInput)
        {
            if (moveInput.x == 0 || moveInput.y == 0) return;
            moveInput.x = math.round(moveInput.x);
            moveInput.y = math.round(moveInput.y);

            TryMove(moveInput);
        }

        private void ProcessMovement(Vector2 moveInput)
        {
            TryMove(moveInput);
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

            CurrentStamina -= cost;
            StartMoveAnimation(targetPosition);
            UpdateGridPosition(targetPosition);

            return true;
        }

        private void ProcessAim(Vector2 aimInput)
        {
            UpdateAimMarker();
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
            if (args.ToggleValue)
            {
                if (_controlState == PlayerControlState.Aiming) return;
                _controlState = PlayerControlState.Diagonal;
                diagonalModeMarker.SetActive(true);
            }
            else
            {
                if (_controlState != PlayerControlState.Diagonal) return;
                _controlState = PlayerControlState.Normal;
                diagonalModeMarker.SetActive(false);
            }
        }
       
        
        public void AdjustHealthAndStamina(int health, int stamina)
        {
            CurrentHealth = math.clamp(CurrentHealth + health, 0, MaxHealth);
            CurrentStamina = math.clamp(CurrentStamina + stamina, 0, MaxStamina);
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void EndTurn()
        {
            OnStatusUpdate?.Invoke(this, EventArgs.Empty);
            GameManager.Instance.TriggerTurn();
        }
        

        
    }
}