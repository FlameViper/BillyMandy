using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezTowerProjectile : MonoBehaviour {
    [SerializeField] private float speed = 10f; // Speed of the projectile
    [SerializeField] private int damageAmount = 0;
    [SerializeField] private float destroyDelay = 5f;
    [SerializeField] private bool destoryedOnEnemyInpact = true;
    private Transform target;


    private void Update() {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        // Check if the collider is an enemy
        if (other.CompareTag("Enemy")) {
            // Get the Enemy component from the collided object
            Enemy enemy = other.GetComponent<Enemy>();

            // If the enemy component exists, apply damage
            if (enemy != null) {


                enemy.Freeze(false);

                if (destoryedOnEnemyInpact) {
                    Destroy(gameObject);
                }
            }
        }
        else if (other.CompareTag("Border")) {
            Destroy(gameObject);
        }

    }
    public void SetTarget(Transform transform) {
        target = transform;
    }
}
