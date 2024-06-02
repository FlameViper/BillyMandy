using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ColoredEnemy : Enemy {
    [SerializeField] List<Sprite> spriteColors;
    [SerializeField] List<GameObject> coloredExplosionPrefabs;
    public string currentColor;
    public Coroutine destroyColoredEnemyCoroutine;
    private bool deathByExplosion;

    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        int random = Random.Range(0, 4);
        PickColor(random);
    }
    protected override void Start() {
        base.Start();
    

    }

    protected override void Update() {
        base.Update();
    }

    protected override void UpdateTarget() {
        if (potentialTargets.Count == 0) {
            target = player;  // Default back to player if no gotchis are close
            return;
        }
        Debug.Log("here2");
        Transform closest = null;
        float minDistance = float.MaxValue;
        foreach (Transform t in potentialTargets) {
            if (t != null) {
                float dist = Vector2.Distance(transform.position, t.position);
                if (dist < minDistance) {
                    closest = t;
                    minDistance = dist;
                }
            }
            else {
                Debug.Log(t + "null");

            }
        }
        Debug.Log(closest.name + "closest");
        if (closest != null && closest != target) {
            Debug.Log("here3");
            target = closest;
            if (attackRoutine != null) {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
        }
        attackRoutine ??= StartCoroutine(DealDamageRepeatedly(target.GetComponent<Collider2D>()));
    }




    public void PickColor(int random) {
        switch (random) {
            case 0:
                currentColor = "Red";
                spriteRenderer.sprite = spriteColors[0];
                break;
            case 1:
                currentColor = "Green";
                spriteRenderer.sprite = spriteColors[1];
                break;
            case 2:
                currentColor = "Yellow";
                spriteRenderer.sprite = spriteColors[2];
                break;
            case 3:
                currentColor = "Purple";
                spriteRenderer.sprite = spriteColors[3];
                break;
            default:
                break;
        }
    }
    public override void TakeDamage(int damage, bool isExplosionDmg) {
        // Ignore damage if already dead
        if (isDead) return;
        if (!isFrozen || SupportThrower.Instance.canDamageFrozenEnemies) {
            currentHealth -= damage;
            DisplayDamage(damage, transform.position);

        }
        // Play damage sound effect
        if (damageSound != null) {
            damageSound.Play();
        }

        if (currentHealth <= 0) {
            if (isExplosionDmg) {
                deathByExplosion = true;
            }
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
        if (!deathByExplosion) {
            Instantiate(GetColorExplosion(), transform.position, Quaternion.identity);

        }
        Instantiate(coinPrefab, transform.position, Quaternion.identity);

        StartCoroutine(FadeOut(1f)); // Fade out over 1 second
    }



    public GameObject GetColorExplosion() {
        switch (currentColor) {
            case "Red":
                return coloredExplosionPrefabs[0];
            case "Green":
                return coloredExplosionPrefabs[1];
            case "Yellow":
                return coloredExplosionPrefabs[2];
            case "Purple":
                return coloredExplosionPrefabs[3];
            default:
                return coloredExplosionPrefabs[0];
        }
    }
    //public void DestroyColoredEnemy() {
    //    destroyColoredEnemyCoroutine ??= StartCoroutine(DestroyAfterDealy());
    //}

    //private IEnumerator DestroyAfterDealy() {
    //    isDieing = true;
    //    moveSpeed = 0;
    //    yield return new WaitForSeconds(0.2f);
    //    if (!deathByExplosion) {
    //        Instantiate(GetColorExplosion(), transform.position, Quaternion.identity);

    //    }
    //    if (gameObject != null) {
    //        Destroy(gameObject);
    //    }
    //}
}
