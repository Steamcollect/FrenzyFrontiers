using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TMP_Text waveTxt;
    public TMP_Text scoreTxt;

    public GameObject deathPanel;

    public AudioClip loseSound;

    public static GameOverManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetPanel(int wave, int score)
    {
        AudioManager.instance.PlayClipAt(loseSound, 0, Vector2.zero);

        waveTxt.text = "Wave max : " + wave.ToString();
        scoreTxt.text = "Score : " + score.ToString();

        deathPanel.SetActive(true);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void RetryButton()
    {
        SceneManager.LoadScene("Game");
    }
}