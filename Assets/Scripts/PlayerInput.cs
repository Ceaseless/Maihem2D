using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Maihem
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private float moveInputBufferWindow = 0.05f;

        public event EventHandler OnMoveAction;
        public event EventHandler OnAttackAction;
        public event EventHandler<SingleAxisEventArgs> OnAttackChangeAction;
        public event EventHandler<ToggleEventArgs> OnToggleAimAction;
        public event EventHandler<ToggleEventArgs> OnToggleDiagonalModeAction;
        public event EventHandler<ToggleEventArgs> OnToggleEnemyMarkersAction;
        
        public Vector2 BufferedMoveInput { get; private set; }
        private float _lastMoveInput;
        private InputAction _moveAction, _mousePosition, _toggleAimAction, _toggleDiagonalModeAction, _attackAction;
        private InputAction _changeAttack;
        private InputAction _enemyMarkerToggle;

        private void Awake()
        {
            SetupInputActions();
        }

        private void SetupInputActions()
        {
            _moveAction = inputActions["Move"];
            _mousePosition = inputActions["Mouse Position"];
            _toggleAimAction = inputActions["Aim Toggle"];
            _toggleDiagonalModeAction = inputActions["Diagonal Toggle"];
            _attackAction = inputActions["Attack"];
            _changeAttack = inputActions["Attack Scroll"];
            _enemyMarkerToggle = inputActions["Enemy Toggle"];

            _moveAction.performed += MoveInputChanged;
            _moveAction.canceled += MoveInputChanged;

            _toggleAimAction.performed += ToggleAim;
            _toggleAimAction.canceled += ToggleAim;

            _toggleDiagonalModeAction.performed += ToggleDiagonalMode;
            _toggleDiagonalModeAction.canceled += ToggleDiagonalMode;

            _enemyMarkerToggle.performed += ToggleEnemyMarkers;
            _enemyMarkerToggle.canceled += ToggleEnemyMarkers;

            _attackAction.performed += _ => OnAttackAction?.Invoke(this, EventArgs.Empty);

            _changeAttack.performed += AttackChanged;
            
            _moveAction.Enable();
            _mousePosition.Enable();
            _toggleAimAction.Enable();
            _toggleDiagonalModeAction.Enable();
            _attackAction.Enable();
            _changeAttack.Enable();
            _enemyMarkerToggle.Enable();
        }

        private void Update()
        {
            if (BufferedMoveInput.sqrMagnitude > 0 && Time.time - _lastMoveInput > moveInputBufferWindow)
            {
                OnMoveAction?.Invoke(this, EventArgs.Empty);
            }
                
        }

        private void ToggleAim(InputAction.CallbackContext ctx)
        {
            switch (ctx.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    OnToggleAimAction?.Invoke(this, new ToggleEventArgs() { ToggleValue = true });
                    break;
                case InputActionPhase.Canceled:
                    OnToggleAimAction?.Invoke(this, new ToggleEventArgs() { ToggleValue = false });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ToggleDiagonalMode(InputAction.CallbackContext ctx)
        {
            switch (ctx.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    OnToggleDiagonalModeAction?.Invoke(this, new ToggleEventArgs() { ToggleValue = true });
                    break;
                case InputActionPhase.Canceled:
                    OnToggleDiagonalModeAction?.Invoke(this, new ToggleEventArgs() { ToggleValue = false });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private void ToggleEnemyMarkers(InputAction.CallbackContext ctx)
        {
            switch (ctx.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    OnToggleEnemyMarkersAction?.Invoke(this, new ToggleEventArgs() { ToggleValue = true });
                    break;
                case InputActionPhase.Canceled:
                    OnToggleEnemyMarkersAction?.Invoke(this, new ToggleEventArgs() { ToggleValue = false });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveInputChanged(InputAction.CallbackContext ctx)
        {
            BufferedMoveInput = ctx.ReadValue<Vector2>();
            _lastMoveInput = Time.time;
        }

        private void AttackChanged(InputAction.CallbackContext ctx)
        {
            var axisValue = Mathf.RoundToInt(ctx.ReadValue<float>());
            OnAttackChangeAction?.Invoke(this, new SingleAxisEventArgs { AxisValue = axisValue });
        }
        
    }
    
    public class ToggleEventArgs : EventArgs
    {
        public bool ToggleValue { get; set; }
    }

    public class SingleAxisEventArgs : EventArgs
    {
        private int _axisValue;
        public int AxisValue
        {
            get => _axisValue;
            set => _axisValue = math.clamp(value, -1,1);
        }
    }
}