using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Settings")]
    [SerializeField] private string bestScoreKeyPrefix = "BestScore";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private int currentScore = 0;
    private int bestScore = 0;
    private bool isGameOver = false;

    public static GameManager Instance { get; private set; }
    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        Instance = this;
        DifficultyRuntime.LoadMode();
        LoadBestScore();
    }

    private void Start()
    {
        // Ensure game runs normally when scene loads
        Time.timeScale = 1f;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (retryButton != null)
            retryButton.onClick.AddListener(Retry);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        UpdateUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;

        currentScore += points;
        UpdateUI();

        // Check for new best score
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            SaveBestScore();
            UpdateUI();
        }
    }

    public void PlayerDied()
    {
        if (isGameOver) return;

        isGameOver = true;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverScoreText != null)
                gameOverScoreText.text = $"Score: {currentScore}";
        }

        // Disable player movement
        if (player != null)
            player.enabled = false;

        // Stop spawner(s) immediately
        foreach (var spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
            spawner.enabled = false;

        // Stop enemies immediately (in case they had velocity)
        foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            var rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // Freeze the whole game
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";

        if (bestScoreText != null)
            bestScoreText.text = $"Best: {bestScore}";
    }

    private void LoadBestScore()
    {
        string key = GetBestScoreKey();
        bestScore = PlayerPrefs.GetInt(key, 0);
    }

    private void SaveBestScore()
    {
        string key = GetBestScoreKey();
        PlayerPrefs.SetInt(key, bestScore);
        PlayerPrefs.Save();
    }

    private string GetBestScoreKey()
    {
        // Separate best score per difficulty mode
        // Example: BestScore_Easy, BestScore_Hard, etc.
        string modeKey = DifficultyRuntime.BestScoreKeyForCurrentMode();
        if (!string.IsNullOrWhiteSpace(bestScoreKeyPrefix) && !modeKey.StartsWith(bestScoreKeyPrefix))
            return $"{bestScoreKeyPrefix}_{DifficultyRuntime.Mode}";
        return modeKey;
    }
}
