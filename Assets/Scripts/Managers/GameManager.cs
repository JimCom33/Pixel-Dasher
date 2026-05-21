using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    LevelComplete,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState startingState = GameState.Playing;

    public GameState currentState { get; private set; }

    public UnityEvent<GameState> OnGameStateChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyGameState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void SetPlaying()
    {
        SetGameState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            SetGameState(GameState.Paused);
        }
        else if (currentState == GameState.Paused)
        {
            SetGameState(GameState.Playing);
        }
    }

    public void SetGameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void SetLevelComplete()
    {
        SetGameState(GameState.LevelComplete);
    }

    private void SetGameState(GameState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        ApplyGameState(currentState);
        OnGameStateChanged.Invoke(currentState);
    }

    private void ApplyGameState(GameState state)
    {
        if (state == GameState.Paused || state == GameState.GameOver || state == GameState.LevelComplete)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        Instance = null;
    }
}
