using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DefaultGalerryCategory : ScriptableObject {

    public IEnumerable<FieldInfo> GetAudioClipFields() {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields) {
            if (field.FieldType == typeof(AudioClip)) {
                yield return field;
            }
        }
    }
    public void ClearDefaultAllAudioClipFields() {
        foreach (var field in GetAudioClipFields()) {
            field.SetValue(this, null);
        }

        // Force garbage collection to clear memory
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

}