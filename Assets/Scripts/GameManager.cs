using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public delegate void GamePausedDelegate();
    public delegate void GameResumedDelegate();
    public static event GamePausedDelegate OnGamePaused;
    public static event GameResumedDelegate OnGameResumed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads a new scene asynchronously by name.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns>Loader coroutine</returns>
    public Coroutine GoToScene(string sceneName) { return StartCoroutine(LoadSceneAsync(sceneName)); }
    /// <summary>
    /// Exits the application.
    /// </summary>
    /// <remarks>On most platforms, this method terminates the application process. In the Unity Editor, this
    /// method has no effect.</remarks>
    public void ExitGame() { Application.Quit(); }
    /// <summary>
    /// Pauses the game by setting time scale to 0 and invoking pause event.
    /// </summary>
    public void PauseGame()
    {
        OnGamePaused?.Invoke();
        Time.timeScale = 0f;
    }
    /// <summary>
    /// Resumes the game by setting time scale to 1 and invoking resume event.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }

    // ------------ Private Methods ------------

    // -------------- Coroutines -----------------

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            Debug.Log($"Loading progress: {progress * 100}%");

            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;

                Debug.Log("Scene loaded.");
            }
            yield return null;
        }
    }
}
