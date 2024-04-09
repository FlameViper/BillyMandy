using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float spawnInterval = 3f;
    public int maxEnemies = 5;

    private float spawnTimer;
    private Vector3 spawnAreaSize;
    private int enemiesSpawned = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();

    void Start()
    {
        spawnAreaSize = GetComponent<Renderer>().bounds.size;
        // Removed auto-start to give control to BattleManager
    }

    public void StartSpawning(int level)
    {
        enemiesSpawned = 0;
        spawnTimer = 0;
        StartCoroutine(SpawnEnemies(level)); // Start coroutine with level parameter
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

                // Correctly select a random prefab from the enemyPrefabs list
                GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                GameObject newEnemy = Instantiate(selectedPrefab, randomSpawnPosition, Quaternion.identity);
                currentEnemies.Add(newEnemy); // Keep track of the spawned enemy

                // Adjust the enemy's health based on the level
                var enemy = newEnemy.GetComponent<Enemy>(); // Try to get the Enemy component
                if (enemy != null)
                {
                    enemy.SetHealth(level); // If it's a regular enemy, set its health based on the level
                }
                else
                {
                    var enemyShooter = newEnemy.GetComponent<EnemyShooter>(); // Try to get the EnemyShooter component
                    if (enemyShooter != null)
                    {
                        enemyShooter.SetHealth(level); // If it's an enemy shooter, set its health based on the level
                    }
                }

                enemiesSpawned++;
            }
            spawnTimer += Time.deltaTime;
            yield return null; // Wait until the next frame to continue
        }
    }


    public void StopSpawning()
    {
        StopAllCoroutines(); // Stops the spawning coroutine
        spawnTimer = spawnInterval; // Ensures immediate spawning upon restart
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
