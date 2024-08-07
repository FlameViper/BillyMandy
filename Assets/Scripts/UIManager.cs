using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public Transform battleCanvasTransform;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    public Camera mainCamera;
    public Camera battleCamera;
    public Camera upgradesCamera;
   // public Camera example2Camera;
    public Camera towerDefenseCamera;

    public AudioSource mainMusic;
    public AudioSource battleMusic;
    public AudioSource upgradesMusic;
   // public AudioSource example2Music;
    public AudioSource towerDefenseMusic;

    public Canvas mainCanvas;
    public Canvas battleCanvas;
    public Canvas upgradesCanvas;
 //   public Canvas example2Canvas;
    public Canvas towerDefenseCanvas;

    public GameObject player; // Reference to the player GameObject

    void Start()
    {
        EnableMainCamera();
    }

    public void EnableBattleCamera()
    {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic, false);
        SetCameraActive(battleCamera, true, battleCanvas, battleMusic, true);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic, false);
       // SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic, true);
        SetPlayerChildrenActive(true); // Enable Player's children
    }

    public void EnableMainCamera()
    {
        SetCameraActive(mainCamera, true, mainCanvas, mainMusic, false);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic, true);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic, false);
       // SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic,true);
        SetPlayerChildrenActive(false); // Disable Player's children
    }
    public void EnableTowerDefenseCamera() {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic, false);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic, true);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic, false);
      //  SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, true, towerDefenseCanvas, towerDefenseMusic, true);
        SetPlayerChildrenActive(true); // Disable Player's children
    }

    public void EnableUpgradesCamera()
    {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic, false);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic, true);
        SetCameraActive(upgradesCamera, true, upgradesCanvas, upgradesMusic, false);
       // SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic, true);
        SetPlayerChildrenActive(false); // Disable Player's children
    }

    public void EnableExample2Camera()
    {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic, false);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic,true);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic, false);
      //  SetCameraActive(example2Camera, true, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic, true);
        SetPlayerChildrenActive(false); // Disable Player's children
    }

    private void SetCameraActive(Camera camera, bool isActive, Canvas canvas, AudioSource music,bool isBattleMusic)
    {
        camera.enabled = isActive;
        canvas.gameObject.SetActive(isActive);

        if (!isActive)
        {
            music.Stop();
        }
        else
        {
            if (isBattleMusic) {
                SoundManager.Instance.audioLoader.ChooseRandomBGM();
                foreach (var field in SoundManager.Instance.audioGalleryEntries.BGMCategory.GetAudioClipFields()) {
                    if (field.Name == "normalBgMusic") { }
                    music.clip = (AudioClip)field.GetValue(SoundManager.Instance.audioGalleryEntries.BGMCategory);
                }
            }
            music.Play();

        }
    }

    // New method to enable/disable Player's children
    private void SetPlayerChildrenActive(bool isActive)
    {
        foreach (Transform child in player.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }
}
