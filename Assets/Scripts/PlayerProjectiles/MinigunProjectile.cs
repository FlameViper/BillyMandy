using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunProjectile : Projectile {

    [SerializeField] float bulletSpread;

    protected override void Awake() {
        base.Awake();
      
    }
    protected override void Start() {
        base.Start();
        // Rotate the sprite to face the direction of movement
        transform.rotation = Quaternion.Euler(0, 0, GetAngleFromVectorFloat(direction));
        SetBulletSpread(direction);
    }

    protected override void Update() {

        // Move the projectile in its direction
        transform.position += direction * speed * Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        // Check if the collider is an enemy
        if (other.CompareTag("Enemy")) {
            // Get the Enemy component from the collided object
            Enemy enemy = other.GetComponent<Enemy>();

            // If the enemy component exists, apply damage
            if (enemy != null) {
                enemy.TakeDamage(damageAmount, false);
                // DisplayDamage(damageAmount, transform.position);      
                if (destoryedOnEnemyInpact) {

                    Destroy(gameObject);
                }
            }
        }
        // Additional condition for CoinStealer
        else if (other.CompareTag("CoinStealer")) {
            // Get the CoinStealer component from the collided object
            CoinStealer coinStealer = other.GetComponent<CoinStealer>();

            // If the CoinStealer component exists, apply damage
            if (coinStealer != null) {
                coinStealer.TakeDamage(damageAmount);
                // DisplayDamage(damageAmount, transform.position);
                if (destoryedOnEnemyInpact) {
                    Destroy(gameObject);
                }
            }
        }
        //else if (other.CompareTag("PassiveEnemy")) {

        //}
        if (other.CompareTag("Border") && destoryedOnBorderInpact) {
            Destroy(gameObject);
        }

    }

    private float GetAngleFromVectorFloat(Vector3 dir) {

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) {
            angle += 360;
        }
        return angle;
    }

    public void SetBulletSpread(Vector2 trajectory) {
      
        trajectory.x += Random.Range(bulletSpread, -1f * bulletSpread);
        trajectory.y += Random.Range(bulletSpread, -1f * bulletSpread);
        direction = trajectory;
        float random = Random.Range(1f, 0f);
        transform.position += new Vector3(random * trajectory.x, random * trajectory.y, 0f);



    }
}
