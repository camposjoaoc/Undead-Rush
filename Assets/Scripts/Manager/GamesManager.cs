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
    [SerializeField] SaveManager saveManager;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TMP_InputField nameInput;
    private float playTime;

    private void Start()
    {
        playTime = 0f;
        
        xpBarFill.fillAmount = 0f;
        levelText.text = "Lv 1 (0%)";
        
        SwitchState(GameState.Playing);
        
        saveManager.LoadData();

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        SoundManager.Instance.PlaySoundEffect(SoundEffects.BackgroundSound);
    }

    private void Update()
    {
        switch (currentGameState)
        {
            case GameState.Playing:
                // Lógica do jogo em andamento atualizado managers
                enemyManager.UpdateEnemyManager();
                player.UpdatePlayer();
                playTime += Time.deltaTime;

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
                break;
        }
    }

    public void SwitchState(GameState aState)
    {
        currentGameState = aState;
        if (pauseUI != null)
        {
            pauseUI.SetActive(currentGameState == GameState.Paused);
        }

        if (upGradeUI != null)
        {
            upGradeUI.SetActive(currentGameState == GameState.Upgrade);
        }

        if (gameOverUI != null)
        {
            if (currentGameState == GameState.GameOver)
            {
                ShowGameOver();
                SoundManager.Instance.PlaySoundEffect(SoundEffects.GameOver);
            }
            else
            {
                gameOverUI.SetActive(false);
            }
        }
        //controla animações de pausa
        bool shouldPauseAnims = (currentGameState == GameState.Paused || currentGameState == GameState.Upgrade);
        SetAnimatorsPaused(shouldPauseAnims);
    }

    private void ShowGameOver()
    {
        if (gameOverUI == null) return;

        gameOverUI.SetActive(true);

        int kills = enemyManager.KillCount;
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);

        statsText.text = $"Enemy Kills: {kills} Time Survived: {minutes:00}:{seconds:00}";
    }

    public void SaveScore()
    {
        int kills = enemyManager.KillCount;
        string playerName = nameInput.text;
        saveManager.SetHighScore(kills, playerName, playTime);
        
        statsText.text += "\n<color=green>Score Saved!</color>";
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
        Debug.Log($"[GamesManager] XP: {experience}/{xpToNextLevel}");

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
        Debug.Log($"[GamesManager] LEVEL UP! Level atual: {level}");
        
        SoundManager.Instance.PlaySoundEffect(SoundEffects.LevelUp);
        
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

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    private void SetAnimatorsPaused(bool paused)
    {
        float speed = paused ? 0f : 1f;

        // pausa o player
        if (player != null)
        {
            Animator playerAnim = player.GetComponentInChildren<Animator>();
            if (playerAnim != null) playerAnim.speed = speed;
        }

        // pausa inimigos
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Animator enemyAnim = enemy.GetComponentInChildren<Animator>();
            if (enemyAnim != null) enemyAnim.speed = speed;
        }
    }
}