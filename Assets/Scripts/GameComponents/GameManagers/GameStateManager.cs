using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState gameState = GameState.Gameplay;
    public GamePhase currentPhase = GamePhase.Building;
    public bool gameEnd = false;
    public AudioClip loseSound;

    public delegate void Gameplay();
    public static event Gameplay OnGameplay;

    public delegate void Paused();
    public static event Paused OnPaused;
    
    public delegate void Builded();
    public static event Builded OnBuild;
    
    public delegate void Fighted();
    public static event Fighted OnFight;

    public delegate void Loose();
    public static event Loose OnLoose;

    public static GameStateManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ResumeGameState()
    {
        gameState = GameState.Gameplay;
        ApplyGameState();
    }
    public void PauseGameState()
    {
        gameState = GameState.Paused;
        ApplyGameState();
    }

    public void ChangePhaseToFight()
    {
        currentPhase = GamePhase.Fighting;
        ApplyGamePhase(); 
    }
    public void ChangePhaseToBuild()
    {
        currentPhase = GamePhase.Building;
        ApplyGamePhase();
    }

    public void ChangePhaseToLoose()
    {   
        gameEnd = true;
        gameState = GameState.Loose;
        OnLoose?.Invoke();
        AudioManager.instance.PlayClipAt(loseSound, 0, Vector2.zero);
    }

    private void ApplyGameState()
    {
        if (gameEnd) return;
        switch (gameState)
        {
            case GameState.Gameplay:
                OnGameplay?.Invoke();
                break;
            case GameState.Paused:
                OnPaused?.Invoke();
                break;
        }
    }
    private void ApplyGamePhase()
    {
        if (gameEnd) return;
        switch (currentPhase)
        {
            case GamePhase.Fighting:
                OnFight?.Invoke();
                break;
            case GamePhase.Building:
                OnBuild?.Invoke();
                break;
        }
    }
}

public enum GameState
{
    Gameplay,
    Paused,
    Loose,
}
public enum GamePhase
{
    Fighting,
    Building
}