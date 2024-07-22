//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Inputs/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""930b8fa0-bd1b-46bc-8db6-995ea9e67277"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""8dde593c-1a13-4096-b596-b1bcb2100163"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Mouse Position"",
                    ""type"": ""Value"",
                    ""id"": ""1e4de75d-17ae-46a1-8baa-b5142a633263"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""59115ef7-07ce-420a-9854-d5c2743b5051"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack Scroll"",
                    ""type"": ""Button"",
                    ""id"": ""9d93b930-a77d-43d5-8c33-20e9f34e7c9e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""f2052fe5-a912-4944-b556-73acfd8a72ca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Diagonal Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""a4dcb784-1baf-4209-83d5-f9c15aa2757d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Enemy Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""ce447dc2-9f64-42ee-9ffe-4728c200cc6e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Use Consumable"",
                    ""type"": ""Button"",
                    ""id"": ""458421ad-5ee7-4d5b-8614-92cf9c449fcc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause Game"",
                    ""type"": ""Button"",
                    ""id"": ""b16f9bf4-38e2-4e8c-ac38-72afa42d5e8e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""ffda9a11-27dc-4722-a206-8783f256706d"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""6d5dad98-ca0f-4fad-aa08-56d9ca8f70be"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""e54e3dfb-6ff8-4459-bd84-1771e0111e11"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""2fe341e5-5259-4516-ba7b-8680fdf2e23d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""40452365-0323-4a3b-bf81-27bd15a21829"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""bbf88548-6780-4036-9f64-cecdf72e9938"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c6aa558d-e60a-49a3-902c-4418a4d9a335"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f2af6044-c74a-437d-b4c4-8388856c4669"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b4280da4-57db-46a7-8b55-6bab067335f7"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9b51c7d8-2704-4382-9f33-ad9df1e038e4"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c540f267-bade-40e8-b881-dcf1b691349f"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04818f98-761f-4e53-8d7e-8824d845092e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8233159-054b-4914-84c9-fea71ffd072d"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Diagonal Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e266e6a0-568f-499e-9413-2f61950b83d4"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""de7a81b7-5a32-46e3-959a-6e0d5f18b1f6"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack Scroll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""1e8fbf0c-efec-4e9a-a0d2-200d7286dad4"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""66963945-2d82-4622-9a77-d33fb613374c"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e7348196-7415-4a93-be74-3f9b5ef1d8a7"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Enemy Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e03b2f61-d031-4caa-8851-46b931910ff0"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use Consumable"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9c1c047-c291-46b4-bc5a-32d2a8b7d511"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause Game"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_Move = m_Default.FindAction("Move", throwIfNotFound: true);
        m_Default_MousePosition = m_Default.FindAction("Mouse Position", throwIfNotFound: true);
        m_Default_Attack = m_Default.FindAction("Attack", throwIfNotFound: true);
        m_Default_AttackScroll = m_Default.FindAction("Attack Scroll", throwIfNotFound: true);
        m_Default_AimToggle = m_Default.FindAction("Aim Toggle", throwIfNotFound: true);
        m_Default_DiagonalToggle = m_Default.FindAction("Diagonal Toggle", throwIfNotFound: true);
        m_Default_EnemyToggle = m_Default.FindAction("Enemy Toggle", throwIfNotFound: true);
        m_Default_UseConsumable = m_Default.FindAction("Use Consumable", throwIfNotFound: true);
        m_Default_PauseGame = m_Default.FindAction("Pause Game", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Default
    private readonly InputActionMap m_Default;
    private List<IDefaultActions> m_DefaultActionsCallbackInterfaces = new List<IDefaultActions>();
    private readonly InputAction m_Default_Move;
    private readonly InputAction m_Default_MousePosition;
    private readonly InputAction m_Default_Attack;
    private readonly InputAction m_Default_AttackScroll;
    private readonly InputAction m_Default_AimToggle;
    private readonly InputAction m_Default_DiagonalToggle;
    private readonly InputAction m_Default_EnemyToggle;
    private readonly InputAction m_Default_UseConsumable;
    private readonly InputAction m_Default_PauseGame;
    public struct DefaultActions
    {
        private @Controls m_Wrapper;
        public DefaultActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Default_Move;
        public InputAction @MousePosition => m_Wrapper.m_Default_MousePosition;
        public InputAction @Attack => m_Wrapper.m_Default_Attack;
        public InputAction @AttackScroll => m_Wrapper.m_Default_AttackScroll;
        public InputAction @AimToggle => m_Wrapper.m_Default_AimToggle;
        public InputAction @DiagonalToggle => m_Wrapper.m_Default_DiagonalToggle;
        public InputAction @EnemyToggle => m_Wrapper.m_Default_EnemyToggle;
        public InputAction @UseConsumable => m_Wrapper.m_Default_UseConsumable;
        public InputAction @PauseGame => m_Wrapper.m_Default_PauseGame;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void AddCallbacks(IDefaultActions instance)
        {
            if (instance == null || m_Wrapper.m_DefaultActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DefaultActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
            @Attack.started += instance.OnAttack;
            @Attack.performed += instance.OnAttack;
            @Attack.canceled += instance.OnAttack;
            @AttackScroll.started += instance.OnAttackScroll;
            @AttackScroll.performed += instance.OnAttackScroll;
            @AttackScroll.canceled += instance.OnAttackScroll;
            @AimToggle.started += instance.OnAimToggle;
            @AimToggle.performed += instance.OnAimToggle;
            @AimToggle.canceled += instance.OnAimToggle;
            @DiagonalToggle.started += instance.OnDiagonalToggle;
            @DiagonalToggle.performed += instance.OnDiagonalToggle;
            @DiagonalToggle.canceled += instance.OnDiagonalToggle;
            @EnemyToggle.started += instance.OnEnemyToggle;
            @EnemyToggle.performed += instance.OnEnemyToggle;
            @EnemyToggle.canceled += instance.OnEnemyToggle;
            @UseConsumable.started += instance.OnUseConsumable;
            @UseConsumable.performed += instance.OnUseConsumable;
            @UseConsumable.canceled += instance.OnUseConsumable;
            @PauseGame.started += instance.OnPauseGame;
            @PauseGame.performed += instance.OnPauseGame;
            @PauseGame.canceled += instance.OnPauseGame;
        }

        private void UnregisterCallbacks(IDefaultActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
            @Attack.started -= instance.OnAttack;
            @Attack.performed -= instance.OnAttack;
            @Attack.canceled -= instance.OnAttack;
            @AttackScroll.started -= instance.OnAttackScroll;
            @AttackScroll.performed -= instance.OnAttackScroll;
            @AttackScroll.canceled -= instance.OnAttackScroll;
            @AimToggle.started -= instance.OnAimToggle;
            @AimToggle.performed -= instance.OnAimToggle;
            @AimToggle.canceled -= instance.OnAimToggle;
            @DiagonalToggle.started -= instance.OnDiagonalToggle;
            @DiagonalToggle.performed -= instance.OnDiagonalToggle;
            @DiagonalToggle.canceled -= instance.OnDiagonalToggle;
            @EnemyToggle.started -= instance.OnEnemyToggle;
            @EnemyToggle.performed -= instance.OnEnemyToggle;
            @EnemyToggle.canceled -= instance.OnEnemyToggle;
            @UseConsumable.started -= instance.OnUseConsumable;
            @UseConsumable.performed -= instance.OnUseConsumable;
            @UseConsumable.canceled -= instance.OnUseConsumable;
            @PauseGame.started -= instance.OnPauseGame;
            @PauseGame.performed -= instance.OnPauseGame;
            @PauseGame.canceled -= instance.OnPauseGame;
        }

        public void RemoveCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDefaultActions instance)
        {
            foreach (var item in m_Wrapper.m_DefaultActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DefaultActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DefaultActions @Default => new DefaultActions(this);
    public interface IDefaultActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnAttackScroll(InputAction.CallbackContext context);
        void OnAimToggle(InputAction.CallbackContext context);
        void OnDiagonalToggle(InputAction.CallbackContext context);
        void OnEnemyToggle(InputAction.CallbackContext context);
        void OnUseConsumable(InputAction.CallbackContext context);
        void OnPauseGame(InputAction.CallbackContext context);
    }
}
