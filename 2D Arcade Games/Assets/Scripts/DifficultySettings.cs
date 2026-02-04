using System;
using UnityEngine;

[Serializable]
public class DifficultyPreset
{
    [Header("Player")]
    [Tooltip("Seconds to reload after missing a shot.")]
    public float missReloadTime = 3f;

    [Header("Enemies")]
    [Tooltip("Multiplier applied to each Enemy's chaseSpeed.")]
    public float enemySpeedMultiplier = 1f;

    [Header("Spawner (start)")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    [Header("Spawner ramp")]
    public bool rampSpawnRateOverTime = true;
    public float rampDuration = 120f;

    [Header("Spawner (end)")]
    public float minSpawnIntervalEnd = 0.6f;
    public float maxSpawnIntervalEnd = 1.5f;
}

[CreateAssetMenu(menuName = "Arcade/Difficulty Settings", fileName = "DifficultySettings")]
public class DifficultySettings : ScriptableObject
{
    public DifficultyPreset easy = new DifficultyPreset();
    public DifficultyPreset medium = new DifficultyPreset();
    public DifficultyPreset hard = new DifficultyPreset();
    public DifficultyPreset extreme = new DifficultyPreset();

    [ContextMenu("Apply Recommended Defaults")]
    public void ApplyRecommendedDefaults()
    {
        // Requested reload miss times:
        // Easy 0.5, Medium 1.0, Hard 1.5, Extreme 2.0
        easy.missReloadTime = 0.5f;
        medium.missReloadTime = 1.0f;
        hard.missReloadTime = 1.5f;
        extreme.missReloadTime = 2.0f;

        // All modes ramp toward the same fast end range:
        // min 0.2, max 0.5
        ApplySharedEndTargets(easy);
        ApplySharedEndTargets(medium);
        ApplySharedEndTargets(hard);
        ApplySharedEndTargets(extreme);

        // Spawning behavior:
        // - Easy: start interval is constant (min == max), and ramps up slower
        // - Others: start faster and ramp up sooner (but still ramp)
        easy.rampSpawnRateOverTime = true;
        easy.minSpawnInterval = 3.0f;
        easy.maxSpawnInterval = 3.0f; // constant at start
        easy.rampDuration = 180f;     // longer to ramp
        easy.enemySpeedMultiplier = 0.9f;

        medium.rampSpawnRateOverTime = true;
        medium.minSpawnInterval = 2.0f;
        medium.maxSpawnInterval = 3.5f;
        medium.rampDuration = 140f;
        medium.enemySpeedMultiplier = 1.0f;

        hard.rampSpawnRateOverTime = true;
        hard.minSpawnInterval = 1.5f;
        hard.maxSpawnInterval = 2.5f;
        hard.rampDuration = 110f;
        hard.enemySpeedMultiplier = 1.15f;

        extreme.rampSpawnRateOverTime = true;
        extreme.minSpawnInterval = 1.0f;
        extreme.maxSpawnInterval = 2.0f;
        extreme.rampDuration = 90f;
        extreme.enemySpeedMultiplier = 1.3f;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private static void ApplySharedEndTargets(DifficultyPreset p)
    {
        p.minSpawnIntervalEnd = 0.2f;
        p.maxSpawnIntervalEnd = 0.5f;
    }

    public DifficultyPreset Get(DifficultyMode mode)
    {
        return mode switch
        {
            DifficultyMode.Easy => easy,
            DifficultyMode.Medium => medium,
            DifficultyMode.Hard => hard,
            DifficultyMode.Extreme => extreme,
            _ => easy
        };
    }
}

