using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class GalleryCategory : ScriptableObject {


    private SerializableDictionary<string, string> audioClipNames = new SerializableDictionary<string, string>();

    private string persistentPath => Application.persistentDataPath;
    private string audioSettingsFilePath => Path.Combine(persistentPath, "SavedAudio", this.name, "audioSettings.json");
    private string savedAudioPath => Path.Combine(persistentPath, "SavedAudio",this.name);
    public IEnumerable<FieldInfo> GetAudioClipFields() {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields) {
            if (field.FieldType == typeof(AudioClip)) {
                yield return field;
            }
        }
    }
    public IEnumerable<FieldInfo> GetIconFields() {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields) {
            if (field.FieldType == typeof(Sprite)) {
                yield return field;
            }
        }
    }
    public virtual void LoadAudioSettings() {

        
        if (File.Exists(audioSettingsFilePath)){
            Debug.Log("init");
            string json = File.ReadAllText(audioSettingsFilePath);
            audioClipNames = JsonUtility.FromJson<SerializableDictionary<string, string>>(json);
            foreach (var field in GetAudioClipFields()) {
                if (audioClipNames.TryGetValue(field.Name, out string audioFileName)) {
                    string fieldPath = Path.Combine(savedAudioPath, field.Name, audioFileName);
                    if (File.Exists(fieldPath))
                    field.SetValue(this, LoadWav(fieldPath));
                }
            }      
        }
        foreach (var field in GetAudioClipFields()) {
            AudioClip currentValue = field.GetValue(this) as AudioClip;
            if (currentValue == null) {
                foreach (DefaultGalerryCategory defaultGalleryCategory in SoundManager.Instance.audioGalleryEntries.GetDefaultCategories()) {
                    if (defaultGalleryCategory.name == "Default"+this.GetType().Name) {
                        
                        foreach (var defaultField in defaultGalleryCategory.GetAudioClipFields()) {
                            if (defaultField.Name == field.Name) {
                                field.SetValue(this, defaultField.GetValue(defaultGalleryCategory));
                            }

                         
                        }
                    }
//#if !UNITY_EDITOR
//         defaultGalleryCategory.ClearDefaultAllAudioClipFields();
//#endif
                }
            }

        }


    }
    public virtual AudioClip LoadWav(string filePath) {
        return NAudioPlayer.LoadWav(filePath);
    }
    public virtual void SaveAudioSetting(string key, string fileName) {
        audioClipNames[key] = fileName;
        string json = JsonUtility.ToJson(audioClipNames, true);
        File.WriteAllText(audioSettingsFilePath, json);
    }
    public void RemoveAudioSetting(string key) {
        if (audioClipNames.ContainsKey(key)) {
            audioClipNames.Remove(key);
            string json = JsonUtility.ToJson(audioClipNames, true);
            File.WriteAllText(audioSettingsFilePath, json);
            
        }
    }
    public string GetCurrentFieldSetting(string fieldName) {
        // Assuming 'audioClipNames' is a dictionary where keys are field names and values are the saved file names
        if (audioClipNames.TryGetValue(fieldName, out string savedFileName)) {
            return savedFileName;
        }
        return null;
    }

}
