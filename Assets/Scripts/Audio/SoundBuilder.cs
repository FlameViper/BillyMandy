using UnityEngine;

public class SoundBuilder {
    readonly SoundManager soundManager;
    SoundData soundData;
    Vector3 position = Vector3.zero;

    public SoundBuilder(SoundManager soundManager) {
        this.soundManager = soundManager;
    }
    public SoundBuilder WithSoundData(SoundData soundData) {
        this.soundData = soundData;
        return this;
    }
    public SoundBuilder WithPosition(Vector3 position) {
        this.position = position;
        return this;
    }

    public void Play() {
        if(!soundManager.CanPlaySound(soundData)) return;

        SoundEmitter soundEmitter = soundManager.GetFromPool();
        soundEmitter.Initialize(soundData);
        soundEmitter.transform.parent = soundManager.transform;
        soundEmitter.transform.position = position;
        if (soundData.frequentSound) {
            soundManager.FrequentSoundEmitters.Enqueue(soundEmitter);
        }
        soundEmitter.Play();
    }
}