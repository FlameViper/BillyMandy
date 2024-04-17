using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public int Coins { get; private set; }
    public int Score { get; private set; } // Added to handle score

    public Text coinsText; // UI text to display coins
    public Text scoreText; // Added UI text to display score

    void Start()
    {
        Coins = 0; // Initialize coins to 0
        Score = 0;  // Initialize score to 0
        UpdateUI(); // Update the UI with the starting data
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateUI(); // Update the UI with the new coins value
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
        coinsText.text = "Coins: " + Coins; // Display the current amount of coins
        scoreText.text = "Score: " + Score; // Display the current score
    }
}
