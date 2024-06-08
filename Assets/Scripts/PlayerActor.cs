using System;
using Maihem.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem
{
    public class PlayerActor : Actor
    {
        private enum PlayerControlState
        {
            Normal,
            Aiming,
            Diagonal
        }

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private int maxStamina;
        [SerializeField] private int moveCost;
        [SerializeField] private int diagonalMoveCost;
        [SerializeField] private GameObject aimMarker;
        [SerializeField] private GameObject aimGrid;
        [SerializeField] private GameObject diagonalModeMarker;
        [SerializeField] private GameObject stickObject;

        private PlayerControlState _controlState = PlayerControlState.Normal;

        private Animator _animator;
        private int _stamina;

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

            _stamina = maxStamina;
            
            playerInput.OnAttackAction += Attack;
            playerInput.OnToggleAimAction += ToggleAim;
            playerInput.OnToggleDiagonalModeAction += ToggleDiagonalMode;
            playerInput.OnMoveAction += ProcessMoveInput;
        }


        private void Attack(object sender, EventArgs e)
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

        private void ProcessMoveInput(object sender, EventArgs e)
        {
            var moveInput = playerInput.BufferedMoveInput;
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
            TryMove(newPosition, diagonalMoveCost);
        }

        private void ProcessMovement(Vector2 moveInput)
        {
            var newPosition = transform.position + moveInput.WithZ(0f);
            var cost = (moveInput.x != 0 && moveInput.y != 0) ? diagonalMoveCost : moveCost;
            TryMove(newPosition, cost);
        }

        private bool TryMove(Vector3 targetPosition, int cost)
        {
            if (_stamina < cost) return false;
            var targetGridPosition = MapManager.Instance.WorldToCell(targetPosition);
            
            if (MapManager.Instance.IsCellBlocking(targetPosition) ||
                GameManager.Instance.CellContainsActor(targetGridPosition)) return false;

            _stamina -= cost;
            StartMoveAnimation(targetPosition);
            UpdateGridPosition(targetPosition);

            return true;
        }

        private void ProcessAim(Vector2 aimInput)
        {
        }

        private void UpdateAimMarker(Vector2Int newFacing)
        {
            aimMarker.transform.localPosition = new Vector3(newFacing.x, newFacing.y, 0);
        }

        private void ToggleAim(object sender, ToggleEventArgs args)
        {
            if (args.NewValue)
            {
                if (_controlState != PlayerControlState.Normal) return;
                _controlState = PlayerControlState.Aiming;
                aimMarker.SetActive(true);
                aimGrid.SetActive(true);
            }
            else
            {
                if (_controlState != PlayerControlState.Aiming) return;
                _controlState = PlayerControlState.Normal;
                aimMarker.SetActive(false);
                aimGrid.SetActive(false);
            }
        }

        private void ToggleDiagonalMode(object sender, ToggleEventArgs args)
        {
            if (args.NewValue)
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
    }
}