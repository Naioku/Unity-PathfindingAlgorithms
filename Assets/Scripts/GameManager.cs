using System;
using DefaultNamespace;
using InteractionSystem;
using StageMachineSystem;
using UnityEngine;

[Serializable]
public class GameManager
{
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InteractionController interactionController;
    
    private StageMachine stageMachine;
    
    public GameDataSO GameDataSO => gameDataSO;

    public void StartGame()
    {
        AllManagers.Instance.InputManager.EnableActionMapPermanent(Enums.ActionMap.Global);
        cameraController.Initialize(Camera.main);
        interactionController.Initialize(Camera.main);
        Maze maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze>(Enums.Utils.Maze); // Todo: Create Maze.
        stageMachine = new StageMachine(new MazeModificationStage(maze));
    }
}