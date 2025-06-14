using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform pauseMenuUI;

    private GameController gameController;

    void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();

        if (pauseMenuUI == null) Debug.LogWarning("pauseMenuUI component not found", this);
    }

    public void SetPauseMenuUIVisibility(bool visible)
    {
        pauseMenuUI.gameObject.SetActive(visible);
    }

    public void UnpauseGame()
    {
        gameController.SetPaused(false);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
