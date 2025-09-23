using System;
using NUnit.Framework;
using UnityEngine;

public class GamesManager : MonoBehaviour
{
    //Singleton
    public static GamesManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            throw new Exception("There can only be one instance of GamesManager");
        }

        Instance = this;
    }

    //Finite state machine game states
    public enum GameState
    {
        Playing,
        Paused,
        Upgrade,
        GameOver
    }

    private GameState currentGameState;
    // References to other managers and UI
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Player player;
    [SerializeField] private GameObject pauseUI;
    
    // Experience and Leveling
    private int currentExperience = 0;
    [SerializeField] private int maxExperience = 1;
    [SerializeField] GameObject upGradeUI;
    private void Start()
    {
        SwitchState(GameState.Playing);
    }

    private void Update()
    {
        switch (currentGameState)
        {
            case GameState.Playing:
                // Lógica do jogo em andamento atualizado managers
                enemyManager.UpdateEnemyManager();
                player.UpdatePlayer();

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SwitchState(GameState.Paused);
                }

                break;
            case GameState.Paused:
                // Lógica do jogo pausado
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SwitchState(GameState.Playing);
                }

                break;
            case GameState.Upgrade:
                // Lógica do jogo em modo de upgrade
                break;
            case GameState.GameOver:
                // Lógica do jogo terminado
                break;
        }
    }

    public void SwitchState(GameState aState)
    {
        upGradeUI.SetActive(aState == GameState.Upgrade);
        currentGameState = aState;

        if (pauseUI != null)
        {
            pauseUI.SetActive(currentGameState == GameState.Paused);
        }
    }

    public void TogglePause()
    {
        if (currentGameState == GameState.Playing)
        {
            SwitchState(GameState.Paused);
        }
        else if (currentGameState == GameState.Paused)
        {
            SwitchState(GameState.Playing);
        }
    }
    
    public void AddExperience(int someXP= 1)
    {
        currentExperience += someXP;
        
        if (currentExperience >= maxExperience)
        {
            currentExperience = 0;
            maxExperience *= 2;
            SwitchState(GameState.Upgrade);
            Debug.Log("Level Up! Next level at " + maxExperience + " XP.");
        }
    }
}