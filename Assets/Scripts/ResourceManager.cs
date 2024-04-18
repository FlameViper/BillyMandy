using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public int Coins { get; private set; }
    public int EnemyCoins { get; private set; } // Coins collected by the enemy
    public int Score { get; private set; } // Manage the score

    public Text coinsText; // Existing UI text to display coins on the Upgrade screen
    public Text enemyCoinsText; // New UI text to display enemy coins
    public Text battleCoinsText; // Added UI text to display coins on the Battle screen
    public Text scoreText; // UI text to display score

    void Start()
    {
        Coins = 0; // Initialize coins to 0
        EnemyCoins = 0;
        Score = 0;  // Initialize score to 0
        UpdateUI(); // Update the UI with the starting data
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateUI(); // Update the UI with the new coins value
    }

    public void AddEnemyCoins(int amount)
    {
        EnemyCoins += amount;
        UpdateUI();
    }

    public void AddScore(int baseScore)
    {
        int multiplier = 1; // Default for Easy
        switch (GameSettings.Instance.currentDifficulty)
        {
            case GameSettings.Difficulty.Medium:
                multiplier = 2;
                break;
            case GameSettings.Difficulty.Hard:
                multiplier = 3;
                break;
        }
        Score += baseScore * multiplier;
        UpdateUI(); // Update the score display
    }

    public bool SubtractCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            UpdateUI(); // Update the UI with the new coins value
            return true;
        }
        else
        {
            Debug.Log("Not enough coins to purchase the upgrade!");
            return false;
        }
    }

    public void UpdateUI()
    {
        if (coinsText != null)
            coinsText.text = "Coins: " + Coins; // Update coin display on the Upgrade screen

        if (enemyCoinsText != null)
            enemyCoinsText.text = "Enemy Coins: " + EnemyCoins;

        if (battleCoinsText != null)
            battleCoinsText.text = "Coins: " + Coins; // Update coin display on the Battle screen

        if (scoreText != null)
            scoreText.text = "Score: " + Score; // Display the current score
    }
}
