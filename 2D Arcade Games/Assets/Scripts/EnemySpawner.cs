using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public float minSpawnInterval = 2f;
    [SerializeField] public float maxSpawnInterval = 5f;

    private float nextSpawnTime;

    private void Start()
    {
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        if (Arena.Instance == null) return;

        Vector2 spawnPos = Arena.Instance.GetRandomEdgeSpawnPosition();
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
