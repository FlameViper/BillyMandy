using System.Collections.Generic;
using UnityEngine;

public class WarriorGotchiSpawner : MonoBehaviour
{
    public GameObject warriorGotchiPrefab; // Assign this in the Unity Inspector
    public float spawnInterval = 10f; // Time in seconds between each spawn
    public int maxGotchis = 5; // Maximum number of WarriorGotchis allowed at once
    public Transform spawnPoint; // Assign a spawn point in the Unity Inspector

    private float nextSpawnTime = 0f;
    private int currentGotchis = 0;

    private List<WarriorGotchi> activeGotchis = new List<WarriorGotchi>();

    void Update()
    {
        // Check if it's time to spawn a new WarriorGotchi and if the current count is below the maximum
        if (Time.time >= nextSpawnTime && currentGotchis < maxGotchis)
        {
            SpawnWarriorGotchi();
            nextSpawnTime = Time.time + spawnInterval; // Set the next spawn time
        }
    }

    void SpawnWarriorGotchi()
    {
        if (currentGotchis < maxGotchis)
        {
            GameObject newGotchiObj = Instantiate(warriorGotchiPrefab, spawnPoint.position, Quaternion.identity, transform);
            WarriorGotchi newGotchi = newGotchiObj.GetComponent<WarriorGotchi>();
            activeGotchis.Add(newGotchi);
            newGotchi.OnDestroyAction += () => {
                activeGotchis.Remove(newGotchi);
                currentGotchis--;
                currentGotchis = Mathf.Max(currentGotchis, 0);
            };
            currentGotchis++;
        }
    }


public void IncreaseMaxGotchis(int amount)
    {
        maxGotchis += amount; // Method to increase the maximum number of WarriorGotchis
    }

    public void DecreaseSpawnInterval(float amount)
    {
        spawnInterval = Mathf.Max(spawnInterval - amount, 1f); // Decrease the spawn interval, minimum of 1 second
    }

    // Call this when a WarriorGotchi is destroyed
    public void WarriorGotchiDestroyed()
    {
        currentGotchis = Mathf.Max(currentGotchis - 1, 0); // Decrement the count of active WarriorGotchis
    }
}
