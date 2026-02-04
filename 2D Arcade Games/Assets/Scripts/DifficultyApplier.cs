using UnityEngine;

/// <summary>
/// Put this in your GAME scene. It reads the selected mode and applies difficulty settings
/// to Player, EnemySpawner, and the global Enemy speed multiplier.
/// </summary>
public class DifficultyApplier : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DifficultySettings difficultySettings;

    [Header("Debug / Testing")]
    [Tooltip("Enable to force a mode in the editor (so you can test without the menu).")]
    [SerializeField] private bool overrideModeInEditor = false;
    [SerializeField] private DifficultyMode overrideMode = DifficultyMode.Easy;

    private void Awake()
    {
        // Ensure the selected mode is loaded even if you skipped the menu
        DifficultyRuntime.LoadMode();

#if UNITY_EDITOR
        if (overrideModeInEditor)
            DifficultyRuntime.SetMode(overrideMode);
#endif

        if (difficultySettings == null)
        {
            Debug.LogWarning("[DifficultyApplier] No DifficultySettings assigned. Using current values.");
            return;
        }

        DifficultyPreset preset = difficultySettings.Get(DifficultyRuntime.Mode);
        DifficultyRuntime.ApplyPreset(preset);

        ApplyToPlayer(preset);
        ApplyToSpawners(preset);

        Debug.Log(
            $"[DifficultyApplier] Applied {DifficultyRuntime.Mode}: " +
            $"missReloadTime={preset.missReloadTime}, " +
            $"enemySpeedMultiplier={preset.enemySpeedMultiplier}, " +
            $"spawn={preset.minSpawnInterval:0.##}-{preset.maxSpawnInterval:0.##}, " +
            $"rampDuration={preset.rampDuration}, " +
            $"end={preset.minSpawnIntervalEnd:0.##}-{preset.maxSpawnIntervalEnd:0.##}"
        );
    }

    private static void ApplyToPlayer(DifficultyPreset preset)
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null) return;

        player.missReloadTime = preset.missReloadTime;
    }

    private static void ApplyToSpawners(DifficultyPreset preset)
    {
        foreach (var spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
        {
            spawner.minSpawnInterval = preset.minSpawnInterval;
            spawner.maxSpawnInterval = preset.maxSpawnInterval;
            spawner.rampSpawnRateOverTime = preset.rampSpawnRateOverTime;
            spawner.rampDuration = preset.rampDuration;
            spawner.minSpawnIntervalEnd = preset.minSpawnIntervalEnd;
            spawner.maxSpawnIntervalEnd = preset.maxSpawnIntervalEnd;
        }
    }
}

