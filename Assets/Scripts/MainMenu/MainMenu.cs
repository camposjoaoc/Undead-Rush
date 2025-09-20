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