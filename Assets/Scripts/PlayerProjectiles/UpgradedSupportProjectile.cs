using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradedSupportProjectile : SupportProjectile
{
 
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) {
         
                FreezeEnemy(enemy);
            }


        }
    }
    private void FreezeEnemy(Enemy enemy) {
        if (enemy != null) {
            enemy.Freeze(true); 

        }
    }

}
