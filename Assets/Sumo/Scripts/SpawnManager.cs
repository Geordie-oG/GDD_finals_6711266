using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;

    public int enemyCount;
    public int waveNumber = 1;

    private float spawnRange = 9.0f;

    void Start()
    {
        Instantiate(
            powerupPrefab,
            GenerateSpawnPosition(),
            powerupPrefab.transform.rotation
        );

        SpawnEnemyWave(waveNumber);
    }

    void Update()
    {
        enemyCount =
            FindObjectsByType<Enemy>().Length;

        if (enemyCount == 0)
        {
            waveNumber++;

            Instantiate(
                powerupPrefab,
                GenerateSpawnPosition(),
                powerupPrefab.transform.rotation
            );

            SpawnEnemyWave(waveNumber);
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(
                enemyPrefab,
                GenerateSpawnPosition(),
                enemyPrefab.transform.rotation
            );
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX =
            Random.Range(-spawnRange, spawnRange);

        float spawnPosZ =
            Random.Range(-spawnRange, spawnRange);

        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}