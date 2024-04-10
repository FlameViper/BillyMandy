using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    // spawnInterval is now set dynamically in StartSpawning
    private float spawnInterval;
    public int maxEnemies = 5;

    private float spawnTimer;
    private Vector3 spawnAreaSize;
    private int enemiesSpawned = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();

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
        float levelDuration = 60f;

        // Adjust the spawn interval based on the number of enemies and the level duration
        // Ensure we adjust for a very high number of enemies to maintain game balance
        if (maxEnemies > 0)
        {
            spawnInterval = Mathf.Max(levelDuration / maxEnemies, 0.25f); // Ensures a minimum spawn interval of 0.25 seconds
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
        while (enemiesSpawned < maxEnemies)
        {
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;

                Vector3 randomSpawnPosition = transform.position + new Vector3(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                    0);

                GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                GameObject newEnemy = Instantiate(selectedPrefab, randomSpawnPosition, Quaternion.identity);
                currentEnemies.Add(newEnemy); // Keep track of the spawned enemy

                // Adjust the enemy's health based on the current level
                newEnemy.GetComponent<Enemy>().SetHealth(level); // Make sure your Enemy class has SetHealth method

                enemiesSpawned++;
            }
            spawnTimer += Time.deltaTime;
            yield return null; // Wait until the next frame to continue
        }
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
