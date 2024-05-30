using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestructableProjectile : EnemyProjectile {
    [SerializeField] protected bool destroysPlayerBulletsAlso;
    [SerializeField] protected int maxHealth=10;
    [SerializeField] protected int currentHealth;
    protected override void Start() {
        base.Start();
        currentHealth = maxHealth;
    }

    protected override void Update() {
        base.Update();
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
            if(destroysPlayerBulletsAlso) {
                Destroy(other.gameObject);
            }
            currentHealth -= Projectile.damageAmount;
            if(currentHealth <= 0) {
                Destroy(gameObject);
            }
        }
    }


}
