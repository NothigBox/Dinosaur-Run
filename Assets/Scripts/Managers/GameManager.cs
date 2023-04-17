using UnityEngine;
using SCENE = UnityEngine.SceneManagement.SceneManager;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameMode mode;

    [Space, Header("Speed")]
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
    [SerializeField] private WebManager web;
    [SerializeField] private ScoreManager score;
    [SerializeField] private DinosaurManager dinosaur;
    
    [Space, Header("Ranking")]
    [SerializeField] private bool deleteData;
    [Tooltip("The path in Resources where to find a JSON pre-config for the ranking.")]
    [SerializeField] private string jsonPath;
    [SerializeField] private bool putData;

    private GameStateObserver[] stateObservers;

    private bool canPutData;
    private bool canDeleteData;
    private bool canRestart;
    private float scoreTimer;
    private float gameplayTimer;

    private static GameManager instance;
    public static GameManager Instance => instance;

    public GameMode Mode => mode;
    public GameState State { get; private set; }

    private void Awake()
    {
        instance = this;

        canRestart = false;
        canPutData = true;
        canDeleteData = true;
        scoreTimer = 0f;
        gameplayTimer = 0f;

        Time.timeScale = 1f;

        stateObservers = FindObjectsOfType<GameStateObserver>();
    }

    private void Start()
    {
        dinosaur.OnDead += StopGame;
        score.OnScoreAdded += ui.UpdateScore;

        if (Mode == GameMode.Web)
        {
            web.OnRankingDeleted += () => StartCoroutine(ui.ResetRanking());

            web.OnRankingGet += (rankingInfos) =>
            {
                ui.UpdateRanking(rankingInfos);
                canPutData = true;
                canDeleteData = true;
            };
        }

        SetGameState(Mode == GameMode.Web? GameState.Home : GameState.Waiting);

        if (UIManager.PlayerName != null) StartWaiting();
    }

    private void Update()
    {
        if (Mode == GameMode.Web)
        {
            if (deleteData && canDeleteData)
            {
                deleteData = false;
                canDeleteData = false;

                web.DeleteData();
            }

            if (putData && canPutData)
            {
                putData = false;
                canPutData = false;

                web.PutData(jsonPath);
            }
        }

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
        if (Mode == GameMode.Web) 
        {
            web.GetData();
        }
    }

    private void OnPlaying()
    {
        StartCoroutine(map.SpawningObstacles(obstaclesDelay));
    }

    private void OnRestarting()
    {
        if (Mode == GameMode.Web)
        {
            web.PostData(new RankSlotData(UIManager.PlayerName, score.CurrentScore));
        }

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
public enum GameMode { Local, Web }
