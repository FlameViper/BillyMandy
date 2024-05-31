using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredProjectile : Projectile {

    public SpriteRenderer spriteRenderer;
    public string color;

    protected override void Update() {

        // Move the projectile in its direction
        transform.position += direction * speed * Time.deltaTime;
    }
    protected override void Start() {
        base.Start();
        // Rotate the sprite to face the direction of movement
        transform.rotation = Quaternion.Euler(0, 0, GetAngleFromVectorFloat(direction));
    }
    private float GetAngleFromVectorFloat(Vector3 dir) {

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) {
            angle += 360;
        }
        return angle;
    }



}
