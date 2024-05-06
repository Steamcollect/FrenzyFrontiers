using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject placeAllTilePanel;
    public GameObject survivNightPanel;
    public GameObject finalpanelPanel;

    TilePlacementManager tilePlacementManager;
    CameraController cameraController;

    public Slider moveSlider, rotateSlider, zoomSlider, tilePlacesCountSlider;
    [SerializeField] float moveTarget, rotateTarget, zoomTarget;

    int currentState = 0;

    public AudioClip popUpSound;

    public static TutorialManager instance;

    private void Awake()
    {
        tilePlacementManager = FindFirstObjectByType<TilePlacementManager>();
        cameraController = FindFirstObjectByType<CameraController>();

        instance = this;
    }

    private void Start()
    {
        if (launchTutorial)
        {
            movePanel.SetActive(false);
            rotatePanel.SetActive(false);
            zoomPanel.SetActive(false);
            centerPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            placeTilePanel.SetActive(false);
            placeAllTilePanel.SetActive(false);
            survivNightPanel.SetActive(false);
            finalpanelPanel.SetActive(false);

            moveSlider.value = 0;
            rotateSlider.value = 0;
            zoomSlider.value = 0;
            tilePlacesCountSlider.value = 0;

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
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!launchTutorial) return;

        moveSlider.value = cameraController.tutorialMove;
        rotateSlider.value = cameraController.tutorialRotate;
        zoomSlider.value = cameraController.tutorialZoom;

        if (cameraController.tutorialMove >= moveTarget && currentState == 0) OnMove();
        if (cameraController.tutorialRotate >= rotateTarget && currentState == 1) OnRotate();
        if (cameraController.tutorialZoom >= zoomTarget && currentState == 2) OnZoom();

        if (cameraController.tutorialReset && currentState == 3) StartCoroutine(OnCameraReset());
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1);


        PlayPopUpSound();
        movePanel.SetActive(true);
        movePanel.transform.Bump(1.08f);
        cameraController.canMove = true;
    }

    void OnMove()
    {
        currentState = 1;
        movePanel.SetActive(false);

        PlayPopUpSound();
        rotatePanel.SetActive(true);
        rotatePanel.transform.Bump(1.08f);
       
        cameraController.canRotate = true;
    }
    void OnRotate()
    {
        currentState = 2;

        rotatePanel.SetActive(false);

        PlayPopUpSound();
        zoomPanel.SetActive(true);
        zoomPanel.transform.Bump(1.08f);
        
        cameraController.canZoom = true;
    }
    void OnZoom()
    {
        currentState = 3;

        zoomPanel.SetActive(false);
        PlayPopUpSound();
        centerPanel.SetActive(true);
        centerPanel.transform.Bump(1.08f);
        
        cameraController.canReset = true;
    }
    IEnumerator OnCameraReset()
    {
        currentState = 4;
        centerPanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        cameraController.enabled = false;
        tilePlacementManager.OnStart();

        PlayPopUpSound();
        inventoryPanel.SetActive(true);
        inventoryPanel.transform.Bump(1.08f);
    }
    public void OnInventoryAgree()
    {
        inventoryPanel.SetActive(false);

        PlayPopUpSound();
        placeTilePanel.SetActive(true);
        placeTilePanel.transform.Bump(1.08f);
    }

    public void OnPlaceTileAgree()
    {
        tilePlacementManager.OnFillHand();

        placeTilePanel.SetActive(false);
        cameraController.enabled = true;

        PlayPopUpSound();
        placeAllTilePanel.SetActive(true);
        placeAllTilePanel.transform.Bump(1.08f);
    }
    public void OnAllTilesPlace()
    {
        placeAllTilePanel.SetActive(false);
        cameraController.enabled = false;

        PlayPopUpSound();
        survivNightPanel.SetActive(true);
        survivNightPanel.transform.Bump(1.08f);
    }
    public void OnSurvivNightAgree()
    {
        cameraController.enabled = true;

        survivNightPanel.SetActive(false);
        GameStateManager.instance.ChangePhaseToFight();
    }

    public IEnumerator OnNightEnd()
    {
        cameraController.enabled = false;

        yield return new WaitForSeconds(.8f);

        PlayPopUpSound();
        finalpanelPanel.SetActive(true);
        finalpanelPanel.transform.Bump(1.08f);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void EndTutorial()
    {
        launchTutorial = false;
        SceneManager.LoadScene("Game");
    }

    void PlayPopUpSound()
    {
        AudioManager.instance.PlayClipAt(popUpSound, 0, Vector2.zero);
    }
}