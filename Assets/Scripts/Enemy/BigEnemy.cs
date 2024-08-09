using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : Enemy {
    [SerializeField] private GameObject spawnedEnemyPrefab;


    protected override void Die() {
        if (isDead) return; // Prevent multiple deaths
        isDead = true; // Mark as dead

        // Disable all colliders on the enemy
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders) {
            collider.enabled = false;
        }

        // Play death sound effect
        if (enemyOnDeathSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            //Debug.Log("played");
            soundManager.CreateSound().WithSoundData(enemyOnDeathSoundData).WithPosition(transform.position).Play();

        }

        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null) {
            resourceManager.AddScore(100);
        }

        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Instantiate(spawnedEnemyPrefab, transform.position, Quaternion.identity);
        Instantiate(spawnedEnemyPrefab, transform.position + new Vector3(1, 0.5f), Quaternion.identity);
        Instantiate(spawnedEnemyPrefab, transform.position + new Vector3(1, 1), Quaternion.identity);
        Instantiate(spawnedEnemyPrefab, transform.position + new Vector3(0.5f, 1), Quaternion.identity);
        Instantiate(spawnedEnemyPrefab, transform.position + new Vector3(1.5f, 1), Quaternion.identity);

        StartCoroutine(FadeOut(1f)); // Fade out over 1 second
    }

    
}
