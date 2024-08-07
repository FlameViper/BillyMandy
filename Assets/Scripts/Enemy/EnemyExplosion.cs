using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] private int explosionDamage = 30;
    [SerializeField] private bool canDamagePlayer = true;
    [SerializeField] private bool canDamageEnemies = false;
    [SerializeField] SoundData explosionSound;
    public SoundManager soundManager => SoundManager.Instance;
    private void Start() {

        InitSoundSettings();
        if (explosionSound.clip != null && !GameSettings.Instance.SFXOFF) {
            soundManager.CreateSound().WithSoundData(explosionSound).WithPosition(transform.position).Play();

        }
    }
    protected virtual void InitSoundSettings() {
        explosionSound.loop = false;
        explosionSound.frequentSound = true;
        SetMusicClip();
    }
    public void SetMusicClip() {
        var projectilesCategory = soundManager.audioGalleryEntries.ProjectilesCategory;
        foreach (var field in projectilesCategory.GetAudioClipFields()) {
            // Matching the name convention for OnHit sounds
            if (field.Name == gameObject.name + "OnShoot") {
                // Get the value from the scriptable object field
                AudioClip clip = (AudioClip)field.GetValue(projectilesCategory);
                // Assign it to your local variable
                explosionSound.clip = clip;
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (canDamagePlayer) {
            if (collision.CompareTag("Player")) {
                Player.Instance.TakeDamage(explosionDamage);
            }
        }
        if (canDamageEnemies) {
            if (collision.CompareTag("Enemy")) {
                collision.GetComponent<Enemy>().TakeDamage(explosionDamage,true);
            }
        }
    }
    public void OnExplosionEnd() {
        Destroy(gameObject);
    }
}
