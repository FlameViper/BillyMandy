using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgrades : MonoBehaviour
{
    [SerializeField] private Button fireballProjectile;
    [SerializeField] private Button boomerangProjectile;
    [SerializeField] private UpgradeManager upgradeManager;


    private void Awake() {
        // Add listeners to the button click events
        fireballProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.Fireball));
        boomerangProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.Boomerang));
      

    }

}
