using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class HoleSegment
{
    public Transform start;
    public Transform startDirection;
    public Transform end;
}

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ScoreboardController))]
[RequireComponent(typeof(PlayerUIController))]
[RequireComponent(typeof(GameSplashScreenController))]
[RequireComponent(typeof(PauseMenuController))]
public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private HoleSegment[] holes;
    private int currentHoleIdx;
    private int currentPlayerIdx;
    [SerializeField] private Transform playersContainer;
    [SerializeField] private GameObject playerPrefab;
    private PlayerController[] players;
    private int[,] playersScores;
    private bool[] playersFinished;

    private ScoreboardController scoreboard;
    private PlayerUIController playerUI;
    private GameSplashScreenController gameSplashScreen;
    private PauseMenuController pauseMenu;

    public bool IsPaused { get; private set; }

    private CameraTracker mainCamera;

    [Header("Sound Settings")]
    [SerializeField] private SoundEvent playerCompletedHoleSoundEvent;
    [SerializeField] private SoundEvent playerTurnStartSoundEvent;
    [SerializeField] private SoundEvent bgAmbienceMusicSoundEvent;
    private AudioSource audioSource;

    [Header("Render Settings")]
    [SerializeField] private int MAX_FRAME_RATE = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MAX_FRAME_RATE;

        audioSource = GetComponent<AudioSource>();
        scoreboard = GetComponent<ScoreboardController>();
        playerUI = GetComponent<PlayerUIController>();
        gameSplashScreen = GetComponent<GameSplashScreenController>();
        pauseMenu = GetComponent<PauseMenuController>();

        currentHoleIdx = -1;
        currentPlayerIdx = -1;
        IsPaused = false;
    }

    void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraTracker>();
        if (mainCamera == null) Debug.LogWarning("Missing CameraTracker on the Main Camera", this);

        bgAmbienceMusicSoundEvent.Play(audioSource);

        // set default states for camera and menus
        mainCamera.SetActive(false);
        scoreboard.SetScoreboardVisibility(false);
        playerUI.SetPlayerUIVisibility(false);
        gameSplashScreen.HideAllSplashScreens();
        pauseMenu.SetPauseMenuUIVisibility(false);

        // set default cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        NewGame();
    }

    void Update()
    {
        // DEBUG
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeHole(currentHoleIdx - 1);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeHole(currentHoleIdx + 1);
        }

        if (!gameSplashScreen.IsAnyActive() && Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!IsPaused);
        }
    }

    public void SetPaused(bool isPaused)
    {
        IsPaused = isPaused;
        pauseMenu.SetPauseMenuUIVisibility(isPaused);
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void NewGame()
    {
        gameSplashScreen.SetSplashScreen(GameSplashScreen.GAME_START);
        Invoke(nameof(_NewGame), 4.0f);
    }

    private void _NewGame()
    {
        if (StateManager.PlayersCount < 1 || StateManager.PlayersCount > 8) throw new ArgumentException("Invalid player count. Allowed range 1-8");

        currentHoleIdx = -1;
        currentPlayerIdx = -1;

        gameSplashScreen.SetSplashScreen(GameSplashScreen.NONE);

        // create players
        GameObjectsUtils.ClearChildren(playersContainer);
        players = new PlayerController[StateManager.PlayersCount];
        for (int i = 0; i < StateManager.PlayersCount; i++)
        {
            players[i] = Instantiate(playerPrefab, playersContainer).GetComponent<PlayerController>();
        }

        playersScores = new int[StateManager.PlayersCount, holes.Length];
        scoreboard.SetPlayers(StateManager.PlayersCount);
        scoreboard.SetHoles(holes.Length);

        playerUI.SetPlayerUIVisibility(true);

        HandleTurnChange();
    }

    public void EndGame()
    {
        gameSplashScreen.SetSplashScreen(GameSplashScreen.NONE);
        scoreboard.SetScoreboardVisibility(true);
        IEnumerator LoadMainMenu()
        {
            yield return new WaitForSeconds(15);
            SceneManager.LoadScene("MainMenu");
        }
        StartCoroutine(LoadMainMenu());
    }

    private void HandleTurnChange()
    {
        if (currentHoleIdx < 0)
        {
            ChangeHole(0);
            ChangeActivePlayer(0);
        }
        else
        {
            // find next player who has not finished
            int nextPlayerIdx = -1;
            int nextIdx;
            for (int i = 1; i <= players.Length; i++)
            {
                nextIdx = (currentPlayerIdx + i) % players.Length;
                if (!playersFinished[nextIdx])
                {
                    nextPlayerIdx = nextIdx;
                    break;
                }
            }
            // if some player has still to finish
            if (nextPlayerIdx > -1)
            {
                // change to next unfinished player
                ChangeActivePlayer(nextPlayerIdx);
            }
            // if all have finished and there are no more holes
            else if (currentHoleIdx + 1 >= holes.Length)
            {
                EndGame();
            }
            // else, change to next hole
            else
            {
                ChangeHole(currentHoleIdx + 1);
            }
        }
    }

    private void ChangeActivePlayer(int newPlayerIdx)
    {
        if (newPlayerIdx >= players.Length) return;
        if (newPlayerIdx < 0) return;

        // Update UI
        playerUI.SetPlayerName($"Player {newPlayerIdx + 1}");
        playerUI.SetThrowCounterText(playersScores[newPlayerIdx, currentHoleIdx]);
        // Update Camera
        mainCamera.SetTrackedTarget(players[newPlayerIdx].ball.transform);
        mainCamera.UpdateCameraPosition();
        // Update indexes
        currentPlayerIdx = newPlayerIdx;

        // if single player, no player change animation
        if (newPlayerIdx == 0 && players.Length == 1)
        {
            playerTurnStartSoundEvent.PlayOneShot(audioSource);
            players[0].SetCurrentTurn(true);
            mainCamera.SetActive(true);
            return;
        }

        // disable all players
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetCurrentTurn(false);
        }
        // show turn change animation
        gameSplashScreen.SetPlayerName($"Player {newPlayerIdx + 1}");
        gameSplashScreen.SetSplashScreen(GameSplashScreen.PLAYER_TURN_START);
        // enable new player turn
        IEnumerator EnableNewPlayerTurn()
        {
            yield return new WaitForSeconds(3);
            gameSplashScreen.SetSplashScreen(GameSplashScreen.NONE);
            playerTurnStartSoundEvent.PlayOneShot(audioSource);
            players[newPlayerIdx].SetCurrentTurn(true);
            mainCamera.SetActive(true);
        }
        StartCoroutine(EnableNewPlayerTurn());
    }

    public void HandlePlayerBallThrow()
    {
        playersScores[currentPlayerIdx, currentHoleIdx] += 1;
        players[currentPlayerIdx].ThrowBall();
        playerUI.SetThrowCounterText(playersScores[currentPlayerIdx, currentHoleIdx]);
    }
    public void HandlePlayerBallStop()
    {
        mainCamera.SetActive(false);

        // if player just finised a hole show splash screen
        if (playersFinished[currentPlayerIdx])
        {
            IEnumerator ChangeTurnAfterAnimation()
            {
                yield return new WaitForSeconds(4);
                gameSplashScreen.SetSplashScreen(GameSplashScreen.NONE);
                HandleTurnChange();
            }
            StartCoroutine(ChangeTurnAfterAnimation());
        }
        else
        {
            HandleTurnChange();
        }
    }

    private void ChangeHole(int newHoleIdx)
    {
        if (newHoleIdx >= holes.Length) return;
        if (newHoleIdx < 0) return;
        if (holes[newHoleIdx] == null || holes[newHoleIdx].start == null || holes[newHoleIdx].startDirection == null) return;

        Debug.Log($"[GC] Changing to hole {newHoleIdx + 1}");

        currentHoleIdx = newHoleIdx;

        foreach (PlayerController p in players)
        {
            p.ball.ResetBall();
            p.ball.transform.position = holes[newHoleIdx].start.position;
            p.ball.SetPointingDirection(holes[newHoleIdx].startDirection.position);
        }

        playersFinished = new bool[players.Length];
        Array.Fill(playersFinished, false);

        ChangeActivePlayer(0);
    }

    public void HandleHoleEndTrigger(GameObject endPlaneCollider)
    {
        if (endPlaneCollider != holes[currentHoleIdx].end.gameObject) return;

        // update values
        playersFinished[currentPlayerIdx] = true;
        scoreboard.SetPlayerScore(currentPlayerIdx, currentHoleIdx, playersScores[currentPlayerIdx, currentHoleIdx]);
        // set splash screen
        gameSplashScreen.SetHoleNumber(currentHoleIdx + 1);
        gameSplashScreen.SetStrokesCount(playersScores[currentPlayerIdx, currentHoleIdx]);
        gameSplashScreen.SetSplashScreen(GameSplashScreen.PLAYER_FINISHED_HOLE);
        // play sound
        playerCompletedHoleSoundEvent.PlayOneShot(audioSource);
    }
}
