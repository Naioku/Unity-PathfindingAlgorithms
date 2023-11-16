using System;
using DefaultNamespace;
using InteractionSystem;
using UnityEngine;

[Serializable]
public class GameManager
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InteractionController interactionController;

    public void StartGame()
    {
        AllManagers.Instance.InputManager.EnableActionMapPermanent(Enums.ActionMap.Global);
        cameraController.Initialize(Camera.main);
        interactionController.Initialize(Camera.main);
    }
}