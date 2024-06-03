using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 100000;

    void Awake()
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

    public void AddScore(int baseScore)
    {
        int multiplier = 1;
        switch (GameSettings.Instance.currentDifficulty)
        {
            case GameSettings.Difficulty.Medium:
                multiplier = 2;
                break;
            case GameSettings.Difficulty.Hard:
                multiplier = 3;
                break;
        }
        score += baseScore * multiplier;
    
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        // Update the UI here with the new score
        Debug.Log("Score: " + score);
        // Example: uiManager.UpdateScoreText(score);
    }
}
