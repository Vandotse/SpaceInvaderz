using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour 
{
    private Label score_;
    private Label health_;
    private VisualElement[] lives_;
    private Button startButton_;
    private VisualElement startOverlay_;
    private VisualElement gameOverBanner_;
    private Label highScoreLabel_;
    private GameManager boundManager_;

    public void OnEnable() 
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        score_ = root.Q<Label>("Score");
        health_ = root.Q<Label>("Health");
        lives_ = new VisualElement[3];
        lives_[0] = root.Q<VisualElement>("Life-1");
        lives_[1] = root.Q<VisualElement>("Life-2");
        lives_[2] = root.Q<VisualElement>("Life-3");

        startOverlay_ = root.Q<VisualElement>("StartOverlay");
        gameOverBanner_ = root.Q<VisualElement>("GameOverBanner");
        startButton_ = root.Q<Button>("StartButton");
        highScoreLabel_ = root.Q<Label>("HighScoreLabel");

        if (startButton_ != null)
        {
            startButton_.clicked += HandleStartButtonClicked;
        }
    }

    private void OnDisable()
    {
        if (startButton_ != null)
        {
            startButton_.clicked -= HandleStartButtonClicked;
        }
    }

    public void SetScore(int score) 
    {
        if (score_ != null)
        {
            score_.text = score.ToString("D5");
        }
    }

    public void SetHealth(int health) 
    {
        if (health_ != null)
        {
            health_.text = health.ToString() + "%";
        }
    }

    public void SetLives(int lives) 
    {
        if (lives_ != null)
        {
            for (int i = 0; i < lives_.Length; i++) 
            {
                if (lives_[i] != null)
                {
                    lives_[i].style.display = (i < lives) ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
        }
    }

    public void BindGameManager(GameManager manager)
    {
        boundManager_ = manager;
    }

    public void SetHighScore(int score)
    {
        if (highScoreLabel_ != null)
        {
            highScoreLabel_.text = $"High Score: {score:D5}";
        }
    }

    public void SetStartPromptVisible(bool visible)
    {
        if (startOverlay_ != null)
        {
            startOverlay_.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public void SetGameOverVisible(bool visible)
    {
        if (gameOverBanner_ != null)
        {
            gameOverBanner_.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void HandleStartButtonClicked()
    {
        boundManager_?.StartGame();
    }
}
