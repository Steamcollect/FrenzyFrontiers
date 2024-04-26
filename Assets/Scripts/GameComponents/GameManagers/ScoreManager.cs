using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public Transform scoreContent;
    public TMP_Text scoreTxt;
    public int score = 0;

    public Animator waveTxtAnim;
    public TMP_Text waveTxt;
    public int currentWave = 0;

    public static ScoreManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        scoreTxt.text = score.ToString() + " pts";
    }

    public void AddScore(int scoreGiven)
    {
        if (GameStateManager.instance.gameEnd) return;

        score += scoreGiven;
        scoreTxt.text = score.ToString() + " pts";

        scoreContent.Bump(1.1f);
    }

    public void NewWave()
    {
        currentWave++;
        waveTxt.text = "Wave : " + currentWave;
        waveTxtAnim.SetTrigger("ShowWave");
    }

    public void SetGameOverPanel()
    {
        GameOverManager.instance.SetPanel(currentWave, score);
    }
}