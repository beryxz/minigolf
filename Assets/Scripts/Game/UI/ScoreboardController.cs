using System;
using TMPro;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    private int playersCount = 0;
    private int holesCount = 0;

    [Header("UI Elements")]
    [SerializeField] private Transform scoreboard;
    private Transform scoreboardTable;
    private Transform scoreboardHolesNumbers;
    private Transform scoreboardPlayersRows;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabHoleNum;
    [SerializeField] private GameObject prefabHoleScore;
    [SerializeField] private GameObject prefabPlayerRow;

    void Awake()
    {
        if (scoreboard == null) Debug.LogWarning("Missing the Scoreboard component", this);

        scoreboardTable = scoreboard.Find("Board/Table");
        scoreboardHolesNumbers = scoreboardTable.Find("HolesRow/HolesNumbers");
        scoreboardPlayersRows = scoreboardTable.Find("PlayersRows");
        if (scoreboardTable == null) Debug.LogWarning("Scoreboard table not found", this);
        if (scoreboardHolesNumbers == null) Debug.LogWarning("Scoreboard holes not found", this);
        if (scoreboardPlayersRows == null) Debug.LogWarning("Scoreboard players not found", this);
    }

    void Start()
    {
        SetScoreboardVisibility(false);
        ResetScoreboard();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) SetScoreboardVisibility(true);
        if (Input.GetKeyUp(KeyCode.Tab)) SetScoreboardVisibility(false);
    }

    public void SetScoreboardVisibility(bool visible)
    {
        scoreboard.gameObject.SetActive(visible);
    }

    public void SetPlayerScore(int playerIdx, int holeIdx, int score)
    {
        if (playerIdx < 0 || playerIdx >= playersCount) throw new ArgumentException("Invalid player index");
        if (holeIdx < 0 || holeIdx >= holesCount) throw new ArgumentException("Invalid hole index");
        scoreboardPlayersRows
            .GetChild(playerIdx)
            .Find("HolesScore")
            .GetChild(holeIdx)
            .GetComponent<TextMeshProUGUI>()
            .SetText(score.ToString());
    }

    public void SetPlayers(int newPlayersCount)
    {
        GameObject newObj;
        Transform playersHoles;

        if (newPlayersCount < 1 || newPlayersCount > 8) throw new ArgumentException("Invalid new players count, must be between 1-8");
        if (newPlayersCount == playersCount) return;

        if (newPlayersCount > playersCount)
        {
            // create new players
            for (int newPlayer = playersCount + 1; newPlayer <= newPlayersCount; newPlayer++)
            {
                newObj = Instantiate(prefabPlayerRow, scoreboardPlayersRows);
                newObj.transform
                    .Find("PlayerName")
                    .GetComponent<TextMeshProUGUI>()
                    .SetText($"Player {newPlayer}");
                playersHoles = newObj.transform.Find("HolesScore");
                for (int i = 0; i < holesCount; i++)
                {
                    Instantiate(prefabHoleScore, playersHoles)
                        .GetComponent<TextMeshProUGUI>()
                        .SetText("-");
                }
            }
        }
        else if (newPlayersCount < playersCount)
        {
            // remove excess players
            for (int playerIdx = playersCount - 1; playerIdx >= newPlayersCount; playerIdx--)
            {
                Destroy(scoreboardPlayersRows.GetChild(playerIdx).gameObject);
            }
        }
        playersCount = newPlayersCount;
    }

    public void SetHoles(int newHolesCount)
    {
        Transform holesScores;

        if (newHolesCount < 1 || newHolesCount > 100) throw new ArgumentException("Invalid new holes count, must be between 1-100");
        if (newHolesCount == holesCount) return;

        if (newHolesCount > holesCount)
        {
            // create new holes columns
            for (int newHole = holesCount + 1; newHole <= newHolesCount; newHole++)
            {
                Instantiate(prefabHoleNum, scoreboardHolesNumbers)
                    .GetComponent<TextMeshProUGUI>()
                    .SetText(newHole.ToString());
            }
            // add scores to players for new holes
            foreach (Transform playerRow in scoreboardPlayersRows)
            {
                holesScores = playerRow.Find("HolesScore");
                for (int i = holesScores.childCount; i < newHolesCount; i++)
                {
                    Instantiate(prefabHoleScore, holesScores)
                            .GetComponent<TextMeshProUGUI>()
                            .SetText("-");
                }
            }
        }
        else if (newHolesCount < holesCount)
        {
            // remove excess holes columns
            for (int holeIdx = holesCount - 1; holeIdx >= newHolesCount; holeIdx--)
            {
                Destroy(scoreboardHolesNumbers.GetChild(holeIdx).gameObject);
            }
            // remove excess scores from players
            foreach (Transform playerRow in scoreboardPlayersRows)
            {
                holesScores = playerRow.Find("HolesScore");
                for (int holeNum = holesScores.childCount; holeNum > newHolesCount; holeNum--)
                {
                    Destroy(holesScores.GetChild(holeNum - 1).gameObject);
                }
            }
        }

        holesCount = newHolesCount;
    }

    private void ResetScoreboard()
    {
        GameObjectsUtils.ClearChildren(scoreboardHolesNumbers);
        GameObjectsUtils.ClearChildren(scoreboardPlayersRows);
    }
}
