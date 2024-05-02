using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public bool launchTutorial;

    [Header("References")]
    public Animator tutorialAnim;

    PauseManager pauseManager;
    TilePlacementManager tilePlacementManager;
    CameraController cameraController;

    public Slider moveSlider, rotateSlider, zoomSlider;
    public float moveTarget, rotateTarget, zoomTarget;

    bool canFillHand = true;

    private void Awake()
    {
        pauseManager = FindFirstObjectByType<PauseManager>();
        tilePlacementManager = FindFirstObjectByType<TilePlacementManager>();
        cameraController = FindFirstObjectByType<CameraController>();
    }

    private void Start()
    {
        if (launchTutorial)
        {
            moveSlider.value = 0;
            rotateSlider.value = 0;
            zoomSlider.value = 0;

            moveSlider.maxValue = moveTarget;
            rotateSlider.maxValue = rotateTarget;
            zoomSlider.maxValue = zoomTarget;

            cameraController.canMove = false;
            cameraController.canRotate = false;
            cameraController.canZoom = false;
            cameraController.canReset = false;

            StartCoroutine(StartTutorial());
        }
        else
        {
            tilePlacementManager.OnStart();
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!launchTutorial) return;

        moveSlider.value = cameraController.tutorialMove;
        rotateSlider.value = cameraController.tutorialRotate;
        zoomSlider.value = cameraController.tutorialZoom;

        if (cameraController.tutorialMove >= moveTarget) StartCoroutine(OnMove());
        if (cameraController.tutorialRotate >= rotateTarget) StartCoroutine(OnRotate());
        if (cameraController.tutorialZoom >= zoomTarget) StartCoroutine(OnZoom());
        if(cameraController.tutorialReset && canFillHand) StartCoroutine(OnCameraReset());
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1);
        tutorialAnim.SetTrigger("Move");
        cameraController.canMove = true;
    }

    IEnumerator OnMove()
    {
        yield return new WaitForSeconds(.5f);

        tutorialAnim.SetTrigger("Rotate");
        cameraController.canRotate = true;
    }
    IEnumerator OnRotate()
    {
        yield return new WaitForSeconds(.5f);

        tutorialAnim.SetTrigger("Zoom");
        cameraController.canZoom = true;
    }
    IEnumerator OnZoom()
    {
        yield return new WaitForSeconds(.5f);
        tutorialAnim.SetTrigger("CameraReset");
        cameraController.canReset = true;
    }
    IEnumerator OnCameraReset()
    {
        canFillHand = false;
        cameraController.enabled = false;
        yield return new WaitForSeconds(.5f);
        tutorialAnim.SetTrigger("Inventory");
        tilePlacementManager.OnStart();
    }
    public void OnInventoryAgree()
    {
        cameraController.enabled = true;
        tutorialAnim.SetTrigger("InventoryAgree");
        print("check");
    }

    void FinishTutorial()
    {
        enabled = false;
    }
}