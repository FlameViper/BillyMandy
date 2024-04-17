using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // Projectile upgrade settings
    public int projectileUpgradeCost = 50; // Initial cost of the projectile upgrade
    public int projectileDamageIncrease = 5; // Damage increase per projectile upgrade
    public int projectileUpgradeCostIncrease = 25; // Cost increase after each projectile upgrade

    // CoinSucker upgrade settings
    public int coinSuckerUpgradeCost = 30; // Initial cost of the CoinSucker upgrade
    public float coinSuckerSpeedIncrease = 2f; // Speed increase per CoinSucker upgrade
    public int coinSuckerUpgradeCostIncrease = 20; // Cost increase after each CoinSucker upgrade

    public int enemySpawnUpgradeCost = 100; // Initial cost for this upgrade
    public int enemiesIncreaseAmount = 2; // How many more enemies will spawn after the upgrade

    public int freezeDurationUpgradeCost = 40; // Initial cost for freeze duration upgrade
    public float freezeDurationIncrease = 0.5f; // How much to increase freeze duration per upgrade
    public int freezeDurationUpgradeCostIncrease = 20; // Cost increase after each freeze duration upgrade

    // Healing upgrade settings
    public int healUpgradeCost = 75; // Initial cost of the healing upgrade
    public int healAmount = 50; // Amount of health restored by the upgrade
    public int healUpgradeCostIncrease = 30; // Cost increase after each healing upgrade purchase

    public int healthUpgradeCost = 100; // Initial cost of the health upgrade
    public int maxHealthIncrease = 20; // Amount of max health increased per upgrade
    public int healthUpgradeCostIncrease = 50; // Cost increase after each health upgrade

    // New citizen spawn upgrade settings
    public GameObject citizenPrefab; // Assign this in the Inspector
    public int citizenSpawnUpgradeCost = 150; // Initial cost for this upgrade
    public int citizenSpawnUpgradeCostIncrease = 50; // Cost increase after each upgrade

    // UI Text References
    public Text healthUpgradeCostText;
    public Text projectileUpgradeCostText;
    public Text coinSuckerUpgradeCostText;
    public Text freezeDurationUpgradeCostText;
    public Text healUpgradeCostText;
    public Text citizenSpawnUpgradeCostText;
    public Text enemySpawnUpgradeCostText;



    private Player player;
    private ResourceManager resourceManager;
    public EnemySpawner enemySpawner; // Assign this in the Inspector

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        player = FindObjectOfType<Player>(); // Find the Player script in the scene
        UpdateCostTexts();

        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager not found in the scene.");
        }
        if (player == null)
        {
            Debug.LogError("Player script not found in the scene.");
        }
    }

    void UpdateCostTexts()
    {
        healthUpgradeCostText.text = "Health Upgrade: " + healthUpgradeCost + " Coins";
        projectileUpgradeCostText.text = "Projectile Damage: " + projectileUpgradeCost + " Coins";
        coinSuckerUpgradeCostText.text = "Coin Sucker Speed: " + coinSuckerUpgradeCost + " Coins";
        freezeDurationUpgradeCostText.text = "Freeze Duration: " + freezeDurationUpgradeCost + " Coins";
        healUpgradeCostText.text = "Heal Amount: " + healUpgradeCost + " Coins";
        citizenSpawnUpgradeCostText.text = "Citizen Spawn: " + citizenSpawnUpgradeCost + " Coins";
        enemySpawnUpgradeCostText.text = "Enemy Spawn Rate: " + enemySpawnUpgradeCost + " Coins";
    }

    private void AfterPurchase()
    {
        UpdateCostTexts();  // Update UI texts after each purchase
    }

    public void PurchaseCitizenSpawnUpgrade()
    {
        if (CanPurchaseUpgrade(citizenSpawnUpgradeCost))
        {
            Instantiate(citizenPrefab, new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)), Quaternion.identity); // Spawn the citizen at a random position
            resourceManager.SubtractCoins(citizenSpawnUpgradeCost);
            citizenSpawnUpgradeCost += citizenSpawnUpgradeCostIncrease; // Increase the cost for the next upgrade
            AfterPurchase();
            Debug.Log("Citizen spawn upgrade purchased.");
        }
    }

    public void PurchaseMaxHealthUpgrade()
    {
        if (CanPurchaseUpgrade(healthUpgradeCost))
        {
            player.maxHealth += maxHealthIncrease; // Increase max health
            player.currentHealth += maxHealthIncrease; // Optionally increase current health as well
            resourceManager.SubtractCoins(healthUpgradeCost); // Deduct the cost from player resources
            healthUpgradeCost += healthUpgradeCostIncrease; // Increase the cost for the next upgrade
            AfterPurchase();
            Debug.Log("Max Health upgrade purchased. New Max Health: " + player.maxHealth);
        }
    }

    public void PurchaseHealUpgrade()
    {
        if (CanPurchaseUpgrade(healUpgradeCost))
        {
            player.currentHealth += healAmount;
            player.currentHealth = Mathf.Min(player.currentHealth, player.maxHealth); // Ensure health does not exceed max
            resourceManager.SubtractCoins(healUpgradeCost);
            healUpgradeCost += healUpgradeCostIncrease; // Increase the cost for the next upgrade
            AfterPurchase();
            Debug.Log("Healing upgrade purchased. Current health: " + player.currentHealth);
        }
    }

    public void PurchaseFreezeDurationUpgrade()
    {
        if (CanPurchaseUpgrade(freezeDurationUpgradeCost))
        {
            SupportProjectile.freezeDuration += freezeDurationIncrease; // Upgrade the freeze duration
            resourceManager.SubtractCoins(freezeDurationUpgradeCost);
            freezeDurationUpgradeCost += freezeDurationUpgradeCostIncrease; // Increase the cost for the next upgrade
            AfterPurchase();
            Debug.Log("Freeze duration upgrade purchased. New duration: " + SupportProjectile.freezeDuration);
        }
    }


    public void PurchaseProjectileUpgrade()
    {
        if (CanPurchaseUpgrade(projectileUpgradeCost))
        {
            Projectile.damageAmount += projectileDamageIncrease;
            resourceManager.SubtractCoins(projectileUpgradeCost);
            projectileUpgradeCost += projectileUpgradeCostIncrease; // Increase the cost for the next upgrade
            AfterPurchase();
            Debug.Log("Projectile upgrade purchased. New damage: " + Projectile.damageAmount);
        }
    }

    public void PurchaseCoinSuckerUpgrade()
    {
        CoinSucker coinSucker = FindObjectOfType<CoinSucker>();
        if (coinSucker == null)
        {
            Debug.LogError("CoinSucker not found in the scene.");
            return;
        }

        if (CanPurchaseUpgrade(coinSuckerUpgradeCost))
        {
            coinSucker.moveSpeed += coinSuckerSpeedIncrease;
            resourceManager.SubtractCoins(coinSuckerUpgradeCost);
            coinSuckerUpgradeCost += coinSuckerUpgradeCostIncrease; // Increase the cost for the next upgrade
            AfterPurchase();
            Debug.Log("CoinSucker upgrade purchased. New speed: " + coinSucker.moveSpeed);
        }
    }

    public void PurchaseEnemySpawnUpgrade()
    {
        // Check if the player has enough coins
        if (resourceManager.Coins >= enemySpawnUpgradeCost)
        {
            // Deduct the coins
            resourceManager.SubtractCoins(enemySpawnUpgradeCost);

            // Increase the spawn rate
            enemySpawner.IncreaseMaxEnemies(enemiesIncreaseAmount);
            AfterPurchase();
            // Optionally increase the cost for the next purchase or adjust other variables
            Debug.Log("Purchased Enemy Spawn Rate Upgrade. Max Enemies is now: " + enemySpawner.maxEnemies);
        }
        else
        {
            Debug.Log("Not enough coins for Enemy Spawn Rate Upgrade.");
        }
    }

    private bool CanPurchaseUpgrade(int cost)
    {
        if (resourceManager.Coins >= cost)
        {
            return true;
        }
        else
        {
            Debug.Log("Not enough coins to purchase the upgrade.");
            return false;
        }
    }
}
