using UnityEngine;

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



    private ResourceManager resourceManager;
    public EnemySpawner enemySpawner; // Assign this in the Inspector

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager not found in the scene.");
        }
    }



    public void PurchaseFreezeDurationUpgrade()
    {
        if (CanPurchaseUpgrade(freezeDurationUpgradeCost))
        {
            SupportProjectile.freezeDuration += freezeDurationIncrease; // Upgrade the freeze duration
            resourceManager.SubtractCoins(freezeDurationUpgradeCost);
            freezeDurationUpgradeCost += freezeDurationUpgradeCostIncrease; // Increase the cost for the next upgrade
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
