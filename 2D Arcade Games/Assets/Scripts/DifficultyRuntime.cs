using UnityEngine;

public static class DifficultyRuntime
{
    private const string ModeKey = "SelectedDifficultyMode";

    public static DifficultyMode Mode { get; private set; } = DifficultyMode.Easy;
    public static float EnemySpeedMultiplier { get; private set; } = 1f;
    public static float MissReloadTime { get; private set; } = 3f;

    public static void SetMode(DifficultyMode mode)
    {
        Mode = mode;
        PlayerPrefs.SetInt(ModeKey, (int)mode);
        PlayerPrefs.Save();
    }

    public static DifficultyMode LoadMode()
    {
        Mode = (DifficultyMode)PlayerPrefs.GetInt(ModeKey, (int)DifficultyMode.Easy);
        return Mode;
    }

    public static void ApplyPreset(DifficultyPreset preset)
    {
        EnemySpeedMultiplier = Mathf.Max(0.01f, preset.enemySpeedMultiplier);
        MissReloadTime = Mathf.Max(0.05f, preset.missReloadTime);
    }

    public static string BestScoreKeyForCurrentMode()
    {
        return $"BestScore_{Mode}";
    }
}

