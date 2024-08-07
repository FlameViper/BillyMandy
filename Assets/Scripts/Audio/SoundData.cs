using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class SoundData {
    public AudioClip clip;
    public bool loop;
    public bool playOnAwake;
    public bool frequentSound;
}
