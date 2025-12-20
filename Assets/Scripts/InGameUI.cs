using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    PlayerPrefs prefs;
    public bool isPaused = false;

    [Header("UI Settings")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseMenuMain;
    [SerializeField] Slider _volumeSlider;
    [SerializeField] Slider _sensitivitySlider;

    public delegate void SettingsChangedDelegate();
    public static event SettingsChangedDelegate OnSettingsChanged;

    private void Awake()
    {
        // Load preferences
        prefs = PreferencesData.LoadPreferences();

        // Initialize sliders
        _volumeSlider.value = prefs.volumeLevel;
        _sensitivitySlider.value = prefs.sensitivity;
    }
    private void OnEnable()
    {
        // Subscribe to game events
        GameManager.OnGamePaused += HandleGamePaused;
        GameManager.OnGameResumed += HandleGameResumed;
        _sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }
    private void Start()
    {
        GUIBrain.Instance.LockCursor();
    }
    private void OnDisable()
    {
        // Unsubscribe from game events
        GameManager.OnGamePaused -= HandleGamePaused;
        GameManager.OnGameResumed -= HandleGameResumed;
        _sensitivitySlider.onValueChanged.RemoveListener(UpdateSensitivity);
    }

    void HandleGamePaused()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        GUIBrain.Instance.StartNewInteraction(pauseMenuMain);
        GUIBrain.Instance.UnlockCursor();
    }
    void HandleGameResumed()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        GUIBrain.Instance.EndInteraction();
        GUIBrain.Instance.LockCursor();
    }
    void UpdateSensitivity(float value)
    {
        prefs.sensitivity = value;
        PreferencesData.SavePreferences(prefs);
        OnSettingsChanged?.Invoke();
    }

    public void TogglePauseMenu()
    {
        if(!isPaused) GameManager.Instance.PauseGame();
        else GameManager.Instance.ResumeGame();
    }
    public void OpenNewMenu(GameObject menu)
    {
        GUIBrain.Instance.OpenNewMenu(menu);
    }
    public void CloseCurrentMenu()
    {
        GUIBrain.Instance.CloseCurrentMenu();
    }
    public void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }
}
