using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] private int explosionDamage = 30;
    [SerializeField] private bool canDamagePlayer = true;
    [SerializeField] private bool canDamageEnemies = false;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (canDamagePlayer) {
            if (collision.CompareTag("Player")) {
                Player.Instance.TakeDamage(explosionDamage);
            }
        }
        if (canDamageEnemies) {
            if (collision.CompareTag("Enemy")) {
                collision.GetComponent<Enemy>().TakeDamage(explosionDamage,true);
            }
        }
    }
    public void OnExplosionEnd() {
        Destroy(gameObject);
    }
}
