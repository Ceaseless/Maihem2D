using System;
using Maihem.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Maihem
{
    public class PlayerActor : Actor
    {
        private enum PlayerControlState { Normal, Aiming, Diagonal }
        
        [SerializeField] private float moveInputBufferWindow = 0.01f;
        
        
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private GameObject aimMarker;
        [SerializeField] private GameObject aimGrid;
        [SerializeField] private GameObject diagonalModeMarker;
        [SerializeField] private GameObject stickObject;
        
        private PlayerControlState _controlState = PlayerControlState.Normal;
        
        private Animator _animator;
        private InputAction _moveAction, _mousePosition, _toggleAimAction, _toggleDiagonalModeAction, _attackAction;
        
        private Vector2 _bufferedMoveInput;
        private float _lastMoveInput;
        
        private static readonly int AnimatorHorizontal = Animator.StringToHash("Horizontal");
        private static readonly int AnimatorVertical = Animator.StringToHash("Vertical");

        
        public override void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth > 0) return;
            Debug.Log("Player died");
        }
        
        protected override void OnMoveAnimationEnd()
        {
            GameManager.Instance.TriggerTurn();
        }

        protected override void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
            
            SetupInputActions();
        }
        
        private void SetupInputActions()
        {
            _moveAction = inputActions["Move"];
            _mousePosition = inputActions["Mouse Position"];
            _toggleAimAction = inputActions["Aim Toggle"];
            _toggleDiagonalModeAction = inputActions["Diagonal Toggle"];
            _attackAction = inputActions["Attack"];

            _moveAction.performed += MoveInputChanged;
            _moveAction.canceled += MoveInputChanged;
            
            _toggleAimAction.performed += StartAim;
            _toggleAimAction.canceled += EndAim;

            _toggleDiagonalModeAction.performed += StartDiagonalMode;
            _toggleDiagonalModeAction.canceled += EndDiagonalMode;
            
            _moveAction.Enable();
            _mousePosition.Enable();
            _toggleAimAction.Enable();
            _toggleDiagonalModeAction.Enable();
            _attackAction.Enable();
        }
       
        private void Update()
        {      
            ProcessInput();       
        }

        private void ProcessInput()
        {
            if (_attackAction.WasPerformedThisFrame())
            {
                Attack();
            } else if(Time.time - _lastMoveInput > moveInputBufferWindow) //if(_moveAction.IsPressed())
                ProcessMoveInput();
        }

        private void Attack()
        {
            if (!GameManager.Instance.CanTakeTurn()) return;
            var attackDirection = CurrentFacing.GetFacingVector();
            var targetCell = GridPosition + attackDirection;
            if (GameManager.Instance.TryGetActorOnCell(targetCell, out var target))
            {
                target.TakeDamage(1);
            }
            else
            {
                Debug.Log("Whiffed!");
            }
            GameManager.Instance.TriggerTurn();
        }
        
        private void ProcessMoveInput()
        {
            if (!GameManager.Instance.CanTakeTurn() || !(_bufferedMoveInput.sqrMagnitude > 0f)) return;
            
            var newFacing = new Vector2Int((int)_bufferedMoveInput.x, (int)_bufferedMoveInput.y);
            _animator.SetInteger(AnimatorHorizontal, newFacing.x);
            _animator.SetInteger(AnimatorVertical, newFacing.y);
            CurrentFacing = CurrentFacing.GetFacingFromDirection(newFacing);
          
            UpdateAimMarker(newFacing);

            switch (_controlState)
            {
                case PlayerControlState.Normal:
                    ProcessMovement(_bufferedMoveInput);
                    break;
                case PlayerControlState.Aiming:
                    ProcessAim(_bufferedMoveInput);
                    break;
                case PlayerControlState.Diagonal:
                    ProcessDiagonalMovement(_bufferedMoveInput);
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

            var newPosition = transform.position + moveInput.WithZ(0f);
            var newGridPosition = MapManager.Instance.GetGridPositionFromWorldPosition(newPosition);
            if (MapManager.Instance.IsCellBlocking(newPosition) || GameManager.Instance.CellContainsActor(newGridPosition)) return;
            
            StartMoveAnimation(newPosition);
            UpdateGridPosition(newPosition);
        }

        private void ProcessMovement(Vector2 moveInput)
        {
            var newPosition = transform.position + moveInput.WithZ(0f);
            var newGridPosition = MapManager.Instance.GetGridPositionFromWorldPosition(newPosition);
            
            if (MapManager.Instance.IsCellBlocking(newPosition) || GameManager.Instance.CellContainsActor(newGridPosition)) return;
            
            StartMoveAnimation(newPosition);
            UpdateGridPosition(newPosition);
            
        }

        private void ProcessAim(Vector2 aimInput)
        {
            
        }

        private void UpdateAimMarker(Vector2Int newFacing)
        {
            aimMarker.transform.localPosition = new Vector3(newFacing.x, newFacing.y, 0);
        }
        
        private void MoveInputChanged(InputAction.CallbackContext ctx)
        {
            _bufferedMoveInput = ctx.ReadValue<Vector2>();
            _lastMoveInput = Time.time;
        }

        private void StartAim(InputAction.CallbackContext ctx)
        {
            if (_controlState != PlayerControlState.Normal) return;
            _controlState = PlayerControlState.Aiming;
            aimMarker.SetActive(true);
            aimGrid.SetActive(true);
        }
        
        private void EndAim(InputAction.CallbackContext ctx)
        {
            if (_controlState != PlayerControlState.Aiming) return;
            _controlState = PlayerControlState.Normal;
            aimMarker.SetActive(false);
            aimGrid.SetActive(false);
        }

        private void StartDiagonalMode(InputAction.CallbackContext ctx)
        {
            if (_controlState == PlayerControlState.Aiming) return;
            _controlState = PlayerControlState.Diagonal;
            diagonalModeMarker.SetActive(true);
        }
        
        private void EndDiagonalMode(InputAction.CallbackContext ctx)
        {
            if (_controlState != PlayerControlState.Diagonal) return;
            _controlState = PlayerControlState.Normal;
            diagonalModeMarker.SetActive(false);
        }
   

  
    }
}
