using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private GameObject[] panels;
    private Vector3[] panelsPosStart;
    private bool isPauseActive;

    CameraController cameraController;
    [SerializeField] GameStateManager gameStateManager;

    private void Awake() => cameraController = FindFirstObjectByType<CameraController>();

    private void Start()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        panelsPosStart= new Vector3[panels.Length];
        for(int i = 0; i < panels.Length; i++)
        {
            panelsPosStart[i] = panels[i].transform.localPosition;
        }
    }

    private void SwitchPanel()
    {
        if (isPauseActive)
        {
            //panels[0].transform.DOLocalMoveY().OnComplete();
        }
        else
        {
           
        }
        panels[0].SetActive(isPauseActive?false:true);
        panels[1].SetActive(isPauseActive?true:false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchStateGame(gameStateManager.gameState== GameState.Paused);
        }
    }

    private void SwitchStateGame(bool isPause)
    {
        if (isPause) { gameStateManager.ResumeGameState(); }
        else { gameStateManager.PauseGameState(); }
        isPauseActive = !isPause;
        SwitchPanel();
    }

    private void BackToMenu()
    {

    }

    private void Quit() { }

}
