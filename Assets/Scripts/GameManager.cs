using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int startingLives = 3;
    public int maxHealth = 100;
    public int healthPerHit = 25;
    public int scorePerAsteroid = 10;

    public float scoreIncreaseRate = 0.25f;
    public float scoreMultiplierPerSecond = 0.05f;

    [Header("Flow Settings")]
    public float startDelaySeconds = 3f;
    public float gameOverDisplaySeconds = 3f;

    public GameUI gameUI;

    private int score = 0;
    private int highScore = 0;
    private int currentHealth = 100;
    private int currentLives = 3;
    private float gameTime = 0f;

    private bool awaitingStart = true;
    private bool startCountdownActive = false;
    private bool gameRunning = false;
    private bool canSpawnTargets = false;
    private bool gameOver = false;
    private float startDelayTimer = 0f;
    private Coroutine gameOverRoutine;

    public bool CanSpawnTargets => canSpawnTargets;
    public bool IsGameRunning => gameRunning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (PlayerInfo.GameLoopCount < 1)
        {
            PlayerInfo.GameLoopCount = 1;
        }

        if (PlayerInfo.ForceFreshStart)
        {
            PlayerInfo.Clear();
            PlayerInfo.ForceFreshStart = false;
        }

        bool hasSavedState = PlayerInfo.HasData;

        if (hasSavedState)
        {
            score = PlayerInfo.CurrentScore;
            currentHealth = Mathf.Clamp(PlayerInfo.CurrentHealth, 0, maxHealth);
            currentLives = Mathf.Clamp(PlayerInfo.NumRemainingLives, 0, startingLives);
            gameTime = Mathf.Max(0f, PlayerInfo.StoredGameTime);
        }
        else
        {
            currentHealth = maxHealth;
            currentLives = startingLives;
            score = 0;
            gameTime = 0f;
        }

        highScore = Mathf.Max(highScore, PlayerInfo.HighScore);

        if (gameUI == null)
        {
            gameUI = FindObjectOfType<GameUI>();
        }

        if (gameUI != null)
        {
            gameUI.BindGameManager(this);
            gameUI.SetHighScore(highScore);
        }

        awaitingStart = !hasSavedState;
        startCountdownActive = false;
        gameRunning = !awaitingStart;
        canSpawnTargets = !awaitingStart;
        gameOver = false;
        startDelayTimer = Mathf.Max(0f, startDelaySeconds);

        Time.timeScale = awaitingStart ? 0f : 1f;

        if (gameUI != null)
        {
            gameUI.SetStartPromptVisible(awaitingStart);
            gameUI.SetGameOverVisible(false);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (startCountdownActive)
        {
            startDelayTimer -= Time.deltaTime;
            if (startDelayTimer <= 0f)
            {
                EnableGameplay();
            }
        }

        if (gameRunning && Time.timeScale > 0f)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        if (!awaitingStart || startCountdownActive || gameOver) return;

        awaitingStart = false;
        startCountdownActive = true;
        startDelayTimer = Mathf.Max(0f, startDelaySeconds);
        Time.timeScale = 1f;

        if (gameUI != null)
        {
            gameUI.SetStartPromptVisible(false);
            gameUI.SetGameOverVisible(false);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (score > highScore)
        {
            highScore = score;
            PlayerInfo.HighScore = highScore;
            gameUI?.SetHighScore(highScore);
        }
        UpdateUI();
    }

    public void AsteroidDestroyed()
    {
        AwardScaledScore(scorePerAsteroid);
    }

    public void PlayerHit()
    {
        ApplyPlayerDamage(healthPerHit);
    }

    public void ApplyPlayerDamage(int damageAmount)
    {
        if (damageAmount <= 0 || gameOver || awaitingStart) return;

        currentHealth -= damageAmount;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDeath();
        }
        else
        {
            UpdateUI();
        }
    }

    private void PlayerDeath()
    {
        currentLives--;
        
        if (currentLives <= 0)
        {
            currentLives = 0;
            HandleGameOver();
        }
        else
        {
            currentHealth = maxHealth;
            UpdateUI();
            RestartCurrentScene();
        }
    }

    private void UpdateUI()
    {
        if (gameUI != null)
        {
            gameUI.SetScore(score);
            
            int healthPercentage = Mathf.RoundToInt((currentHealth / (float)maxHealth) * 100f);
            gameUI.SetHealth(healthPercentage);
            
            gameUI.SetLives(currentLives);
        }
    }

    public int GetCurrentScorePerAsteroid()
    {
        return CalculateScaledScore(scorePerAsteroid);
    }

    public int GetScore() => score;
    public int GetHealth() => currentHealth;
    public int GetLives() => currentLives;
    public float GetGameTime() => gameTime;
    public int GetMaxHealth() => maxHealth;

    public void SavePlayerState()
    {
        PlayerInfo.CurrentScore = score;
        PlayerInfo.CurrentHealth = currentHealth;
        PlayerInfo.NumRemainingLives = currentLives;
        PlayerInfo.StoredGameTime = gameTime;
        PlayerInfo.HighScore = Mathf.Max(PlayerInfo.HighScore, highScore);
        PlayerInfo.HasData = true;
    }

    private void RestartCurrentScene()
    {
        SavePlayerState();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AwardScaledScore(int basePoints)
    {
        if (basePoints <= 0) return;
        AddScore(CalculateScaledScore(basePoints));
    }

    private int CalculateScaledScore(int basePoints)
    {
        float timeMultiplier = 1f + (gameTime * scoreMultiplierPerSecond);
        float baseScoreIncrease = gameTime * scoreIncreaseRate;
        int scaledScore = Mathf.RoundToInt(basePoints * timeMultiplier + baseScoreIncrease);
        return Mathf.Max(basePoints, scaledScore);
    }

    private void EnableGameplay()
    {
        startCountdownActive = false;
        canSpawnTargets = true;
        gameRunning = true;
    }

    private void HandleGameOver()
    {
        if (gameOver) return;

        gameOver = true;
        gameRunning = false;
        canSpawnTargets = false;
        PlayerInfo.Clear();
        UpdateUI();

        Time.timeScale = 0f;

        if (gameUI != null)
        {
            gameUI.SetGameOverVisible(true);
        }

        if (gameOverRoutine != null)
        {
            StopCoroutine(gameOverRoutine);
        }
        gameOverRoutine = StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        float waitTime = Mathf.Max(3f, gameOverDisplaySeconds);
        yield return new WaitForSecondsRealtime(waitTime);
        RestartEntireGame();
    }

    private void RestartEntireGame()
    {
        PlayerInfo.GameLoopCount = 1;
        PlayerInfo.Clear();
        PlayerInfo.ForceFreshStart = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
