using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseEnemySpawner : MonoBehaviour {
    public static TowerDefenseEnemySpawner Instance;
    [field:SerializeField] public int BaseEnemyCount { get; private set; } = 30;
    [SerializeField] private int IncreasedCountPer10Levels = 20;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Transform spawnPoint;
    private int levelNumberOfEnemies;
    private int currentNumberOfEnemies;
    private Coroutine spawnEnemiesCoroutine;
    void Awake() {
        if (Instance == null) {
            Instance = this;

        }
        else {
            Destroy(gameObject);
        }
    
    }
    public void StartWave() {
        currentNumberOfEnemies = 0;
        levelNumberOfEnemies = 0;
        levelNumberOfEnemies = BaseEnemyCount + BattleManager.Instance.level / 10 * IncreasedCountPer10Levels;
        TowerDefenseManager.Instance.currentEnemyCount = levelNumberOfEnemies;
        spawnEnemiesCoroutine ??= StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies() {

        while(currentNumberOfEnemies < levelNumberOfEnemies) {
            int random = Random.Range(0, enemies.Count);
            if(random != 0) {
                random = Random.Range(0, enemies.Count);
            }
            Instantiate(enemies[random], spawnPoint.position,Quaternion.identity);
            if (random == 0) {
                currentNumberOfEnemies++;

            }
            else {
                currentNumberOfEnemies += 5;
            }
            yield return new WaitForSeconds(spawnDelay);

        }
        spawnEnemiesCoroutine = null;
    }
}
