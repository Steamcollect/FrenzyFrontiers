//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/Inputs/PlayerInputs.inputactions
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

public partial class @PlayerInputs: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputs"",
    ""maps"": [
        {
            ""name"": ""CameraControls"",
            ""id"": ""bea650e6-fc4e-4db1-a61c-5b55c8ce82df"",
            ""actions"": [
                {
                    ""name"": ""RightMouse"",
                    ""type"": ""Button"",
                    ""id"": ""fe00d294-1d19-4856-9c0e-5f24c8d1b55d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftMouse"",
                    ""type"": ""Button"",
                    ""id"": ""cab23a19-1ac2-4ff2-8a43-6c190c8ec89e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""65c51022-a77d-42f6-b3d3-344775714f5c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseScrollDelta"",
                    ""type"": ""Value"",
                    ""id"": ""74a739dd-fed9-41be-9570-8ab5c8b7659c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MiddleMouse"",
                    ""type"": ""Button"",
                    ""id"": ""273eed82-2940-4a93-9e8d-bad4d2f34437"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""57d5e460-fcd3-4f19-8047-805c7a990110"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""215f4a46-bcaf-4d7a-bda3-89f688fb92f8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b023caa-35f2-4f3a-a774-740bfc07a123"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseScrollDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ebd733d8-378f-4c07-bcd9-56d54f7ba5be"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c8d4886b-e92f-4cc6-9e78-d4fc6343e364"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MiddleMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CameraControls
        m_CameraControls = asset.FindActionMap("CameraControls", throwIfNotFound: true);
        m_CameraControls_RightMouse = m_CameraControls.FindAction("RightMouse", throwIfNotFound: true);
        m_CameraControls_LeftMouse = m_CameraControls.FindAction("LeftMouse", throwIfNotFound: true);
        m_CameraControls_MousePosition = m_CameraControls.FindAction("MousePosition", throwIfNotFound: true);
        m_CameraControls_MouseScrollDelta = m_CameraControls.FindAction("MouseScrollDelta", throwIfNotFound: true);
        m_CameraControls_MiddleMouse = m_CameraControls.FindAction("MiddleMouse", throwIfNotFound: true);
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

    // CameraControls
    private readonly InputActionMap m_CameraControls;
    private List<ICameraControlsActions> m_CameraControlsActionsCallbackInterfaces = new List<ICameraControlsActions>();
    private readonly InputAction m_CameraControls_RightMouse;
    private readonly InputAction m_CameraControls_LeftMouse;
    private readonly InputAction m_CameraControls_MousePosition;
    private readonly InputAction m_CameraControls_MouseScrollDelta;
    private readonly InputAction m_CameraControls_MiddleMouse;
    public struct CameraControlsActions
    {
        private @PlayerInputs m_Wrapper;
        public CameraControlsActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @RightMouse => m_Wrapper.m_CameraControls_RightMouse;
        public InputAction @LeftMouse => m_Wrapper.m_CameraControls_LeftMouse;
        public InputAction @MousePosition => m_Wrapper.m_CameraControls_MousePosition;
        public InputAction @MouseScrollDelta => m_Wrapper.m_CameraControls_MouseScrollDelta;
        public InputAction @MiddleMouse => m_Wrapper.m_CameraControls_MiddleMouse;
        public InputActionMap Get() { return m_Wrapper.m_CameraControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraControlsActions set) { return set.Get(); }
        public void AddCallbacks(ICameraControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Add(instance);
            @RightMouse.started += instance.OnRightMouse;
            @RightMouse.performed += instance.OnRightMouse;
            @RightMouse.canceled += instance.OnRightMouse;
            @LeftMouse.started += instance.OnLeftMouse;
            @LeftMouse.performed += instance.OnLeftMouse;
            @LeftMouse.canceled += instance.OnLeftMouse;
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
            @MouseScrollDelta.started += instance.OnMouseScrollDelta;
            @MouseScrollDelta.performed += instance.OnMouseScrollDelta;
            @MouseScrollDelta.canceled += instance.OnMouseScrollDelta;
            @MiddleMouse.started += instance.OnMiddleMouse;
            @MiddleMouse.performed += instance.OnMiddleMouse;
            @MiddleMouse.canceled += instance.OnMiddleMouse;
        }

        private void UnregisterCallbacks(ICameraControlsActions instance)
        {
            @RightMouse.started -= instance.OnRightMouse;
            @RightMouse.performed -= instance.OnRightMouse;
            @RightMouse.canceled -= instance.OnRightMouse;
            @LeftMouse.started -= instance.OnLeftMouse;
            @LeftMouse.performed -= instance.OnLeftMouse;
            @LeftMouse.canceled -= instance.OnLeftMouse;
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
            @MouseScrollDelta.started -= instance.OnMouseScrollDelta;
            @MouseScrollDelta.performed -= instance.OnMouseScrollDelta;
            @MouseScrollDelta.canceled -= instance.OnMouseScrollDelta;
            @MiddleMouse.started -= instance.OnMiddleMouse;
            @MiddleMouse.performed -= instance.OnMiddleMouse;
            @MiddleMouse.canceled -= instance.OnMiddleMouse;
        }

        public void RemoveCallbacks(ICameraControlsActions instance)
        {
            if (m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICameraControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_CameraControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CameraControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CameraControlsActions @CameraControls => new CameraControlsActions(this);
    public interface ICameraControlsActions
    {
        void OnRightMouse(InputAction.CallbackContext context);
        void OnLeftMouse(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnMouseScrollDelta(InputAction.CallbackContext context);
        void OnMiddleMouse(InputAction.CallbackContext context);
    }
}
