using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour {
    [SerializeField] Button musicButton;
    [SerializeField] Button SFXButton;
    [SerializeField] AudioSource battleMusic;
    [SerializeField] AudioSource upgradesMusic;
    [SerializeField] AudioSource mainMusic;
    [SerializeField] AudioSource towerDefenseMusic;
    [SerializeField] AudioSource startgameSFX;
    [SerializeField] private Sprite musicIconOn;
    [SerializeField] private Sprite musicIconOff;
    [SerializeField] private Image musicIcon;
    [SerializeField] private Sprite SFXIconOn;
    [SerializeField] private Sprite SFXIconOff;
    [SerializeField] private Image SFXIcon;
    //private void Awake() {
     
    //}

    private void Start() {
        GameSettings.Instance.MusicOFF = false;
        GameSettings.Instance.SFXOFF = false;

        musicButton.onClick.AddListener(() => {
            GameSettings.Instance.MusicOFF = !GameSettings.Instance.MusicOFF;
            battleMusic.mute = GameSettings.Instance.MusicOFF;
            mainMusic.mute = GameSettings.Instance.MusicOFF;
            upgradesMusic.mute = GameSettings.Instance.MusicOFF;
            towerDefenseMusic.mute = GameSettings.Instance.MusicOFF;
            musicIcon.sprite = GameSettings.Instance.MusicOFF ? musicIconOff : musicIconOn;

        });

        SFXButton.onClick.AddListener(() => {
            GameSettings.Instance.SFXOFF = !GameSettings.Instance.SFXOFF;
            SFXIcon.sprite = GameSettings.Instance.SFXOFF ? SFXIconOff : SFXIconOn;
            startgameSFX.mute = GameSettings.Instance.SFXOFF;
        });
    }

}
