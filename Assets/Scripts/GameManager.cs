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


    public Coroutine GoToScene(string sceneName) { return StartCoroutine(LoadSceneAsync(sceneName)); }
    public void ExitGame() { Application.Quit(); }
    public void PauseGame()
    {
        OnGamePaused?.Invoke();
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }

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
