using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFreezableDestructableProjectile : EnemyProjectile {
    [SerializeField] protected bool destroysPlayerBulletsAlso;
    [SerializeField] protected int maxHealth = 10;
    [SerializeField] protected int currentHealth;
    private bool isFrozen;
    SpriteRenderer spriteRenderer;
    Color baseColor;
    Coroutine freezCorutine;
    protected override void Start() {
        base.Start();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor= spriteRenderer.color;
    }

    protected override void Update() {
        if(isFrozen) {
           
            return;
        }
        else if(!isFrozen) {
      
            base.Update();
        }
    }


    protected override void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            if (player != null) {
                player.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("PlayerProjectile")) {
            SupportProjectile supportProjectile;
            if (other.TryGetComponent<SupportProjectile>(out supportProjectile)) {
                    Debug.Log("1");
                if (!isFrozen) {
                    freezCorutine ??= StartCoroutine(Freeze());
                }
                else {
                    Debug.Log("3");
                    StopCoroutine(freezCorutine);
                    freezCorutine ??= StartCoroutine(Freeze());
                }
                return;
            }
            if (destroysPlayerBulletsAlso) {
                Destroy(other.gameObject);
            }
            currentHealth -= Projectile.damageAmount;
            if (currentHealth <= 0) {
                Destroy(gameObject);
            }
        }
    }


    private IEnumerator Freeze() {
        
        isFrozen = true;
        Debug.Log("4");
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(SupportProjectile.freezeDuration);
        isFrozen = false;
        spriteRenderer.color = baseColor;
        freezCorutine = null;
    }
}
