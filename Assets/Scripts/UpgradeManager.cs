using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // Projectile upgrade settings
    public int projectileUpgradeCost = 50; // Initial cost of the projectile upgrade
    public int projectileDamageIncrease = 5; // Damage increase per projectile upgrade
    public int projectileUpgradeCostIncrease = 25; // Cost increase after each projectile upgrade

    //Weapon settings
    public int fireballCost = 0;
    public int fireballDamage = 20;
    public int boomerangCost = 50;
    public int boomerangDamage = 50;

    //Support Settings
    public int freezBurstCost = 30;
   

    // CoinSucker upgrade settings
    public int coinSuckerUpgradeCost = 30; // Initial cost of the CoinSucker upgrade
    public float coinSuckerPowerIncrease = 2f; // Suck power increase per CoinSucker upgrade
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

    public WarriorGotchiSpawner warriorGotchiSpawner; // Reference to the WarriorGotchiSpawner

    public int gotchiSpawnRateUpgradeCost = 50; // Initial cost to decrease the spawn interval
    public int gotchiMaxIncreaseUpgradeCost = 100; // Initial cost to increase the maximum number of Gotchis

    public int gotchiHealthUpgradeCost = 75; // Initial cost to increase health
    public int gotchiDamageUpgradeCost = 100; // Initial cost to increase damage

    public int gotchiHealthUpgradeAmount = 20; // Health increase per upgrade
    public int gotchiDamageUpgradeAmount = 5;  // Damage increase per upgrade

    // UI Text References
    public Text healthUpgradeCostText;
    public Text projectileUpgradeCostText;
    public Text coinSuckerUpgradeCostText;
    public Text freezeDurationUpgradeCostText;
    public Text healUpgradeCostText;
    public Text citizenSpawnUpgradeCostText;
    public Text enemySpawnUpgradeCostText;

    public Text gotchiSpawnRateUpgradeCostText;  // UI Text for Gotchi spawn rate upgrade
    public Text gotchiMaxUpgradeCostText;        // UI Text for Gotchi max count upgrade
    public Text gotchiHealthUpgradeCostText;     // UI Text for Gotchi health upgrade
    public Text gotchiDamageUpgradeCostText;     // UI Text for Gotchi damage upgrade
    public TMP_Text fireballWeaponCostText;    
    public TMP_Text boomerangWeaponCostText;     
    public TMP_Text burstFreezCostText;    
   


    private CoinSucker coinSucker; // Reference to the CoinSucker



    private Player player;
    private ResourceManager resourceManager;
    public EnemySpawner enemySpawner; // Assign this in the Inspector

    void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        player = FindObjectOfType<Player>(); // Find the Player script in the scene
        coinSucker = FindObjectOfType<CoinSucker>(true);
        InitialUpdateText();
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

    private void InitialUpdateText() {
        burstFreezCostText.text = "BurstFreez:" + freezBurstCost + "Coins";
    }

    void UpdateCostTexts()
    {
        healthUpgradeCostText.text = "Max HP +50: " + healthUpgradeCost + " Coins";
        projectileUpgradeCostText.text = "Projectile Damage +20: " + projectileUpgradeCost + " Coins";
        coinSuckerUpgradeCostText.text = "Coin Sucker Power +2: " + coinSuckerUpgradeCost + " Coins";
        freezeDurationUpgradeCostText.text = "Freeze Duration +1s: " + freezeDurationUpgradeCost + " Coins";
        healUpgradeCostText.text = "Heal Amount +50: " + healUpgradeCost + " Coins";
        citizenSpawnUpgradeCostText.text = "+1 Citizen: " + citizenSpawnUpgradeCost + " Coins";
        enemySpawnUpgradeCostText.text = "+2 Enemy Spawn: " + enemySpawnUpgradeCost + " Coins";
        fireballWeaponCostText.text = "Fireball:"+ fireballCost + " Coins";
        boomerangWeaponCostText.text = "Boomerang:" + boomerangCost + " Coins";
       
        // New Warrior Gotchi Upgrades
        gotchiSpawnRateUpgradeCostText.text = "Warrior Spawn rate -1s: " + gotchiSpawnRateUpgradeCost + " Coins";
        gotchiMaxUpgradeCostText.text = "+1 Maximum Warrior: " + gotchiMaxIncreaseUpgradeCost + " Coins";
        gotchiHealthUpgradeCostText.text = "Increase Warrior Health by " + gotchiHealthUpgradeAmount + ": " + gotchiHealthUpgradeCost + " Coins";
        gotchiDamageUpgradeCostText.text = "Increase Warrior Damage by " + gotchiDamageUpgradeAmount + ": " + gotchiDamageUpgradeCost + " Coins";
    }

    private void AfterPurchase()
    {
        UpdateCostTexts();  // Update UI texts after each purchase
    }

    public void PurchaseGotchiHealthUpgrade()
    {
        if (CanPurchaseUpgrade(gotchiHealthUpgradeCost))
        {
            WarriorGotchi.IncreaseHealth(gotchiHealthUpgradeAmount);
            resourceManager.SubtractCoins(gotchiHealthUpgradeCost);
            gotchiHealthUpgradeCost += 1; // Increment cost for next upgrade
            AfterPurchase();
            Debug.Log("Warrior Gotchi health upgrade purchased. New health: " + WarriorGotchi.baseHealth);
        }
    }

    public void PurchaseGotchiDamageUpgrade()
    {
        if (CanPurchaseUpgrade(gotchiDamageUpgradeCost))
        {
            WarriorGotchi.IncreaseDamage(gotchiDamageUpgradeAmount);
            resourceManager.SubtractCoins(gotchiDamageUpgradeCost);
            gotchiDamageUpgradeCost += 1; // Increment cost for next upgrade
            AfterPurchase();
            Debug.Log("Warrior Gotchi damage upgrade purchased. New damage: " + WarriorGotchi.baseDamage);
        }
    }

    public void PurchaseGotchiSpawnRateUpgrade()
    {
        if (CanPurchaseUpgrade(gotchiSpawnRateUpgradeCost))
        {
            warriorGotchiSpawner.DecreaseSpawnInterval(1f); // Decrease interval by 1 second
            resourceManager.SubtractCoins(gotchiSpawnRateUpgradeCost);
            AfterPurchase();
            Debug.Log("Gotchi spawn rate upgrade purchased.");
        }
    }

    public void PurchaseGotchiMaxIncreaseUpgrade()
    {
        if (CanPurchaseUpgrade(gotchiMaxIncreaseUpgradeCost))
        {
            warriorGotchiSpawner.IncreaseMaxGotchis(1); // Increase max Gotchis by 1
            resourceManager.SubtractCoins(gotchiMaxIncreaseUpgradeCost);
            AfterPurchase();
            Debug.Log("Gotchi max increase upgrade purchased.");
        }
    }

    public Transform spawnPointTransform; // Assign this in the Unity Inspector
    public void PurchaseCitizenSpawnUpgrade()
    {
        if (CanPurchaseUpgrade(citizenSpawnUpgradeCost))
        {
            GameObject newCitizen = Instantiate(citizenPrefab, spawnPointTransform.position, Quaternion.identity);
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            newCitizen.transform.SetParent(playerGameObject.transform, false);

            // Set a random color hue and saturation
            SpriteRenderer spriteRenderer = newCitizen.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = RandomColorHue();
            }

            resourceManager.SubtractCoins(citizenSpawnUpgradeCost);
            citizenSpawnUpgradeCost += citizenSpawnUpgradeCostIncrease;
            AfterPurchase();
            Debug.Log("Citizen spawn upgrade purchased.");
        }
    }
    Color RandomColorHue()
    {
        float hue = Random.Range(0f, 1f); // Random hue from 0 to 1
        float saturation = Random.Range(0.5f, 1f); // Random saturation from 50% to 100%
        return Color.HSVToRGB(hue, saturation, 1); // Full value for brightness
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
    public void PurchaseBurstFreezeUpgrade()
    {
        if (CanPurchaseUpgrade(freezBurstCost))
        {
            player.supportThrower.supportProjectileIndex = 1;
            resourceManager.SubtractCoins(freezBurstCost);
            freezBurstCost = 0; // Increase the cost for the next upgrade
                                // AfterPurchase();
            burstFreezCostText.text = "Max";
            Debug.Log("Purchased burst freeze upgrade: ");
        }
    }


    public void PurchaseProjectileUpgrade()
    {
        if (CanPurchaseUpgrade(projectileUpgradeCost))
        {
            Projectile.projectileBonusDamage += projectileDamageIncrease;
            Projectile.UpdateDamageAmount();
            resourceManager.SubtractCoins(projectileUpgradeCost);
            projectileUpgradeCost += projectileUpgradeCostIncrease; // Increase the cost for the next upgrade
            AfterPurchase();
            Debug.Log("Projectile upgrade purchased. New damage: " + Projectile.damageAmount);
        }
    }

    public void PurchaseWeaponUpgrade(ProjectileType projectileType) {
        if (CanPurchaseUpgrade(GetWeaponUpgradeCost(projectileType))) {
            Projectile.weaponBonusDamage = GetWeaponDamageUpgrade(projectileType);
            Projectile.UpdateDamageAmount();
            resourceManager.SubtractCoins(GetWeaponUpgradeCost(projectileType));
            player.projectileThrower.projectileType = projectileType;
            UpdateWeaponUpgradeCost(projectileType);
            AfterPurchase();
            Debug.Log("Weapon purchased: " + projectileType);
        }
    }

    public void PurchaseCoinSuckerUpgrade()
    {
        if (coinSucker == null)
        {
            Debug.LogError("CoinSucker not found in the scene.");
            return;
        }

        if (resourceManager.Coins >= coinSuckerUpgradeCost)
        {
            coinSucker.SuckPower += coinSuckerPowerIncrease;
            resourceManager.SubtractCoins(coinSuckerUpgradeCost);
            coinSuckerUpgradeCost += coinSuckerUpgradeCostIncrease; // Increase the cost for the next upgrade
            UpdateCostTexts();
            Debug.Log("CoinSucker upgrade purchased. New Suck Power: " + coinSucker.SuckPower);
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

    private int GetWeaponUpgradeCost(ProjectileType projectileType) {
        switch (projectileType) {
            case ProjectileType.Fireball:
                return 0;
            case ProjectileType.Boomerang:
                return boomerangCost;
            
            default: return 0;
        }
    }
    private int GetWeaponDamageUpgrade(ProjectileType projectileType) {
        switch (projectileType) {
            case ProjectileType.Fireball:
                return fireballDamage;
            case ProjectileType.Boomerang:
                return boomerangDamage;
            default: return 0;
        }
    }

    private void UpdateWeaponUpgradeCost(ProjectileType projectileType) {
        switch (projectileType) {
            case ProjectileType.Fireball:
                fireballCost = 0;
                break;
            case ProjectileType.Boomerang:
                boomerangCost = 0;
                break;
            default:
                break;
        }
    }
}
