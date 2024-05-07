using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    // spawnInterval is now set dynamically in StartSpawning
    private float spawnInterval;
    public int maxEnemies = 5;
    public GameObject specialEnemyPrefab; // This is the CoinStealer prefab

    public int specialEnemyStartLevel = 5;
    public int specialEnemyInterval = 1; // Spawns an extra CoinStealer every 5 levels past level 5


    private float spawnTimer;
    private Vector3 spawnAreaSize;
    private int enemiesSpawned = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();

    public GameObject bossEnemyPrefab; // Boss enemy prefab
    private bool shouldSpawnBossNextRound = false;

    public bool ShouldSpawnBossNextRound
    {
        get { return shouldSpawnBossNextRound; }
        private set { shouldSpawnBossNextRound = value; }
    }

    public void PrepareBossSpawn()
    {
        ShouldSpawnBossNextRound = true;
    }


    void Start()
    {
        spawnAreaSize = GetComponent<Renderer>().bounds.size;
    }

    public void StartSpawning(int level)
    {
        enemiesSpawned = 0;
        spawnTimer = 0;

        // Calculate and adjust spawn interval based on maxEnemies and level duration
        AdjustSpawnInterval();

        StartCoroutine(SpawnEnemies(level)); // Start coroutine with level parameter
    }

    private void AdjustSpawnInterval()
    {
        // Assuming a level duration of 60 seconds
        float levelDuration = 40f;

        // Adjust the spawn interval based on the number of enemies and the level duration
        // Ensure we adjust for a very high number of enemies to maintain game balance
        if (maxEnemies > 0)
        {
            spawnInterval = Mathf.Max(levelDuration / maxEnemies, 0.01f); // Ensures a minimum spawn interval of 0.25 seconds
        }
        else
        {
            // Default to 3 seconds if maxEnemies is somehow not set properly
            spawnInterval = 3f;
        }
    }

    public void IncreaseMaxEnemies(int increaseAmount)
    {
        maxEnemies += increaseAmount;
        // Optionally re-adjust spawn interval if maxEnemies changes mid-level
        // AdjustSpawnInterval();
    }

    IEnumerator SpawnEnemies(int level)
    {
        if (shouldSpawnBossNextRound)
        {
            GameObject boss = Instantiate(bossEnemyPrefab, CalculateSpawnPosition(), Quaternion.identity);
            currentEnemies.Add(boss);  // Add boss to the list of current enemies
            shouldSpawnBossNextRound = false; // Reset the flag after spawning
        }

        int specialEnemyCount = 0;
        if (level >= specialEnemyStartLevel)
        {
            specialEnemyCount = 1 + (level - specialEnemyStartLevel) / specialEnemyInterval;
        }

        while (enemiesSpawned < maxEnemies)
        {
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                Vector3 randomSpawnPosition = CalculateSpawnPosition();

                GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                if (specialEnemyCount > 0 && Random.Range(0, maxEnemies) < specialEnemyCount)
                {
                    selectedPrefab = specialEnemyPrefab;
                    specialEnemyCount--; // Decrement after choosing to spawn a special enemy
                }

                GameObject newEnemy = Instantiate(selectedPrefab, randomSpawnPosition, Quaternion.identity);
                currentEnemies.Add(newEnemy);

                // Check for the Enemy component
                Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.SetHealth(level);
                }

                // Check for the CoinStealer component and call SetHealth if it exists
                CoinStealer coinStealerComponent = newEnemy.GetComponent<CoinStealer>();
                if (coinStealerComponent != null)
                {
                    coinStealerComponent.SetHealth(level);
                }

                enemiesSpawned++;
            }
            spawnTimer += Time.deltaTime;
            yield return null;
        }
    }

    private Vector2 CalculateSpawnPosition()
    {
        return transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            0);
    }



    public void StopSpawning()
    {
        StopAllCoroutines(); // Stops the spawning coroutine
        spawnTimer = spawnInterval; // Resets spawn timer for immediate effect upon restart
    }

    public void DestroyAllEnemies()
    {
        foreach (var enemy in currentEnemies)
        {
            Destroy(enemy);
        }
        currentEnemies.Clear();
    }
}
