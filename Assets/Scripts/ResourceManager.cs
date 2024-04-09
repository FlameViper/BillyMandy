using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public int Coins { get; private set; } // The only resource

    public Text coinsText; // UI text to display coins

    void Start()
    {
        Coins = 0; // Initialize coins to 0 or any starting value you prefer
        UpdateUI(); // Update the UI with the starting data
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateUI(); // Update the UI with the new coins value
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
    }
}
