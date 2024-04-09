using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // List of enemy prefabs to be spawned
    public float spawnInterval = 3f; // Interval between enemy spawns
    public int maxEnemies = 5; // Maximum number of enemies this spawner will create

    private float spawnTimer; // Timer for tracking spawn intervals
    private Vector3 spawnAreaSize; // Size of the spawn area
    private int enemiesSpawned = 0; // Counter for the number of enemies this spawner has spawned

    void Start()
    {
        // Get the size of the spawn area
        spawnAreaSize = GetComponent<Renderer>().bounds.size;
    }

    void Update()
    {
        // If this spawner has already spawned the max number of enemies, do nothing further
        if (enemiesSpawned >= maxEnemies)
        {
            return;
        }

        // Update the spawn timer
        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn a new enemy
        if (spawnTimer >= spawnInterval && enemiesSpawned < maxEnemies)
        {
            // Reset the timer for the next spawn
            spawnTimer = 0f;

            // Spawn a new enemy at a random position within the spawn area
            Vector3 randomSpawnPosition = transform.position + new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                0); // Assuming a 2D game; adjust z if necessary

            // Randomly select an enemy prefab from the list
            GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Instantiate the selected enemy prefab at the calculated position
            Instantiate(selectedPrefab, randomSpawnPosition, Quaternion.identity);

            // Increment the count of enemies spawned by this spawner
            enemiesSpawned++;
        }
    }
}
