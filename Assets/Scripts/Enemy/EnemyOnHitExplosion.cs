using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnHitExplosion : MonoBehaviour
{
    [SerializeField] private int explosionDamage = 30;
    [SerializeField] private bool canDamageEnemies = true;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (canDamageEnemies) {
            if (collision.CompareTag("Enemy")) {
                collision.GetComponent<Enemy>().TakeDamage(explosionDamage, true);
            }
        }
    }
}
