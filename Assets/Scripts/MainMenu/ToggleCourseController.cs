using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleCourseController : MonoBehaviour
{
    private Toggle toggle;
    [SerializeField] private string toggleSceneName;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void Start()
    {
        if (toggle.isOn) ChangeSceneToOurs();
        toggle.onValueChanged.AddListener(value =>
        {
            if (value && toggleSceneName != null) ChangeSceneToOurs();
        });
    }

    private void ChangeSceneToOurs() {
        StateManager.SelectedSceneName = toggleSceneName;
    }
}
