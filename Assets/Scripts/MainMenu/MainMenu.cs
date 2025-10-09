using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")] [SerializeField] private GameObject MainMenuUI;
    [SerializeField] private GameObject SettingsUI;

    [Header("Settings")] [SerializeField] private Slider VolumeSlider;
    [SerializeField] private Toggle Fullscreen;
    [SerializeField] private Toggle VSync;

    [SerializeField] SaveManager saveManager; // Referência ao SaveManager
    [Header("High Score UI")]
    [SerializeField] private TextMeshProUGUI highScorePlayerText;
    [SerializeField] private TextMeshProUGUI highScoreKillsText;
    [SerializeField] private TextMeshProUGUI highScoreTimeText;

    private void Start()
    {
        // Inicializa os controles com os valores salvos
        VolumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 1f);
        Fullscreen.isOn = Screen.fullScreen;
        VSync.isOn = QualitySettings.vSyncCount > 0;

        ApplyVolume(VolumeSlider.value);

        // Adiciona listeners aos controles
        VolumeSlider.onValueChanged.AddListener(ApplyVolume);
        Fullscreen.onValueChanged.AddListener(SetFullscreen);
        VSync.onValueChanged.AddListener(SetVSync);

        // Carrega o high score do SaveManager
        SaveManager.Instance.LoadData();
        UpdateHighScoreUI();
        SoundManager.Instance.PlaySoundEffect(SoundEffects.MenuBackgroundSound);
    }

    private void UpdateHighScoreUI()
    {
        SaveManager.SaveData data = SaveManager.Instance.GetData();
        if (data != null && !string.IsNullOrEmpty(data.playerName))
        {
            highScorePlayerText.text = $"Player: {data.playerName}";
            highScoreKillsText.text = $"Kills: {data.highScoreKills}";
            int minutes = Mathf.FloorToInt(data.bestTime / 60f);
            int seconds = Mathf.FloorToInt(data.bestTime % 60f);
            highScoreTimeText.text = $"Best Time: {minutes}m {seconds}s";
        }
        else
        {
            highScorePlayerText.text = "Player: -";
            highScoreKillsText.text = "Kills: -";
            highScoreTimeText.text = "Time: -";
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        MainMenuUI.SetActive(false);
        SettingsUI.SetActive(true);
    }

    public void ShowMainMenu()
    {
        SettingsUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    //Painel de Configurações
    public void ApplyVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
    }
}