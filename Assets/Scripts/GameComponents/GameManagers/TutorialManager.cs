using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public bool launchTutorial;

    [Header("References")]
    public GameObject movePanel;
    public GameObject rotatePanel;
    public GameObject zoomPanel;
    public GameObject centerPanel;
    public GameObject inventoryPanel;
    public GameObject placeTilePanel;

    PauseManager pauseManager;
    TilePlacementManager tilePlacementManager;
    CameraController cameraController;

    public Slider moveSlider, rotateSlider, zoomSlider;
    public float moveTarget, rotateTarget, zoomTarget;

    public AudioClip popUpSound;

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
            movePanel.SetActive(false);
            rotatePanel.SetActive(false);
            zoomPanel.SetActive(false);
            centerPanel.SetActive(false);

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

        AudioManager.instance.PlayClipAt(popUpSound, 0, Vector2.zero);
        movePanel.SetActive(true);
        movePanel.transform.Bump(1.08f);
        cameraController.canMove = true;
    }

    IEnumerator OnMove()
    {
        yield return new WaitForSeconds(.5f);

        movePanel.SetActive(false);
        
        rotatePanel.SetActive(true);
        rotatePanel.transform.Bump(1.08f);
       
        cameraController.canRotate = true;
    }
    IEnumerator OnRotate()
    {
        yield return new WaitForSeconds(.5f);

        rotatePanel.SetActive(false);
        
        zoomPanel.SetActive(true);
        zoomPanel.transform.Bump(1.08f);
        
        cameraController.canZoom = true;
    }
    IEnumerator OnZoom()
    {
        yield return new WaitForSeconds(.5f);
        
        zoomPanel.SetActive(false);
        
        centerPanel.SetActive(true);
        centerPanel.transform.Bump(1.08f);
        
        cameraController.canReset = true;
    }
    IEnumerator OnCameraReset()
    {
        canFillHand = false;
        cameraController.enabled = false;

        centerPanel.SetActive(false);

        tilePlacementManager.OnStart();

        yield return new WaitForSeconds(.3f);

        inventoryPanel.SetActive(true);
        inventoryPanel.transform.Bump(1.08f);
    }
    public void OnInventoryAgree()
    {
        //tutorialAnim.SetTrigger("InventoryAgree");
        cameraController.enabled = true;
        print("check");
    }

    public void OnPlaceTileAgree()
    {

    }

    void FinishTutorial()
    {
        gameObject.SetActive(false);
    }
}