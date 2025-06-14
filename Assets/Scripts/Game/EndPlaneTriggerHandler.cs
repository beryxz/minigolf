using System;
using UnityEngine;

public class EndPlaneTriggerHandler : MonoBehaviour
{
    private GameController gameController;

    void Start()
    {
        gameController = FindFirstObjectByType<GameController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.HandleHoleEndTrigger(this.gameObject);
        }
    }
}
