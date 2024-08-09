using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFireballProjectile : Projectile {


    public static float maxNumberOfPassedEnemies = 3;
    public float currrentNumberOfPassedEnemies = 0;
    private SpriteRenderer spriteRenderer;
    private static Color color = Color.white;
    private static bool isInfinite = false;

    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }
    protected override void Start() {
        base.Start();
        // Rotate the sprite to face the direction of movement
        transform.rotation = Quaternion.Euler(0, 0, GetAngleFromVectorFloat(direction));
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
                currrentNumberOfPassedEnemies++;
                if (destoryedOnEnemyInpact && !isInfinite && currrentNumberOfPassedEnemies >= maxNumberOfPassedEnemies) {
                    
                    Destroy(gameObject);
                }
            }
        }
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

    public static void SetColor(Color value) {
        color = value;
    }

    public static void SetBlackfire() {
        isInfinite = true;
    }

}
