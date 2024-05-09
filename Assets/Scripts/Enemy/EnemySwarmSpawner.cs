using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmSpawner : Enemy
{
    [SerializeField] private GameObject swarmEnemyPrefab;
    [SerializeField] private float enemyHitsTaken=5;
    protected override void Start() {
        base.Start();

    }
    protected override void Update() {
       
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Limit")) {
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) {
     
    }

    protected override void OnCollisionStay2D(Collision2D collision) {
   

    }
    public override void TakeDamage(int damage, bool isExplosionDmg) {
        // Ignore damage if already dead
        if (isDead) return;

        enemyHitsTaken --;
        DisplayDamage(1, transform.position);
        // Play damage sound effect
        if (damageSound != null) {
            damageSound.Play();
        }

        if (enemyHitsTaken <= 0) {
            Die();
        }
    }

    protected override void Die() {
        if (isDead) return; // Prevent multiple deaths
        isDead = true; // Mark as dead

        // Disable all colliders on the enemy
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders) {
            collider.enabled = false;
        }

        // Play death sound effect
        if (deathSound != null) {
            deathSound.Play();
        }

        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null) {
            resourceManager.AddScore(100);
        }

        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Instantiate(swarmEnemyPrefab, transform.position, Quaternion.identity);
        Instantiate(swarmEnemyPrefab, transform.position + new Vector3(1, 0.5f), Quaternion.identity);
        Instantiate(swarmEnemyPrefab, transform.position + new Vector3(1, 1), Quaternion.identity);
        Instantiate(swarmEnemyPrefab, transform.position + new Vector3(0.5f, 1), Quaternion.identity);
        Instantiate(swarmEnemyPrefab, transform.position + new Vector3(1.5f, 1), Quaternion.identity);

        StartCoroutine(FadeOut(1f)); // Fade out over 1 second
    }
}
