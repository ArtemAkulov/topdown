using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public bool devMode;

    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float CampingThresholdDistance = 1.5f;
    float nextCampingCheckTimer;
    Vector3 campPositionOld;
    bool isCamping;
    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start() {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampingCheckTimer = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;

        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update() {
        if (!isDisabled) {
            if (Time.time > nextCampingCheckTimer) {
                nextCampingCheckTimer = timeBetweenCampingChecks + Time.time;
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < CampingThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infiniteEnemies) && Time.time > nextSpawnTime) {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>()) {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy() {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        Transform portal = map.GetRandomOpenTile();
        if (isCamping) {
            portal = map.GetTileFromPosition(playerT.position);
        }
        Material portalMaterial = portal.GetComponent<Renderer>().material;
        print(portalMaterial.color);
        Color portalColor = portalMaterial.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay) {
            portalMaterial.color = Color.Lerp(portalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, portal.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetParameters(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    void OnPlayerDeath() {
        isDisabled = true;
    }

    void OnEnemyDeath() {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0) {
            NextWave();
        }
    }

    void ResetPlayerPosition() {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2;
    }

    void NextWave() {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length) {
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            if (OnNewWave != null) {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

    [System.Serializable]
    public class Wave {
        public bool infiniteEnemies;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }

}
