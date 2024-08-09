using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : Enemy
{
    [SerializeField] private GameObject explosionPrefabPlayerOnly;
    [SerializeField] private GameObject explosionPrefabEnemiesOnly;
    [SerializeField] private GameObject explosionPrefabEverything;
    [SerializeField] private GameObject explosionOnHitObject;
    [SerializeField] private float explostionRange = 1f; // Range to trigger explosion
    [SerializeField] private bool canDamagePlayer = true;
    [SerializeField] private bool canDamageEnemies = false;
    [SerializeField] private bool explodesOnDeath = true;
    [SerializeField] private bool explodesOnHit = false;
    private Coroutine explosionCorutine;

    protected override void Start() {
        base.Start();

    }
    protected override void Update() {
        base.Update();
       
        if (Vector3.Distance(transform.position, player.position) <= explostionRange) {  
            if(canDamagePlayer) {
                SelfDestruct(); 

            }
        }
    }
 
    void SelfDestruct() {
        Die();
    }

    public override void TakeDamage(int damage,bool isExplosionDmg) {
        // Ignore damage if already dead
        if (isDead) return;

        if (!isFrozen || SupportThrower.Instance.canDamageFrozenEnemies) {
            currentHealth -= damage;
            DisplayDamage(damage, transform.position);

        }
        // Play damage sound effect
        if (enemyOnHitSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            //Debug.Log("played");
            soundManager.CreateSound().WithSoundData(enemyOnHitSoundData).WithPosition(transform.position).Play();

        }

        if (currentHealth <= 0) {
            Die();
        }

        if (explodesOnHit && !isExplosionDmg) {
            if (explosionCorutine == null) {
                explosionCorutine = StartCoroutine(HandleExplosionOnHit());
            }          
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
        if (enemyOnDeathSoundData.clip != null && !GameSettings.Instance.SFXOFF) {
            //Debug.Log("played");
            soundManager.CreateSound().WithSoundData(enemyOnDeathSoundData).WithPosition(transform.position).Play();

        }

        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager != null) {
            resourceManager.AddScore(100);
        }

        if (explodesOnDeath) {
            HandleExplosion();
        }

        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        StartCoroutine(FadeOut(1f));

    }

    private void HandleExplosion() {
        (bool canDamagePlayer, bool canDamageEnemies) condition = (this.canDamagePlayer, this.canDamageEnemies);

        switch (condition) {
            case (true, true):
                Instantiate(explosionPrefabEverything, transform.position, Quaternion.identity);
                break;
            case (true, false):
                Instantiate(explosionPrefabPlayerOnly, transform.position, Quaternion.identity);
                break;
            case (false, true):
                Instantiate(explosionPrefabEnemiesOnly, transform.position, Quaternion.identity);
                break;
            default:
                // Handle default case
                break;
        }
    }
    private IEnumerator HandleExplosionOnHit() {
        explosionOnHitObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        explosionOnHitObject.SetActive(false);
        explosionCorutine = null;
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

}
