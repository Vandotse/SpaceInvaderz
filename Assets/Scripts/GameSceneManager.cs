using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Tooltip("Seconds each scene remains active before loading the next one.")]
    public float sceneDuration = 60f;

    private float timer = 0f;
    private bool timerEnabled = true;
    private bool waitingForMiniBoss = false;

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
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        timer = 0f;
        timerEnabled = true;
        waitingForMiniBoss = false;
    }

    private void Update()
    {
        if (!timerEnabled) return;
        if (sceneDuration <= 0f) return;
        if (SceneManager.sceneCountInBuildSettings == 0) return;
        if (Time.timeScale == 0f) return;
        if (GameManager.Instance != null && !GameManager.Instance.IsGameRunning) return;

        timer += Time.deltaTime;
        if (timer >= sceneDuration)
        {
            timer = 0f;
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        if (SceneManager.sceneCountInBuildSettings == 0) return;

        if (PlayerInfo.ForceFreshStart)
        {
            PlayerInfo.ForceFreshStart = false;
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerState();
        }

        Scene currentScene = SceneManager.GetActiveScene();
        int nextIndex = (currentScene.buildIndex + 1) % SceneManager.sceneCountInBuildSettings;

        SceneManager.LoadScene(nextIndex);
    }

    public void BeginMiniBossEncounter()
    {
        waitingForMiniBoss = true;
        timerEnabled = false;
        timer = 0f;
    }

    public void NotifyMiniBossDefeated()
    {
        waitingForMiniBoss = false;
        timerEnabled = true;
        LoadNextScene();
    }
}

