using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public float minSpawnInterval = 2f;
    [SerializeField] public float maxSpawnInterval = 5f;

    [Header("Spawn Placement")]
    [Tooltip("If enabled, enemies spawn off-screen (outside camera view) but still inside the arena, so they walk into view.")]
    [SerializeField] public bool spawnOutsideCameraView = true;
    [Tooltip("How far outside the camera view enemies should spawn (world units).")]
    [SerializeField] public float offscreenMargin = 2f;

    [Header("Ramp Up (harder over time)")]
    [Tooltip("If enabled, spawn intervals shrink over time (spawns get faster).")]
    [SerializeField] public bool rampSpawnRateOverTime = true;
    [Tooltip("How long until reaching the end intervals (seconds).")]
    [SerializeField] public float rampDuration = 120f;
    [Tooltip("Minimum spawn interval after ramp completes.")]
    [SerializeField] public float minSpawnIntervalEnd = 0.6f;
    [Tooltip("Maximum spawn interval after ramp completes.")]
    [SerializeField] public float maxSpawnIntervalEnd = 1.5f;

    private float nextSpawnTime;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    private void SetNextSpawnTime()
    {
        float curMin = minSpawnInterval;
        float curMax = maxSpawnInterval;

        if (rampSpawnRateOverTime && rampDuration > 0f)
        {
            float t = Mathf.Clamp01(Time.timeSinceLevelLoad / rampDuration);
            curMin = Mathf.Lerp(minSpawnInterval, minSpawnIntervalEnd, t);
            curMax = Mathf.Lerp(maxSpawnInterval, maxSpawnIntervalEnd, t);
        }

        // Safety: ensure min <= max
        if (curMax < curMin) curMax = curMin;

        nextSpawnTime = Time.time + Random.Range(curMin, curMax);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        if (Arena.Instance == null) return;

        Vector2 spawnPos = spawnOutsideCameraView ? GetOffscreenSpawnPosition() : Arena.Instance.GetRandomEdgeSpawnPosition();
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private Vector2 GetOffscreenSpawnPosition()
    {
        // If we can't compute camera bounds, fall back to arena-edge spawn.
        if (cam == null || !cam.orthographic)
            return Arena.Instance.GetRandomEdgeSpawnPosition();

        // Camera world bounds
        Vector2 camPos = cam.transform.position;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float camLeft = camPos.x - halfW;
        float camRight = camPos.x + halfW;
        float camBottom = camPos.y - halfH;
        float camTop = camPos.y + halfH;

        // Arena inner bounds (use spawnInset so we don't spawn inside wall thickness)
        // NOTE: This project assumes Arena is centered at (0,0). Keep Arena at (0,0) for now.
        float arenaHalfW = Arena.Instance.width * 0.5f - Arena.Instance.spawnInset;
        float arenaHalfH = Arena.Instance.height * 0.5f - Arena.Instance.spawnInset;
        float arenaLeft = -arenaHalfW;
        float arenaRight = arenaHalfW;
        float arenaBottom = -arenaHalfH;
        float arenaTop = arenaHalfH;

        // Build a list of valid sides where "outside camera but inside arena" has room.
        // 0=Left, 1=Right, 2=Bottom, 3=Top
        int[] sides = new int[4];
        int sideCount = 0;

        if (camLeft - offscreenMargin > arenaLeft) sides[sideCount++] = 0;
        if (camRight + offscreenMargin < arenaRight) sides[sideCount++] = 1;
        if (camBottom - offscreenMargin > arenaBottom) sides[sideCount++] = 2;
        if (camTop + offscreenMargin < arenaTop) sides[sideCount++] = 3;

        if (sideCount == 0)
        {
            // Arena isn't larger than the camera view (or margin too big). Fall back.
            return Arena.Instance.GetRandomEdgeSpawnPosition();
        }

        int side = sides[Random.Range(0, sideCount)];
        switch (side)
        {
            case 0: // Left
                return new Vector2(
                    Random.Range(arenaLeft, camLeft - offscreenMargin),
                    Random.Range(arenaBottom, arenaTop)
                );
            case 1: // Right
                return new Vector2(
                    Random.Range(camRight + offscreenMargin, arenaRight),
                    Random.Range(arenaBottom, arenaTop)
                );
            case 2: // Bottom
                return new Vector2(
                    Random.Range(arenaLeft, arenaRight),
                    Random.Range(arenaBottom, camBottom - offscreenMargin)
                );
            default: // Top
                return new Vector2(
                    Random.Range(arenaLeft, arenaRight),
                    Random.Range(camTop + offscreenMargin, arenaTop)
                );
        }
    }
}
