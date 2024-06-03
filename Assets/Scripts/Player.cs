using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public int maxHealth = 100; // Maximum health of the player
    public int currentHealth; // Current health of the player
    [SerializeField] int dmgToScore=100; // Current health of the player
    public GameObject gameOverScreen; // Drag your Game Over UI GameObject here in the inspector
    public Text healthText; // Drag your health Text UI component here in the inspector
    public AudioSource deathSound; // Reference to the AudioSource component that plays the death sound
    private bool isDead = false; // Flag to prevent multiple death sequences
    //projectile ref
    public ProjectileThrower projectileThrower;
    public CoinSucker coinSucker;
    public SupportThrower supportThrower;
    public int numberOfLivesDuringBoss = 3;
    public int healthWhenFightingTheBoss;

    private void Awake() {

        if (Instance == null) {
            Instance = this;

        }
        else {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        currentHealth = maxHealth;
        Projectile.UpdateDamageAmount();
        gameOverScreen.SetActive(false); // Ensure the Game Over screen is hidden at start
        UpdateHealthUI(); // Initial update of health display
    }

    void Update()
    {
        UpdateHealthUI(); // Continuously update health display
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // If already dead, do nothing
        if (BattleManager.Instance.playerHasScoreAsHpShield && ResourceManager.Instance.Score > 0) {
            ResourceManager.Instance.AddScore(-dmgToScore);
        }
        else {
            currentHealth -= damage;

        }
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true; // Set isDead to true to prevent multiple calls
            deathSound.Play(); // Play the death sound
            StartCoroutine(HandleDeath()); // Start the coroutine to handle death
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "HP " + currentHealth + "/" + maxHealth; // Displays as 'currentHealth/maxHealth', e.g., '50/100'
    }

    IEnumerator HandleDeath()
    {
        Debug.Log("Player Died!");
        gameOverScreen.SetActive(true); // Show the Game Over screen
        numberOfLivesDuringBoss--;
        BattleManager.Instance.UpdateLivesText();
        yield return new WaitForSeconds(5); // Wait for 5 seconds
        if (BattleManager.Instance.isBossLevel && numberOfLivesDuringBoss>0) {
            isDead = false;
            currentHealth = healthWhenFightingTheBoss;
            healthText.text = currentHealth.ToString();
            gameOverScreen.SetActive(false);
            ResourceManager.Instance.ResetBossFightScore();
            BattleManager.Instance.RestartRound();
            yield break;
        }
        Time.timeScale = 0; // Reload the scene
    }
    public bool GetSuckerStatus() {
        return coinSucker.isSuckerActive;
    }

}
