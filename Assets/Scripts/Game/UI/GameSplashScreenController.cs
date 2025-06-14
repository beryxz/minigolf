using TMPro;
using UnityEngine;

public enum GameSplashScreen
{
    NONE,
    GAME_START,
    PLAYER_TURN_START,
    PLAYER_FINISHED_HOLE,
}

public class GameSplashScreenController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform gameStartSplashScreen;
    [SerializeField] private Transform playerTurnSplashScreen;
    [SerializeField] private Transform playerFinishedSplashScreen;

    private TextMeshProUGUI playerTurnName;
    private TextMeshProUGUI playerFinishedHoleNumber;
    private TextMeshProUGUI playerFinishedStrokesCount;

    private bool isAnyScreenActive;


    void Awake()
    {
        if (gameStartSplashScreen == null) Debug.LogWarning("gameStartSplashScreen component not found", this);
        if (playerTurnSplashScreen == null) Debug.LogWarning("playerTurnSplashScreen component not found", this);
        if (playerFinishedSplashScreen == null) Debug.LogWarning("playerFinishedSplashScreen component not found", this);

        playerTurnName = playerTurnSplashScreen.Find("txtPlayerName").GetComponent<TextMeshProUGUI>();
        playerFinishedHoleNumber = playerFinishedSplashScreen.Find("txtHoleNumber").GetComponent<TextMeshProUGUI>();
        playerFinishedStrokesCount = playerFinishedSplashScreen.Find("txtStrokesCount").GetComponent<TextMeshProUGUI>();
        if (playerTurnName == null) Debug.LogWarning("playerTurnName not found", this);
        if (playerFinishedHoleNumber == null) Debug.LogWarning("playerFinishedHoleNumber not found", this);
        if (playerFinishedStrokesCount == null) Debug.LogWarning("playerFinishedStrokesCount not found", this);

        isAnyScreenActive = false;
    }

    public bool IsAnyActive()
    {
        return isAnyScreenActive;
    }

    public void HideAllSplashScreens()
    {
        gameStartSplashScreen.gameObject.SetActive(false);
        playerTurnSplashScreen.gameObject.SetActive(false);
        playerFinishedSplashScreen.gameObject.SetActive(false);
        isAnyScreenActive = false;
    }

    public void SetSplashScreen(GameSplashScreen screen)
    {
        HideAllSplashScreens();
        switch (screen)
        {
            case GameSplashScreen.GAME_START:
                isAnyScreenActive = true;
                gameStartSplashScreen.gameObject.SetActive(true);
                break;
            case GameSplashScreen.PLAYER_TURN_START:
                isAnyScreenActive = true;
                playerTurnSplashScreen.gameObject.SetActive(true);
                break;
            case GameSplashScreen.PLAYER_FINISHED_HOLE:
                isAnyScreenActive = true;
                playerFinishedSplashScreen.gameObject.SetActive(true);
                break;
        }
    }

    public void SetPlayerName(string playerName)
    {
        playerTurnName.SetText(
            playerName.Length <= 14
                ? playerName.ToUpper()
                : playerName.ToUpper()[..14]);
    }

    public void SetHoleNumber(int holeNumber)
    {
        playerFinishedHoleNumber.SetText($"HOLE {holeNumber}");
    }

    public void SetStrokesCount(int strokesCount)
    {
        playerFinishedStrokesCount.SetText($"{strokesCount} {(strokesCount == 1 ? "STROKE" : "STROKES")}");
    }
}
