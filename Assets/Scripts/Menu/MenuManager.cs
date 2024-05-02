using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    private bool isSettingsOpen;
    private float posSettingsPanel;

    private void Start()
    {
        posSettingsPanel = settingsPanel.transform.localPosition.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isSettingsOpen) SettingButton();
    }

    public void PlayButton(bool isTuto)
    {
        SceneManager.LoadScene(isTuto? "Tutorial":"Game");
    }

    public void SettingButton()
    {
        isSettingsOpen = !isSettingsOpen;
        if(!isSettingsOpen)
        {
            settingsPanel.transform.DOLocalMoveX(posSettingsPanel, 0.5f).OnComplete(() => settingsPanel.SetActive(isSettingsOpen));
        }
        else
        {
            settingsPanel.SetActive(isSettingsOpen);
            settingsPanel.transform.DOLocalMoveX(0f , 0.5f);
        }
        
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}