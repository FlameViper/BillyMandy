using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public float roundTimeLimit = 10f;

    private bool isRoundActive = false;
    public int level = 1; // Level counter

    void Start()
    {
        StartRound();
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

    public void StartRound()
    {
        isRoundActive = true;
        roundTimeLimit = 60f;

        uiManager.EnableBattleCamera();
        enemySpawner.StartSpawning(level); // Pass current level to spawner
    }

    void EndRound()
    {
        isRoundActive = false;
        enemySpawner.StopSpawning();
        enemySpawner.DestroyAllEnemies();

        level++; // Increment level
        uiManager.EnableUpgradesCamera();
    }

    public void OnUpgradesDone()
    {
        uiManager.EnableMainCamera();
        StartRound(); // Start new round with incremented level
    }
}
