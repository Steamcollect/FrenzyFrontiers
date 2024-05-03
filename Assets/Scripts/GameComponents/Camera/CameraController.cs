using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementTime;
    public Vector2 minMaxZoom;
    float currentZoomAmount = 1;
    public bool isMoving;
    public bool isRotating;

    //Position
    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;

    [HideInInspector] public bool canMove = true, canRotate = true, canZoom = true, canReset = true;
    [HideInInspector] public float tutorialMove, tutorialRotate, tutorialZoom;
    [HideInInspector] public bool tutorialReset = false;

    //Rotation
    Vector3 initialStartPosition;
    Vector3 rotateStartPosition;
    Vector3 rotateCurrentPosition;

    Vector3 newPosition;
    public Quaternion newRotation { get; set; }
    Vector3 newZoom;
    Vector3 zoomMultiplicator = new Vector3(0, -8, 11);

    Plane plane = new Plane(Vector3.up, Vector3.zero);

    Camera cam;
    Transform cameraTransform;
    InputManager inputManager;
    GameStateManager gameStateManager;

    private void Awake()
    {
        cam = Camera.main;
        cameraTransform = cam.transform;
        inputManager = FindFirstObjectByType<InputManager>();
        gameStateManager = FindFirstObjectByType<GameStateManager>();
    }
    private void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }
    private void Update()
    {
        if (gameStateManager.gameState == GameState.Gameplay)
        {
            if (canMove) MoveCamera();
            if (canRotate) RotateCamera();
            if (canZoom && inputManager.mouseScrollInput.y != 0) CameraZoom();

            newPosition.y = 0;

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
        }
    }

    void MoveCamera()
    {
        if (inputManager.leftMouseDownInput)
        {
            Ray ray = cam.ScreenPointToRay(inputManager.mousePosInput);

            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
                isMoving = false;
            }
        }
        if (inputManager.leftMouseInput)
        {
            Ray ray = cam.ScreenPointToRay(inputManager.mousePosInput);

            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPosition - dragCurrentPosition;

                float tmpDist = Vector3.Distance(dragCurrentPosition, dragStartPosition);
                tutorialMove += tmpDist / 10;
                if (tmpDist > 1 && !isMoving)
                {
                    isMoving = true;
                }
            }
        }
        if (canReset && inputManager.middleMouseInput)
        {
            newPosition = Vector3.zero;
            tutorialReset = true;
        }
    }

    void RotateCamera()
    {
        if (inputManager.rightMouseDownInput)
        {
            rotateStartPosition = inputManager.mousePosInput;
            initialStartPosition = rotateStartPosition;
            isRotating = false;
        }

        if (inputManager.rightMouseInput)
        {
            rotateCurrentPosition = inputManager.mousePosInput;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            tutorialRotate += difference.magnitude / 100;

            rotateStartPosition = rotateCurrentPosition;

            float tmpDist = Vector3.Distance(initialStartPosition, rotateCurrentPosition);
            if (tmpDist > 1 && !isRotating)
            {
                isRotating = true;
            }

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
        }
    }

    void CameraZoom()
    {
        float scrollValue = inputManager.mouseScrollInput.y / 10;

        currentZoomAmount -= scrollValue;
        tutorialZoom += Mathf.Abs(scrollValue);

        currentZoomAmount = currentZoomAmount <= minMaxZoom.x ? minMaxZoom.x : currentZoomAmount;
        currentZoomAmount = currentZoomAmount >= minMaxZoom.y ? minMaxZoom.y : currentZoomAmount;

        newZoom = -currentZoomAmount * zoomMultiplicator;
    }
}