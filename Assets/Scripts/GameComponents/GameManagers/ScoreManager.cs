using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreTxt;
    public int score = 0;

    public TMP_Text waveTxt;
    public int currentWave = 0;

    public static ScoreManager instance;
    public LoadAndSaveData toolSave;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (scoreTxt)
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
        waveTxt.text = "Day " + currentWave;
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

    public List<int> GetHighscore()
    {
        var hightScore = toolSave.gamesData.highScore;
        if (hightScore.Count < 5 || (hightScore.Count >= 5 && hightScore[hightScore.Count - 1] < score))
        {
            if (hightScore.Count >= 5) hightScore.RemoveAt(hightScore.Count - 1);
            hightScore.Add(score);
            hightScore = hightScore.OrderByDescending(o=> o).ToList();

            toolSave.gamesData.highScore = hightScore;
            toolSave.SaveGamesData();
        }

        hightScore.ForEach(o=> print(o));
        return hightScore;
    }
}