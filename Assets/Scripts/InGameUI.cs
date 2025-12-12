using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public bool isPaused = false;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseMenuMain;

    private void OnEnable()
    {
        GameManager.OnGamePaused += HandleGamePaused;
        GameManager.OnGameResumed += HandleGameResumed;
    }
    private void OnDisable()
    {
        GameManager.OnGamePaused -= HandleGamePaused;
        GameManager.OnGameResumed -= HandleGameResumed;
    }

    void HandleGamePaused()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        GUIBrain.Instance.StartNewInteraction(pauseMenuMain);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void HandleGameResumed()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        GUIBrain.Instance.EndInteraction();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
