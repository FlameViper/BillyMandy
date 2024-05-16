using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgrades : MonoBehaviour
{
    [SerializeField] private Button fireballProjectile;
    [SerializeField] private Button boomerangProjectile;
    [SerializeField] private Button balisticProjectile;
    [SerializeField] private Button blueFireProjectile;
    [SerializeField] private Button minigunProjectile;
    [SerializeField] private Button lazerProjectile;
    [SerializeField] private UpgradeManager upgradeManager;


    private void Awake() {
        // Add listeners to the button click events
        fireballProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.Fireball));
        boomerangProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.Boomerang));
        balisticProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.Balistic));
        blueFireProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.BlueFire));
        minigunProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.Minigun));
        lazerProjectile.onClick.AddListener(() => upgradeManager.PurchaseWeaponUpgrade(ProjectileType.Lazer));
      

    }

}
