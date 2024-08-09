using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnHitExplosion : MonoBehaviour
{
    [SerializeField] private int explosionDamage = 30;
    [SerializeField] private bool canDamageEnemies = true;
    [SerializeField] SoundData explosionSound;
    public SoundManager soundManager => SoundManager.Instance;
    private bool initialized;

    private void OnEnable() {
        if(!initialized)
        InitSoundSettings();
        if (explosionSound.clip != null && !GameSettings.Instance.SFXOFF) {
            soundManager.CreateSound().WithSoundData(explosionSound).WithPosition(transform.position).Play();

        }
    }
    protected virtual void InitSoundSettings() {
        explosionSound.loop = false;
        explosionSound.frequentSound = false;
        initialized = true;


     
        var enemyOnHitCategory = soundManager.audioGalleryEntries.ProjectilesCategory;

        // Set the OnShoot sound data
        SoundManager.SetAudioClipForGameobject(gameObject, enemyOnHitCategory, explosionSound, "OnShoot");

    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (canDamageEnemies) {
            if (collision.CompareTag("Enemy")) {
                collision.GetComponent<Enemy>().TakeDamage(explosionDamage, true);
            }
        }
    }
}
