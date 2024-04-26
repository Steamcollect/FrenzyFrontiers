using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    bool isSettingsOpen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isSettingsOpen) SettingButton();
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void SettingButton()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}