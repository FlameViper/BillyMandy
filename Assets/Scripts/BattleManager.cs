using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Include this to use the Text component

public class BattleManager : MonoBehaviour
{
    // Static singleton property
    public static BattleManager Instance { get; private set; }

    public event EventHandler OnUpgradesDoneEvent;

    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public float roundTimeLimit = 60f; //I AM EDITING YOUR FILE
    public Text levelText; // UI Text element for displaying the level
    public TextMeshProUGUI TDLevelText; // UI Text element for displaying the level
    public Text livesDuringBossText;

    private bool isRoundActive = false;
    public int level = 1; // Level counter
    public bool isBossLevel;
    public bool playerHasScoreAsHpShield;
    public bool hasChoosen;
    [SerializeField] GameObject bossChoice;
    [SerializeField] GameObject nightLayer;
    [SerializeField] Transform playerNormalPosition;
    [SerializeField] Transform playerTowerDefensePosition;
    void Awake()  
    {
        if (Instance != null && Instance != this)      
        {
            Destroy(this.gameObject) ; 
        }
        else
        {
            Instance = this;
        }
    }

    void StartRound()
    {    
        isRoundActive = true;
        roundTimeLimit = 60;
        uiManager.EnableBattleCamera();
        enemySpawner.StartSpawning(level); // Pass current level to spawner
        UpdateAllStealers(level);
        UpdateLevelDisplay(); // Update the level display at the start of each round
    }

    void UpdateAllStealers(int level)
    {
        foreach (CoinStealer stealer in FindObjectsOfType<CoinStealer>())
        {
            stealer.UpdateAttractionPower(level);
        }
    }


    void Update()
    {
        if (isRoundActive)
        {
            if (!isBossLevel) {

                roundTimeLimit -= Time.deltaTime;

            }

            if (roundTimeLimit <= 0)
            {
                EndRound();
            }
        }
    }


    public void EndRound()
    {
        isRoundActive = false;
        enemySpawner.StopSpawning();
        enemySpawner.DestroyAllEnemies();

        level++; // Increment level

        // Start the coroutine to wait a few seconds before moving the camera
        StartCoroutine(WaitAndMoveCamera());
    }

    public void RestartRound() {
        isRoundActive = false;
        enemySpawner.StopSpawning();
        enemySpawner.DestroyAllEnemies();


        // Start the coroutine to wait a few seconds before moving the camera
        StartCoroutine(WaitAndMoveCamera());
    }

    IEnumerator WaitAndMoveCamera()
    {
        // Wait for 3 seconds (you can change this value to any duration you want)
        yield return new WaitForSeconds(3f);

        // Move the camera after the wait
        uiManager.EnableUpgradesCamera();
    }


    public void OnUpgradesDone()
    {
        if (level != EnemySpawner.Instance.bossSpawningLevel && level % 10 != 0) {
            if(level % 5 == 0) {
                nightLayer.gameObject.SetActive(true);
            }
            else {
                nightLayer.gameObject.SetActive(false);
            }
            TowerDefenseManager.Instance.isInPreparationPhase = false;
            Player.Instance.transform.position = playerNormalPosition.position;
            uiManager.EnableMainCamera();
            StartRound(); 
            playerHasScoreAsHpShield = false;
            livesDuringBossText.gameObject.SetActive(false);
            bossChoice.SetActive(false);
            hasChoosen = false;
        }
        else if(level == EnemySpawner.Instance.bossSpawningLevel && !hasChoosen) {
            nightLayer.gameObject.SetActive(false);         
            TowerDefenseManager.Instance.isInPreparationPhase = false;
            Player.Instance.transform.position = playerNormalPosition.position;
            livesDuringBossText.gameObject.SetActive(true);
            Player.Instance.healthWhenFightingTheBoss = Player.Instance.currentHealth;
            ResourceManager.Instance.UpdateBossfightScore();
            UpdateLivesText();
            bossChoice.SetActive(true);
        }
        else if (hasChoosen && level == EnemySpawner.Instance.bossSpawningLevel) {
            nightLayer.gameObject.SetActive(false);
            TowerDefenseManager.Instance.isInPreparationPhase = false;
            Player.Instance.transform.position = playerNormalPosition.position;
            uiManager.EnableMainCamera();
            StartRound();
            bossChoice.SetActive(false);
            UpdateLivesText();
        }
        else if (level != EnemySpawner.Instance.bossSpawningLevel && level % 10 == 0) {
            nightLayer.gameObject.SetActive(false);
            Player.Instance.transform.position = playerTowerDefensePosition.position;
            TowerDefenseManager.Instance.isInPreparationPhase = true;
            uiManager.EnableTowerDefenseCamera();
            playerHasScoreAsHpShield = false;
            livesDuringBossText.gameObject.SetActive(false);
            bossChoice.SetActive(false);
            hasChoosen = false;

        }
        OnUpgradesDoneEvent?.Invoke(this, EventArgs.Empty);
        UpdateLevelDisplay();
    }

    public void UpdateLevelDisplay()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
        if (TDLevelText != null)
            TDLevelText.text = "Level: " + level;
        //else
        //    Debug.LogError("Level text component is not assigned in the BattleManager.");
    }

    public void SetPlayerHasScoreAsHpShield() {
        playerHasScoreAsHpShield = true;
        hasChoosen = true;
     
        StartRound();
        bossChoice.SetActive(false);
    }
    public void SetOffPlayerHasScoreAsHpShield() {
        playerHasScoreAsHpShield = false;
        hasChoosen = true;
        StartRound();
        bossChoice.SetActive(false);
    }

    public void UpdateLivesText() {
        livesDuringBossText.text = "Lives:" + Player.Instance.numberOfLivesDuringBoss.ToString();
    }
}
