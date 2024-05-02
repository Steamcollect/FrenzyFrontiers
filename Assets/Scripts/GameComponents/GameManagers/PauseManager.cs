using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanelGO;
    public GameObject settingsPanel;
    bool pauseResumeInput;

    public bool isOnMenu = false;

    CameraController cameraController;
    [SerializeField] GameStateManager gameStateManager;

    private void Awake()
    {
        cameraController = FindFirstObjectByType<CameraController>();
    }

    private void Start()
    {
        pausePanelGO.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (gameStateManager.gameState)
            {
                case GameState.Gameplay:
                    Pause();
                    break;
                case GameState.Paused:
                    Resume();
                    break;
            }
        }
    }

    public void Resume()
    {
        gameStateManager.ResumeGameState();

        isOnMenu = false;
        StartCoroutine(ClosePausePanel());
    }
    public void Pause()
    {
        gameStateManager.PauseGameState();

        isOnMenu = true;
        pausePanelGO.SetActive(true);
    }

    IEnumerator ClosePausePanel()
    {
        settingsPanel.SetActive(false);

        yield return new WaitForSeconds(.3f);

        pausePanelGO.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void BackToMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}