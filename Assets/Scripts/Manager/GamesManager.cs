using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] GameObject upGradeUI;

    // Experience and Leveling
    [SerializeField] private int experience = 0;
    [SerializeField] private int level = 1;
    [SerializeField] private int xpToNextLevel = 20;

    [SerializeField] private Image xpBarFill;
    [SerializeField] TextMeshProUGUI levelText;

    public GameState CurrentState => currentGameState;

    private void Start()
    {
        xpBarFill.fillAmount = 0f;
        levelText.text = "Lv 1 (0%)";
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

    public void AddExperience(int amout)
    {
        experience += amout;
        Debug.Log($"XP: {experience}/{xpToNextLevel}");

        if (experience >= xpToNextLevel)
        {
            LevelUp();
        }
        UpdateXPUI();
    }

    private void LevelUp()
    {
        level++;
        experience = 0;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        Debug.Log($"LEVEL UP! Level atual: {level}");
        UpdateXPUI();
        // Vai para o estado de upgrade
        SwitchState(GameState.Upgrade);
    }

    private void UpdateXPUI()
    {
        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = (float)experience / xpToNextLevel;
        }

        if (levelText != null)
        {
            float percent = (float)experience / xpToNextLevel * 100f;
            levelText.text = $"Lv {level}  ({percent:0}%)";
        }
    }
}