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

    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Player player;

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
        currentGameState = aState;
    }
}