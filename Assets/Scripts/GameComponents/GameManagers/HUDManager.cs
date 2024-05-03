using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject[] panels;
    private Vector3[] panelsPosStart;
    [SerializeField] private float timeAnimation;
    private bool isPauseActive;
    
    private bool canInput = true;

    [Header("Settings Panel")]
    public GameObject settingsPanel;
    private bool isSettingsOpen;
    private float posSettingsPanel;


    [Header("References")]
    CameraController cameraController;
    [SerializeField] GameStateManager gameStateManager;

    private void Awake() => cameraController = FindFirstObjectByType<CameraController>();

    private void Start()
    {
        posSettingsPanel = settingsPanel.transform.localPosition.x;
        cameraController = FindFirstObjectByType<CameraController>();
        panelsPosStart= new Vector3[panels.Length];
        for(int i = 0; i < panels.Length; i++)
        {
            panelsPosStart[i] = panels[i].transform.localPosition;
            panels[i].SetActive(i == 0);
        }
        panels[isPauseActive ? 1 : 0].transform.localPosition = Vector3.zero;
    }

    private void SwitchPanel()
    {
        var goHidden = isPauseActive ? 0 : 1;
        var goShowed = isPauseActive ? 1 : 0;

        canInput = false;

        panels[goHidden].transform.DOLocalMoveY(panelsPosStart[goHidden].y, timeAnimation*1.3f).OnComplete(()=>
        {
            panels[goHidden].SetActive(false);
            canInput = true;
            
        });
        panels[goShowed].SetActive(true);
        panels[goShowed].transform.DOLocalMoveY(0, timeAnimation);
        if (!isPauseActive && isSettingsOpen) SettingButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canInput)
        {
            ShowMenu();
        }
    }

    public void ShowMenu()
    {
        SwitchStateGame(gameStateManager.gameState == GameState.Paused);
    }

    public void SettingButton()
    {
        isSettingsOpen = !isSettingsOpen;
        if (!isSettingsOpen)
        {
            settingsPanel.transform.DOLocalMoveX(posSettingsPanel, 0.5f).OnComplete(() => settingsPanel.SetActive(isSettingsOpen));
        }
        else
        {
            settingsPanel.SetActive(isSettingsOpen);
            settingsPanel.transform.DOLocalMoveX(0f, 0.5f);
        }

    }

    private void SwitchStateGame(bool isPause)
    {
        if (isPause) { gameStateManager.ResumeGameState(); }
        else { gameStateManager.PauseGameState(); }
        isPauseActive = !isPause;
        SwitchPanel();
    }

    public void BackToMenu() => SceneManager.LoadSceneAsync("MainMenu");

    public void Quit() => Application.Quit();

}
