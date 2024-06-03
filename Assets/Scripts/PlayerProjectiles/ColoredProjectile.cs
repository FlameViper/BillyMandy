using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredProjectile : Projectile {

    public static int maxNumberOfBounces = 3;

    [SerializeField] private GameObject chainLightning;
    [SerializeField] private GameObject gotBouncedOn;
    [SerializeField] private int damage=20;
    [SerializeField] private bool damageIncrasedByProjectileDamage=true;
    public SpriteRenderer spriteRenderer;
    public string color;
    protected override void Start() {
        base.Start();
        if (!damageIncrasedByProjectileDamage) {
            damageAmount = damage;

        }
        // Rotate the sprite to face the direction of movement
        transform.rotation = Quaternion.Euler(0, 0, GetAngleFromVectorFloat(direction));
    }
 
    protected override void Update() {

        // Move the projectile in its direction
        transform.position += direction * speed * Time.deltaTime;

    }
    private float GetAngleFromVectorFloat(Vector3 dir) {

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) {
            angle += 360;
        }
        return angle;
    }



    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision != null && collision.gameObject.CompareTag("ColoredEnemy")) {
            // Get the Enemy component from the collided object
            ColoredEnemy coloredEnemy = collision.gameObject.GetComponent<ColoredEnemy>();

            // If the enemy component exists, apply damage
            if (coloredEnemy != null) {

                if (coloredEnemy.currentColor == color) {
                 
                    coloredEnemy.TakeDamage(damageAmount, false);
                    ColoredChainLighting coloredChainLighting = Instantiate(chainLightning, collision.gameObject.transform.position, Quaternion.identity).GetComponent<ColoredChainLighting>();
                    coloredChainLighting.numberOfBounces = maxNumberOfBounces;
                    coloredChainLighting.damage = damageAmount;
                    coloredChainLighting.color = color;
                    Instantiate(gotBouncedOn, collision.gameObject.transform);

                }

                if (destoryedOnEnemyInpact) {
                    Destroy(gameObject);
                }
            }
        }


    }


}
