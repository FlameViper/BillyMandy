using UnityEngine.Pool;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SoundManager : PersistentSingleton<SoundManager> {
    IObjectPool<SoundEmitter> soundEmitterPool;
    readonly List<SoundEmitter> activeSoundEmitters = new();
   // public readonly Dictionary<SoundData, int> Counts = new();
    public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();
    [SerializeField] SoundEmitter soundEmitterPrefab;
    [SerializeField] bool collectionCheck = true;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxPoolSize = 100;
    [SerializeField] int maxSoundInstances = 30;
    public AudioGalleryEntries audioGalleryEntries;
    public string CurrentSelectedAudioField;
    public AudioLoader audioLoader => GetComponent<AudioLoader>();
    private void Start() {
        if (audioGalleryEntries != null) {
           
            audioGalleryEntries.InitializeAudioClips();
        }
        InitializePool();
    }
    public SoundBuilder CreateSound() {
      
        return new SoundBuilder(this);
    }

    public SoundEmitter GetFromPool() {
        return soundEmitterPool.Get(); 
    }

    public bool CanPlaySound(SoundData soundData) {
        if(!soundData.frequentSound) return true;
        if (FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter)) {
            try {
                soundEmitter.Stop();
                return true;
            }
            catch {
                //Debug.Log("SoundEmitter already released");
            }
            return false;
        }
        return true;

    }

    public void ReturnToPool(SoundEmitter soundEmitter) {
        soundEmitterPool.Release(soundEmitter);
    }


    void InitializePool() {
        soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
        );
    }

    private void OnDestroyPoolObject(SoundEmitter soundEmitter) {
        Destroy(soundEmitter.gameObject);
    }

    private void OnReturnedToPool(SoundEmitter soundEmitter) {
        //if (Counts.TryGetValue(soundEmitter.SoundData, out var count)) {
        //    Counts[soundEmitter.SoundData] -= count > 0 ?1:0;

        //}
        soundEmitter.gameObject.SetActive(false);
        activeSoundEmitters.Remove(soundEmitter);
    }

    private void OnTakeFromPool(SoundEmitter soundEmitter) {
        soundEmitter.gameObject.SetActive(true);
        activeSoundEmitters.Add(soundEmitter);
    }

    private SoundEmitter CreateSoundEmitter() {
        var soundEmitter = Instantiate(soundEmitterPrefab);
        soundEmitter.gameObject.SetActive(false);
        return soundEmitter;
    }
}