using System;
using BreadthFirstSearch.Scripts;
using CustomInputSystem;
using InteractionSystem;
using StageMachineSystem;
using StageMachineSystem.Algorithm;
using UnityEngine;

[Serializable]
public class GameManager
{
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InteractionController interactionController;

    private InputManager inputManager;
    private StageMachine stageMachine;
    private Maze maze;
    
    public GameDataSO GameDataSO => gameDataSO;

    public void Initialize()
    {
        inputManager = AllManagers.Instance.InputManager;
        InitInput();
    }

    public void Destroy()
    {
        RemoveInput();
    }

    private void InitInput()
    {
        inputManager.GlobalMap.OnExitStageData.Performed += ExitStage;
        inputManager.StageSelectionMap.OnMazeModificationData.Performed += StartMazeModification;
        inputManager.StageSelectionMap.OnBFSData.Performed += StartBFS;
        inputManager.StageSelectionMap.OnAStarData.Performed += StartAStar;
        
        inputManager.StageSelectionMap.Enable();
    }

    private void RemoveInput()
    {
        inputManager.StageSelectionMap.Disable();
        
        inputManager.GlobalMap.OnExitStageData.Performed -= ExitStage;
        inputManager.StageSelectionMap.OnMazeModificationData.Performed -= StartMazeModification;
        inputManager.StageSelectionMap.OnBFSData.Performed -= StartBFS;
        inputManager.StageSelectionMap.OnAStarData.Performed -= StartAStar;
    }

    private void ExitStage()
    {
        stageMachine.SetStage(null);
        inputManager.StageSelectionMap.Enable();
    }

    private void StartMazeModification() => stageMachine.SetStage(new MazeModificationStage(maze));
    private void StartBFS() => stageMachine.SetStage(new AlgorithmStage(maze, new BFS()));
    private void StartAStar() => Debug.Log("AStar not implemented yet.");

    public void StartGame()
    {
        inputManager.GlobalMap.Enable();
        cameraController.Initialize(Camera.main);
        interactionController.Initialize(Camera.main);
        maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze>(Enums.Utils.Maze);
        stageMachine = new StageMachine();
    }
}