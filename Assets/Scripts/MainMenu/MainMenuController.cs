using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Rendering Settings")]
    [SerializeField] private int MAX_FRAME_RATE = 60;

    [Header("Game Settings - Players")]
    [SerializeField] private int playerCountMax = 4;
    [SerializeField] private Button btnPlayerAdd;
    [SerializeField] private Button btnPlayerRemove;
    [SerializeField] private Transform playersRowsContainer;
    [SerializeField] private GameObject playerRowPrefab;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = MAX_FRAME_RATE;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetupPlayersRows();
        FixPlayersButtonsVisiblity();
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(StateManager.SelectedSceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PlayerAdd()
    {
        if (StateManager.PlayersCount >= playerCountMax) return;
        StateManager.PlayersCount += 1;
        PlayerAddRow(StateManager.PlayersCount);
        FixPlayersButtonsVisiblity();
    }
    public void PlayerRemove()
    {
        if (StateManager.PlayersCount <= 1) return;
        StateManager.PlayersCount -= 1;
        PlayerRemoveRow(StateManager.PlayersCount);
        FixPlayersButtonsVisiblity();
    }
    public void FixPlayersButtonsVisiblity()
    {
        if (StateManager.PlayersCount == 1)
        {
            btnPlayerAdd.interactable = true;
            btnPlayerRemove.interactable = false;
        }
        else if (StateManager.PlayersCount == playerCountMax)
        {
            btnPlayerAdd.interactable = false;
            btnPlayerRemove.interactable = true;
        }
        else
        {
            btnPlayerAdd.interactable = true;
            btnPlayerRemove.interactable = true;
        }
    }

    private void PlayerRemoveRow(int playerIdx) {
        Destroy(playersRowsContainer.GetChild(playerIdx).gameObject);
    }

    private void PlayerAddRow(int playerNumber) {
        Instantiate(playerRowPrefab, playersRowsContainer).transform
            .Find("PlayerRowUsername")
            .GetComponent<TextMeshProUGUI>()
            .SetText($"Player {playerNumber}");
    }

    private void SetupPlayersRows()
    {
        GameObjectsUtils.ClearChildren(playersRowsContainer);
        for (int playerNum = 1; playerNum <= StateManager.PlayersCount; playerNum++)
        {
            PlayerAddRow(playerNum);
        }
    }
}
