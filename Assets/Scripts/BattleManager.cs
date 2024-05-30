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

    private bool isRoundActive = false;
    public int level = 1; // Level counter
    public bool isBossLevel;

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

    IEnumerator WaitAndMoveCamera()
    {
        // Wait for 3 seconds (you can change this value to any duration you want)
        yield return new WaitForSeconds(3f);

        // Move the camera after the wait
        uiManager.EnableUpgradesCamera();
    }


    public void OnUpgradesDone()
    {
        uiManager.EnableMainCamera();
        StartRound(); // Start new round with incremented level
    }

    void UpdateLevelDisplay()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
        else
            Debug.LogError("Level text component is not assigned in the BattleManager.");
    }
}
