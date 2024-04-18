using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Static singleton property
    public static BattleManager Instance { get; private set; }

    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public float roundTimeLimit = 60f;

    private bool isRoundActive = false;
    public int level = 1; // Level counter

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
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
            roundTimeLimit -= Time.deltaTime;

            if (roundTimeLimit <= 0)
            {
                EndRound();
            }
        }
    }


    void EndRound()
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
}
