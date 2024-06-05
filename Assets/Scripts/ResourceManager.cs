using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;
    [SerializeField] int initialCoinAmount = 10;
 
    public int Coins { get; private set; }
    public int EnemyCoins { get; private set; } // Coins collected by the enemy
    public int Score { get; private set; }  // Manage the score

    public Text coinsText; // Existing UI text to display coins on the Upgrade screen
    public Text enemyCoinsText; // New UI text to display enemy coins
    public Text battleCoinsText; // Added UI text to display coins on the Battle screen
    public Text scoreText; // UI text to display score
    public TextMeshProUGUI TDScoreText;
    public TextMeshProUGUI TDCoinsText;
   

    public EnemySpawner enemySpawner; // Reference to the EnemySpawner
    private int scoreWhenFightingTheBoss;
    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        Coins = initialCoinAmount; // Initialize coins to 0
        EnemyCoins = 0;
        Score = 500;  // Initialize score to 0
        UpdateUI(); // Update the UI with the starting data
        //enemySpawner.PrepareBossSpawn(true);
      
    }


    //void Update()
    //{
    //    // Example condition for boss spawn: every 3 EnemyCoins
    //    int bossSpawnThreshold = 3; // Set the threshold for boss spawn

    //    // Regularly check if there are enough coins for a boss spawn
    //    while (EnemyCoins >= bossSpawnThreshold && !enemySpawner.ShouldSpawnBossNextRound)
    //    {
    //        EnemyCoins -= bossSpawnThreshold;
    //        enemySpawner.PrepareBossSpawn(); // Prepare the boss spawn for the next round
    //    }
    //}
    


    //void CheckForBossSpawn()
    //{
    //    int bossSpawnThreshold = 3; // Set the threshold for boss spawn

    //    while (EnemyCoins >= bossSpawnThreshold && !enemySpawner.ShouldSpawnBossNextRound)
    //    {
    //        EnemyCoins -= bossSpawnThreshold;
    //        enemySpawner.PrepareBossSpawn(); // Prepare the boss spawn for the next round
    //    }
    //}

    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateUI(); // Update the UI with the new coins value
    }

    public void AddEnemyCoins(int amount)
{
    EnemyCoins += amount;
    UpdateUI(); // Always update the UI when coins change
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

        if(TDScoreText != null) 
            TDScoreText.text = "Score: " + Score;

        if (TDCoinsText != null)
            TDCoinsText.text = "Coins: " + Coins;

    }

    public void UpdateBossfightScore() {
        scoreWhenFightingTheBoss = Score;
   
    }
    public void ResetBossFightScore() {
        Score = scoreWhenFightingTheBoss;
        UpdateUI();
    }
}
