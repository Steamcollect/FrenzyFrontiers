using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreTxt;
    public int score = 0;

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
        if (GameStateManager.instance.gameEnd || !scoreTxt) return;

        score += scoreGiven;
        scoreTxt.text = score.ToString() + " pts";

        scoreTxt.transform.Bump(1.1f);
    }

    public void NewWave()
    {
        if (!waveTxt) return;

        currentWave++;
        waveTxt.text = "Wave " + currentWave;
        waveTxt.gameObject.SetActive(true);
        waveTxt.transform.Bump(1.1f);

        StartCoroutine(HidWaveTxt());
    }

    IEnumerator HidWaveTxt()
    {
        yield return new WaitForSeconds(3);
        Color initColor = waveTxt.color;

        waveTxt.DOColor(new Color(initColor.r, initColor.g, initColor.b, 0), 1).OnComplete(() =>
        {
            waveTxt.gameObject.SetActive(false);
            waveTxt.color = initColor;
        });
    }

    public void SetGameOverPanel()
    {
        GameOverManager.instance.SetPanel(currentWave, score);
    }
}