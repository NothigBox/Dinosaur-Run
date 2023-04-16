using UnityEngine;
using SCENE = UnityEngine.SceneManagement.SceneManager;

public class GameManager : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speed;
    [SerializeField] private float speedIncreasePercentage;
    [Tooltip("The maximum value that can reach the Time.timeScale")]
    [SerializeField] private float maxSpeedMultiplier;

    [Space, Header("Delays")]
    [SerializeField] private float obstaclesDelay;
    [SerializeField] private float restartGameDelay;
    [SerializeField] private float increaseSpeedDelay;
    [SerializeField] private float increaseScoreDelay;

    [Space, Header("Managers")]
    [SerializeField] private UIManager ui;
    [SerializeField] private MapManager map;
    [SerializeField] private ScoreManager score;
    [SerializeField] private DinosaurManager dinosaur;

    private GameStateObserver[] stateObservers;

    private bool canRestart;
    private float scoreTimer;
    private float gameplayTimer;

    private static GameManager instance;
    public static GameManager Instance => instance;

    public GameState State { get; private set; }

    private void Awake()
    {
        instance = this;

        canRestart = false;
        scoreTimer = 0f;
        gameplayTimer = 0f;

        Time.timeScale = 1f;

        stateObservers = FindObjectsOfType<GameStateObserver>();
    }

    private void Start()
    {
        dinosaur.OnDead += StopGame;
        score.OnScoreAdded += ui.UpdateScore;

        SetGameState(GameState.Home);

        if (UIManager.PlayerName != null)
        {
            StartWaiting();
        }
    }

    private void Update()
    {

        if (State == GameState.Home) return;

        //Player Input
        bool receivedInput = Input.anyKeyDown;

        if (State == GameState.Restarting)
        {
            if (receivedInput && canRestart) RestartScene();
        }

        if (receivedInput)
        {
            dinosaur.Jump();
            
            if (State == GameState.Waiting) 
            {
                SetGameState(GameState.Playing);
            }
        }

        if (State != GameState.Playing) return;

        //Map Logic
        if (gameplayTimer >= increaseSpeedDelay)
        {
            gameplayTimer = 0f;

            float newTimeScale = Time.timeScale * (1f + (speedIncreasePercentage / 100f));
            Time.timeScale = Mathf.Clamp(newTimeScale, 1f, maxSpeedMultiplier);
        }

        gameplayTimer += Time.deltaTime;

        Vector3 displacement = Vector3.left * (Time.deltaTime * speed);
        map.Move(displacement);
    }

    private void FixedUpdate()
    {
        if (State != GameState.Playing) return;

        //Score Logic
        if (scoreTimer >= increaseScoreDelay)
        {
            scoreTimer = 0f;

            score.AddScore(1);
        }

        scoreTimer += Time.fixedUnscaledDeltaTime;
    }

    private void StopGame()
    {
        SetGameState(GameState.Restarting);
    }

    public void SetGameState(GameState newState) 
    {
        State = newState;

        switch (State)
        {
            case GameState.Home:
                OnHome();
                break;

            case GameState.Waiting:
                OnWaiting();
                break;

            case GameState.Playing:
                OnPlaying();
                break;

            case GameState.Restarting:
                OnRestarting();
                break;
        }

        foreach (var observer in stateObservers) 
        {
            observer.UpdateState(newState);
        }
    }

    private void OnHome() 
    {
    }

    private void OnWaiting()
    {
    }

    private void OnPlaying()
    {
        StartCoroutine(map.SpawningObstacles(obstaclesDelay));
    }

    private void OnRestarting()
    {
        StopAllCoroutines();

        Invoke(nameof(ActivateRestart), restartGameDelay);
    }

    public void StartWaiting() 
    {
        SetGameState(GameState.Waiting);
    }

    private void RestartScene()
    {
        SCENE.LoadScene(SCENE.GetActiveScene().name);
    }

    private void ActivateRestart() 
    {
        canRestart = true;
    }
}

public enum GameState { Home, Waiting, Playing, Restarting }
