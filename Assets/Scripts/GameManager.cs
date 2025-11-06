using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int startingLives = 3;
    public int maxHealth = 100;
    public int healthPerHit = 25;
    public int scorePerAsteroid = 10;

    public float baseSpawnInterval = 2f;
    public float minSpawnInterval = 0.5f;
    public float spawnIntervalDecreaseRate = 0.1f;
    public float scoreIncreaseRate = 0.5f;
    public float scoreMultiplierPerSecond = 0.1f;

    public GameUI gameUI;

    private int score = 0;
    private int currentHealth = 100;
    private int currentLives = 3;
    private float gameTime = 0f;

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

        currentHealth = maxHealth;
        currentLives = startingLives;
        score = 0;

        if (gameUI == null)
        {
            gameUI = FindObjectOfType<GameUI>();
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Time.timeScale > 0f)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void AsteroidDestroyed()
    {
        int scaledScore = GetCurrentScorePerAsteroid();
        AddScore(scaledScore);
    }

    public void PlayerHit()
    {
        currentHealth -= healthPerHit;
        
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
            Time.timeScale = 0f;
        }
        else
        {
            currentHealth = maxHealth;
        }
        
        UpdateUI();
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

    public float GetCurrentSpawnInterval()
    {
        float scaledInterval = baseSpawnInterval - (gameTime * spawnIntervalDecreaseRate);
        return Mathf.Max(scaledInterval, minSpawnInterval);
    }

    public int GetCurrentScorePerAsteroid()
    {
        float timeMultiplier = 1f + (gameTime * scoreMultiplierPerSecond);
        float baseScoreIncrease = gameTime * scoreIncreaseRate;
        int scaledScore = Mathf.RoundToInt(scorePerAsteroid * timeMultiplier + baseScoreIncrease);
        return Mathf.Max(scaledScore, scorePerAsteroid);
    }

    public int GetScore() => score;
    public int GetHealth() => currentHealth;
    public int GetLives() => currentLives;
    public float GetGameTime() => gameTime;
}
