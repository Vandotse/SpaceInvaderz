using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour 
{
    private Label score_;
    private Label health_;
    private VisualElement[] lives_;

    public void OnEnable() 
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        score_ = root.Q<Label>("Score");
        health_ = root.Q<Label>("Health");
        lives_ = new VisualElement[3];
        lives_[0] = root.Q<VisualElement>("Life-1");
        lives_[1] = root.Q<VisualElement>("Life-2");
        lives_[2] = root.Q<VisualElement>("Life-3");
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
}
