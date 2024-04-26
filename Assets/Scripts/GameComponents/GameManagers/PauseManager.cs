using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanelGO;
    public GameObject settingsPanel;

    Animator pausePanelAnim;

    public ParticleSystem[] particlesToCheck;
    bool pauseResumeInput;

    public bool isOnMenu = false;

    CameraController cameraController;
    GameStateManager gameStateManager;

    private void Awake()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        gameStateManager = FindFirstObjectByType<GameStateManager>();
    }

    private void Start()
    {
        //gameStateManager.PauseGameState();
        pausePanelAnim = pausePanelGO.GetComponent<Animator>();
        pausePanelGO.SetActive(false);
    }

    private void Update()
    {
        if (pauseResumeInput)
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

        SetParticleState(false);

        pausePanelAnim.SetBool("IsOpen", false);
        StartCoroutine(ClosePausePanel());
    }
    public void Pause()
    {
        gameStateManager.PauseGameState();

        isOnMenu = true;

        SetParticleState(true);

        pausePanelGO.SetActive(true);
        pausePanelAnim.SetBool("IsOpen", true);
    }

    void SetParticleState(bool stop)
    {
        if (stop)
        {
            for (int i = 0; i < particlesToCheck.Length; i++)
            {
                if (particlesToCheck[i].isPlaying) particlesToCheck[i].Pause();
            }
        }
        else
        {
            for (int i = 0; i < particlesToCheck.Length; i++)
            {
                if (particlesToCheck[i].isPaused) particlesToCheck[i].Play();
            }
        }

    }

    public void TutorialButton()
    {
        Debug.LogWarning("Launch tutorial scene");
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