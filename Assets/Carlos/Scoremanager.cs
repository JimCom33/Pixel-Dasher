using UnityEngine;
using TMPro;   // remove this line if you are using legacy UI Text

/// <summary>
/// Score manager singleton.
/// Place an empty GameObject with this script in the scene.
/// Persists across scene loads via DontDestroyOnLoad.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;   // drag the TMP label from the Inspector

    private int score;

    void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Add points to the current score.</summary>
    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    /// <summary>Returns the current total score.</summary>
    public int GetScore() => score;

    /// <summary>Resets the score to zero (call this on game restart).</summary>
    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
}
