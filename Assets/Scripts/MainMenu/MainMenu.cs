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
    [SerializeField] TextMeshProUGUI highScoreText; // Referência ao texto de high score

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
        //highScoreText.text = "High Score: " + saveManager.GetHighScore.ToString() + " Kills";
        SoundManager.Instance.PlaySoundEffect(SoundEffects.MenuBackgroundSound);
    }

    private void UpdateHighScoreUI()
    {
        SaveManager.SaveData data = SaveManager.Instance.GetData();
        if (data != null && !string.IsNullOrEmpty(data.playerName))
        {
            highScoreText.text =
                $"High Score:\n" +
                $"Player: {data.playerName}\n" +
                $"Kills: {data.highScoreKills}\n" +
                $"Time: {data.bestTime:0.0}s";
        }
        else
        {
            highScoreText.text = "High Score:\nNo records yet!";
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