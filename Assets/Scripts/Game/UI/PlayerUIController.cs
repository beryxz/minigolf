using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform playerUI;
    private Scrollbar powerMeter;
    private TextMeshProUGUI throwCounter;
    private TextMeshProUGUI playerName;

    void Awake()
    {
        if (playerUI == null) Debug.LogWarning("PlayerUI component not found", this);

        powerMeter = playerUI.Find("PowerMeter/PowerMeterFG").GetComponent<Scrollbar>();
        throwCounter = playerUI.Find("ThrowsCounter/ThrowsCounterFG").GetComponent<TextMeshProUGUI>();
        playerName = playerUI.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        if (powerMeter == null) Debug.LogWarning("PowerMeterFG not found", this);
        if (throwCounter == null) Debug.LogWarning("ThrowsCounterFG not found", this);
        if (playerName == null) Debug.LogWarning("PlayerName not found", this);
    }

    void Start()
    {
        SetPlayerUIVisibility(false);
    }

    public void SetPlayerUIVisibility(bool visible)
    {
        playerUI.gameObject.SetActive(visible);
    }

    public void SetThrowCounterText(int newCounter)
    {
        if (newCounter < 0 || newCounter > 1000) throw new ArgumentException("Invalid throwCounter number, must be between 0-1000");
        throwCounter.SetText(newCounter.ToString());
    }

    public void SetPowerMeterLevel(float newLevel)
    {
        if (newLevel < 0f || newLevel > 1f) throw new ArgumentException("Invalid PowerMeter level, must be between 0.0-1.0");
        powerMeter.size = newLevel;
    }

    public void SetPlayerName(string newPlayerName)
    {
        playerName.SetText(newPlayerName);
    }
}
