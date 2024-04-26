using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadAndSaveData : MonoBehaviour
{
    public GamesData gamesData = new GamesData();
    public ParametersData parametersData = new ParametersData();

    string gamesDataPath;
    string parametersDataPath;

    private void Awake()
    {
        gamesDataPath = Application.persistentDataPath + "/GamesData.json";
        parametersDataPath = Application.persistentDataPath + "/ParametersData.json";

        if (GamesDataFileAlreadyExist()) LoadGamesData();
        else SaveGamesData();

        if (ParametersDataFileAlreadyExist()) LoadParametersData();
        else SaveParametersData();
    }

    public void SaveGamesData()
    {
        string infoData = JsonUtility.ToJson(gamesData);
        File.WriteAllText(gamesDataPath, infoData);
    }
    public void SaveParametersData()
    {
        string infoData = JsonUtility.ToJson(parametersData);
        File.WriteAllText(parametersDataPath, infoData);
    }

    public void LoadGamesData()
    {
        string infoData = File.ReadAllText(gamesDataPath);
        gamesData = JsonUtility.FromJson<GamesData>(infoData);
    }
    public void LoadParametersData()
    {
        string infoData = File.ReadAllText(parametersDataPath);
        parametersData = JsonUtility.FromJson<ParametersData>(infoData);
    }

    bool GamesDataFileAlreadyExist()
    {
        return File.Exists(gamesDataPath);
    }
    bool ParametersDataFileAlreadyExist()
    {
        return File.Exists(parametersDataPath);
    }
}

[System.Serializable]
public class GamesData
{

}

[System.Serializable]
public class ParametersData
{
    public float masterVolum = 0, musicVolum = 0, soundVolum = 0;
    public bool isFullScreen = true;
}