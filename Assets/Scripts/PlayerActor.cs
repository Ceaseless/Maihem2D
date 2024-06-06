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
        
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private GameObject aimMarker;
        [SerializeField] private GameObject aimGrid;
        [SerializeField] private GameObject diagonalModeMarker;
        [SerializeField] private GameObject stickObject;
        
        private PlayerControlState _controlState = PlayerControlState.Normal;
        
        private Animator _animator;
        private InputAction _moveAction, _mousePosition, _toggleAimAction, _toggleDiagonalModeAction, _attackAction;
        
        
        private static readonly int AnimatorHorizontal = Animator.StringToHash("Horizontal");
        private static readonly int AnimatorVertical = Animator.StringToHash("Vertical");


        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
            
            SetupInputActions();
        }

       

        protected override void OnMoveAnimationEnd()
        {
            GameManager.Instance.TriggerTurn();
        }


        private void SetupInputActions()
        {
            _moveAction = inputActions["Move"];
            _mousePosition = inputActions["Mouse Position"];
            _toggleAimAction = inputActions["Aim Toggle"];
            _toggleDiagonalModeAction = inputActions["Diagonal Toggle"];
            _attackAction = inputActions["Attack"];

            _toggleAimAction.performed += ToggleAim;
            _toggleAimAction.canceled += EndAim;

            _toggleDiagonalModeAction.performed += ToggleDiagonalModeMode;
            _toggleDiagonalModeAction.canceled += EndDiagonalModeMode;
            
            _moveAction.Enable();
            _mousePosition.Enable();
            _toggleAimAction.Enable();
            _toggleDiagonalModeAction.Enable();
            _attackAction.Enable();
        }

        private void ToggleAim(InputAction.CallbackContext ctx)
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

        private void ToggleDiagonalModeMode(InputAction.CallbackContext ctx)
        {
            if (_controlState == PlayerControlState.Aiming) return;
            _controlState = PlayerControlState.Diagonal;
            diagonalModeMarker.SetActive(true);
        }
        
        private void EndDiagonalModeMode(InputAction.CallbackContext ctx)
        {
            if (_controlState != PlayerControlState.Diagonal) return;
            _controlState = PlayerControlState.Normal;
            diagonalModeMarker.SetActive(false);
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
            } else if(_moveAction.IsPressed())
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
            var moveInput = _moveAction.ReadValue<Vector2>();
            if (!GameManager.Instance.CanTakeTurn() || !(moveInput.sqrMagnitude > 0f)) return;
            
            var newFacing = new Vector2Int((int)moveInput.x, (int)moveInput.y);
            _animator.SetInteger(AnimatorHorizontal, newFacing.x);
            _animator.SetInteger(AnimatorVertical, newFacing.y);
            CurrentFacing = CurrentFacing.GetFacingFromDirection(newFacing);
          
            UpdateAimMarker(newFacing);

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
        
        public override void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth > 0) return;
            Debug.Log("Player died");
        }
   

  
    }
}
