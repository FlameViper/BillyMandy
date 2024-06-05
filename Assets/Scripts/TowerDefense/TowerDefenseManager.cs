using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerDefenseManager : MonoBehaviour {

    public static TowerDefenseManager Instance;

   // [field: SerializeField] public int numberOfAvalabileWalls { get; private set; } = 35;
    [SerializeField] private TowerDefenseEnemySpawner enemySpawner;
    [SerializeField] private Button startWaveButton;
    [SerializeField] private List<GameObject> towerPrefabs;
    [SerializeField] private bool validPathToPlayer;
    [SerializeField] private GameObject validPathChecker;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject invalidPathWarning;
    [SerializeField] private Camera towerDefenseCamera; 
    public int SelectedTowerIndex { get; private set; } = -1;
    public bool isInPreparationPhase;
    public bool checkingValidPath;
    private Coroutine invalidPathWarningCoroutine;
    private Image currentSelectedTowerImage;
    public int currentEnemyCount;
    void Awake() {
        if (Instance == null) {
            Instance = this;

        }
        else {
            Destroy(gameObject);
        }

        startWaveButton.onClick.AddListener(() => {
            if (!validPathToPlayer && !checkingValidPath) {
                CheckPathToPlayer();

            }
            
        });
        UnselectTower();
        BattleManager.Instance.OnUpgradesDoneEvent += BattleManager_OnUpgradesDoneEvent;
    }

    private void BattleManager_OnUpgradesDoneEvent(object sender, System.EventArgs e) {
        if(BattleManager.Instance.level % 10 == 0) {
            startWaveButton.gameObject.SetActive(true);
            isInPreparationPhase = true;
            checkingValidPath = false;
            validPathToPlayer = false;

        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
           
            Vector2 mousePosition = towerDefenseCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(mousePosition, Vector2.zero);

           
            if (hit2D.collider != null) {
                
                if (hit2D.collider.CompareTag("TowerDefenseTile")) {
                    return;
                }
                else {
                    
                    UnselectTower();
                  
                }
            }
            else {

                UnselectTower();
            }
        }
    }


    private void CheckPathToPlayer() {
        checkingValidPath = true;
        isInPreparationPhase=false;
        AstarPath.active.Scan();
        Instantiate(validPathChecker, spawnPoint.position,Quaternion.identity);  


    }
    public GameObject PlaceTower(Transform transform) {
        return Instantiate(towerPrefabs[SelectedTowerIndex],transform);
    }

    public void SelectTowerIndex(int index) {
        if(SelectedTowerIndex != -1) {
            UnselectTower();
        }
        SelectedTowerIndex = index;
    }
    public void SelectTower(Image image) {
       
        currentSelectedTowerImage = image;
        currentSelectedTowerImage.color = Color.green;
    }
    public void UnselectTower() {
        SelectedTowerIndex = -1;
        if (currentSelectedTowerImage != null) {
            currentSelectedTowerImage.color = Color.white;

        }
    }

    public void UpdateValidPath(bool value) {
        validPathToPlayer = value;
        if (validPathToPlayer) {
            startWaveButton.gameObject.SetActive(false);
            isInPreparationPhase = false;
            TowerDefenseEnemySpawner.Instance.StartWave();

        }
        else {
            invalidPathWarningCoroutine ??= StartCoroutine(InvalidPathWarning());
        }
        checkingValidPath = false;
    }

    private IEnumerator InvalidPathWarning() {
        invalidPathWarning.SetActive(true);
        isInPreparationPhase = true;
        yield return new WaitForSeconds(2);
        invalidPathWarning.SetActive(false);
        invalidPathWarningCoroutine = null;
    }

    public void UpdateEnemyCount(int value) {
        if (currentEnemyCount > 0) {
            currentEnemyCount -= value;
        }
        if(currentEnemyCount <= 0) {
         
            BattleManager.Instance.EndRound();
        }

    }

}
