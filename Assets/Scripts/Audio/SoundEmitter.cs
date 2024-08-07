using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour {
    public SoundData SoundData { get; private set; }
    AudioSource audioSource;
    Coroutine playingCoroutine;

    private void Awake() {
        audioSource = gameObject.GetOrAdd<AudioSource>();
    }

    public void Play() {
        if (playingCoroutine != null) {
            StopCoroutine(playingCoroutine);
        }
        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }

    public void Stop() {
        if (playingCoroutine != null) {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }
        audioSource.Stop();
        SoundManager.Instance.ReturnToPool(this);
    }

    private IEnumerator WaitForSoundToEnd() {
        yield return new WaitWhile( () => audioSource.isPlaying);
        SoundManager.Instance.ReturnToPool(this);
    }

    public void Initialize(SoundData soundData) {
        SoundData = soundData;
        audioSource.clip = soundData.clip;
        audioSource.loop = soundData.loop;
        audioSource.playOnAwake = soundData.playOnAwake;
    
    
    }
}
