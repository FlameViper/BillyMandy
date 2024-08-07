using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioGalleryEntries", menuName = "Data/AudioGalleryEntries")]
public class AudioGalleryEntries : ScriptableObject {


    public GalleryCategory BGMCategory;
    public GalleryCategory EnemyOnDeathCategory;
    public GalleryCategory EnemyOnHitCategory;
    public GalleryCategory ProjectilesCategory;
    public DefaultGalerryCategory DefaultBGMCategory;
    public DefaultGalerryCategory DefaultEnemyOnDeathCategory;
    public DefaultGalerryCategory DefaultEnemyOnHitCategory;
    public DefaultGalerryCategory DefaultProjectilesCategory;
    public void InitializeAudioClips() {

        LoadAudioSettings();
    }

    private void LoadAudioSettings() {

        foreach (GalleryCategory galleryCategory in GetCategories()) {
            galleryCategory.LoadAudioSettings();
        }

    }

    public IEnumerable<GalleryCategory> GetCategories() {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields) {
            if (field.FieldType == typeof(GalleryCategory)) {
                yield return (GalleryCategory)field.GetValue(this);
            }
        }
    }
    public IEnumerable<DefaultGalerryCategory> GetDefaultCategories() {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields) {
            if (field.FieldType == typeof(DefaultGalerryCategory)) {
                yield return (DefaultGalerryCategory)field.GetValue(this);
            }
        }
    }
}
