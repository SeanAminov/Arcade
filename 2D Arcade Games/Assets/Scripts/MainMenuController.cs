using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Put this in your MAIN MENU scene. Wire UI buttons to the public methods.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "SampleScene";

    public void StartEasy() => StartGame(DifficultyMode.Easy);
    public void StartMedium() => StartGame(DifficultyMode.Medium);
    public void StartHard() => StartGame(DifficultyMode.Hard);
    public void StartExtreme() => StartGame(DifficultyMode.Extreme);

    private void StartGame(DifficultyMode mode)
    {
        Time.timeScale = 1f;
        DifficultyRuntime.SetMode(mode);
        SceneManager.LoadScene(gameSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

