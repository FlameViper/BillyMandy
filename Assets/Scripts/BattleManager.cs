using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Include this to use the Text component

public class BattleManager : MonoBehaviour
{
    // Static singleton property
    public static BattleManager Instance { get; private set; }

    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public float roundTimeLimit = 60f; //I AM EDITING YOUR FILE
    public Text levelText; // UI Text element for displaying the level
    public Text livesDuringBossText;

    private bool isRoundActive = false;
    public int level = 1; // Level counter
    public bool isBossLevel;
    public bool playerHasScoreAsHpShield;
    public bool hasChoosen;
    [SerializeField] GameObject bossChoice;
    void Awake()   // I AM EDITING YOUR FILE
    {
        if (Instance != null && Instance != this)       //     I AM EDITING YOUR FILE
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

        //if (level == EnemySpawner.Instance.bossSpawningLevel && !hasChoosen) {
      
        //    livesDuringBossText.gameObject.SetActive(true);
        //    Player.Instance.healthWhenFightingTheBoss = Player.Instance.currentHealth;
        //    livesDuringBossText.text = "Lives left:" + Player.Instance.numberOfLifesLeft.ToString();
        //    bossChoice.SetActive(true);
        //    return;
        //}
        //else {
    
        //    playerHasScoreAsHpShield = false;
        //    livesDuringBossText.gameObject.SetActive(false);
        //    bossChoice.SetActive(false);
        //}
        
       
        isRoundActive = true;
        roundTimeLimit = 60;
        uiManager.EnableBattleCamera();
        enemySpawner.StartSpawning(level); // Pass current level to spawner
        //if (isBossLevel) {
        //    livesDuringBossText.gameObject.SetActive(true);
        //    Player.Instance.healthWhenFightingTheBoss = Player.Instance.currentHealth;
        //    livesDuringBossText.text = "Lives left:" + Player.Instance.numberOfLifesLeft.ToString();
        //}
        //else {
        //    playerHasScoreAsHpShield = false;
        //    livesDuringBossText.gameObject.SetActive(false);

        //}
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
    //private void Start() {
    //    if (isBossLevel) {
    //        Player.Instance.healthWhenFightingTheBoss = Player.Instance.currentHealth;
    //    }
    //}

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
        //if (hasChoosen) {
        //    StartRound();
        //    return;
        //}
        if(level != EnemySpawner.Instance.bossSpawningLevel || hasChoosen) {
            uiManager.EnableMainCamera();
            StartRound(); // Start new round with incremented level
            playerHasScoreAsHpShield = false;
            livesDuringBossText.gameObject.SetActive(false);
            bossChoice.SetActive(false);
        }
        else if(level == EnemySpawner.Instance.bossSpawningLevel && !hasChoosen) {
            livesDuringBossText.gameObject.SetActive(true);
            Player.Instance.healthWhenFightingTheBoss = Player.Instance.currentHealth;
            ResourceManager.Instance.UpdateBossfightScore();
            livesDuringBossText.text = "Lives left:" + Player.Instance.numberOfLifesLeft.ToString();
            bossChoice.SetActive(true);
        }
        if (level != EnemySpawner.Instance.bossSpawningLevel) {
            hasChoosen = false;
        }
    }

    void UpdateLevelDisplay()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
        else
            Debug.LogError("Level text component is not assigned in the BattleManager.");
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
}
