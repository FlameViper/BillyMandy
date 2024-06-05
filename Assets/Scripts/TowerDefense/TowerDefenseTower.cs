using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseTower : MonoBehaviour {
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float attackInterval = 1f;
    [SerializeField] protected Transform projectileSpawnPoint;
    [field: SerializeField] public int TowerCoinCost { get; private set; } = 50;
    protected List<Transform> potentialTargets = new List<Transform>();
    protected Transform currentTarget;
    protected Coroutine attackCoroutine;

    protected virtual void Start() {
       
    }

    protected virtual void Update() {
      
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            potentialTargets.Add(collision.transform);
            UpdateTarget();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision) {
        if (potentialTargets.Contains(collision.transform)) {
            potentialTargets.Remove(collision.transform);
            UpdateTarget();
        }
    }

    protected virtual void UpdateTarget() {
        if (potentialTargets.Count > 0) {
            currentTarget = potentialTargets[0];
            if (attackCoroutine == null) {
                attackCoroutine = StartCoroutine(ShootTargetRepeatedly());
            }
        }
        else {
            currentTarget = null;
            if (attackCoroutine != null) {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    protected virtual IEnumerator ShootTargetRepeatedly() {
        while (currentTarget != null) {
            ShootProjectile();
            yield return new WaitForSeconds(attackInterval);
        }
    }

    protected virtual void ShootProjectile() {
        if (currentTarget == null) return;

        // Instantiate the projectile at the tower's position and set its target
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        DPSTowerProjectile dPSTowerProjectile;
        FreezTowerProjectile freezTowerProjectile;
        projectile.TryGetComponent(out dPSTowerProjectile);
        projectile.TryGetComponent(out freezTowerProjectile);
        if (dPSTowerProjectile != null) {
            dPSTowerProjectile.SetTarget(currentTarget);
        }
        else if (freezTowerProjectile != null) {
            freezTowerProjectile.SetTarget(currentTarget);

        }
    }

    public void DestroyTower() {
        Destroy(gameObject);
    }
}
