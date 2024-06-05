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
            DontDestroyOnLoad(gameObject);
        }
    }

    public Camera mainCamera;
    public Camera battleCamera;
    public Camera upgradesCamera;
    public Camera example2Camera;
    public Camera towerDefenseCamera;

    public AudioSource mainMusic;
    public AudioSource battleMusic;
    public AudioSource upgradesMusic;
    public AudioSource example2Music;
    public AudioSource towerDefenseMusic;

    public Canvas mainCanvas;
    public Canvas battleCanvas;
    public Canvas upgradesCanvas;
    public Canvas example2Canvas;
    public Canvas towerDefenseCanvas;

    public GameObject player; // Reference to the player GameObject

    void Start()
    {
        EnableMainCamera();
    }

    public void EnableBattleCamera()
    {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic);
        SetCameraActive(battleCamera, true, battleCanvas, battleMusic);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic);
        SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic);
        SetPlayerChildrenActive(true); // Enable Player's children
    }

    public void EnableMainCamera()
    {
        SetCameraActive(mainCamera, true, mainCanvas, mainMusic);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic);
        SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic);
        SetPlayerChildrenActive(false); // Disable Player's children
    }
    public void EnableTowerDefenseCamera() {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic);
        SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, true, towerDefenseCanvas, towerDefenseMusic);
        SetPlayerChildrenActive(true); // Disable Player's children
    }

    public void EnableUpgradesCamera()
    {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic);
        SetCameraActive(upgradesCamera, true, upgradesCanvas, upgradesMusic);
        SetCameraActive(example2Camera, false, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic);
        SetPlayerChildrenActive(false); // Disable Player's children
    }

    public void EnableExample2Camera()
    {
        SetCameraActive(mainCamera, false, mainCanvas, mainMusic);
        SetCameraActive(battleCamera, false, battleCanvas, battleMusic);
        SetCameraActive(upgradesCamera, false, upgradesCanvas, upgradesMusic);
        SetCameraActive(example2Camera, true, example2Canvas, example2Music);
        SetCameraActive(towerDefenseCamera, false, towerDefenseCanvas, towerDefenseMusic);
        SetPlayerChildrenActive(false); // Disable Player's children
    }

    private void SetCameraActive(Camera camera, bool isActive, Canvas canvas, AudioSource music)
    {
        camera.enabled = isActive;
        canvas.gameObject.SetActive(isActive);

        if (!isActive)
        {
            music.Stop();
        }
        else
        {
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
