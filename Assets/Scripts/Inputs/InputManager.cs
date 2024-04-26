using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerControls playerControls;

    [Header("Movement")]
    // Right Mouse
    public bool rightMouseInput;
    public bool rightMouseDownInput;
    public bool rightMouseUpInput;

    // Left Mouse
    public bool leftMouseInput;
    public bool leftMouseDownInput;
    public bool leftMouseUpInput;

    // Middle Mouse
    public bool middleMouseInput;

    // Mouse Pos & Scroll
    public Vector2 mousePosInput;
    public Vector2 mouseScrollInput;

    //Movement
    InputAction rightMouseAction;
    InputAction leftMouseAction;
    InputAction mousePosAction;
    InputAction mouseScrollAction;
    InputAction middleMouseAction;

    private void Awake()
    {
        playerControls = new PlayerControls();

        //Movement
        rightMouseAction = playerControls.CameraControls.RightMouse;
        leftMouseAction = playerControls.CameraControls.LeftMouse;
        mousePosAction = playerControls.CameraControls.MousePosition;
        mouseScrollAction = playerControls.CameraControls.MouseScrollDelta;
        middleMouseAction = playerControls.CameraControls.MiddleMouse;
    }

    private void Update()
    {
        // Movement
        // Right
        rightMouseInput = rightMouseAction.IsPressed();
        rightMouseDownInput = rightMouseAction.WasPressedThisFrame();
        rightMouseUpInput = rightMouseAction.WasReleasedThisFrame();
        // Left
        leftMouseInput = leftMouseAction.IsPressed();
        leftMouseDownInput = leftMouseAction.WasPressedThisFrame();
        leftMouseUpInput = leftMouseAction.WasReleasedThisFrame();
        // Other
        middleMouseInput = middleMouseAction.WasPressedThisFrame();
        mousePosInput = mousePosAction.ReadValue<Vector2>();
        mouseScrollInput = mouseScrollAction.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        //Movement
        rightMouseAction.Enable();
        leftMouseAction.Enable();
        mousePosAction.Enable();
        mouseScrollAction.Enable();
        middleMouseAction.Enable();
    }

    private void OnDisable()
    {
        //Movement
        rightMouseAction.Disable();
        leftMouseAction.Disable();
        mousePosAction.Disable();
        mouseScrollAction.Disable();
        middleMouseAction.Disable();
    }
}