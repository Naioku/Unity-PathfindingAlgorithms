using System;
using BreadthFirstSearch.Scripts;
using CustomInputSystem;
using DefaultNamespace;
using InteractionSystem;
using StageMachineSystem;
using UnityEngine;
using UnityEngine.InputSystem;

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
        inputManager.SetOnPerformed(Enums.ActionMap.Global, Enums.InputAction.ExitStage, ExitStage);
        inputManager.SetOnPerformed(Enums.ActionMap.StageSelection, Enums.InputAction.MazeModification, StartMazeModification);
        inputManager.SetOnPerformed(Enums.ActionMap.StageSelection, Enums.InputAction.BFS, StartBFS);
        inputManager.SetOnPerformed(Enums.ActionMap.StageSelection, Enums.InputAction.AStar, StartAStar);
        inputManager.SetActionMap(Enums.ActionMap.StageSelection);
    }

    private void RemoveInput()
    {
        inputManager.RemoveOnPerformed(Enums.ActionMap.Global, Enums.InputAction.ExitStage, ExitStage);
        inputManager.RemoveOnPerformed(Enums.ActionMap.StageSelection, Enums.InputAction.MazeModification, StartMazeModification);
        inputManager.RemoveOnPerformed(Enums.ActionMap.StageSelection, Enums.InputAction.BFS, StartBFS);
        inputManager.RemoveOnPerformed(Enums.ActionMap.StageSelection, Enums.InputAction.AStar, StartAStar);
    }

    private void ExitStage(InputAction.CallbackContext obj)
    {
        stageMachine.SetStage(null);
        inputManager.SetActionMap(Enums.ActionMap.StageSelection);
    }

    private void StartMazeModification(InputAction.CallbackContext obj) => stageMachine.SetStage(new MazeModificationStage(maze));
    private void StartBFS(InputAction.CallbackContext obj) => stageMachine.SetStage(new AlgorithmStage(maze, new BFS()));

    private void StartAStar(InputAction.CallbackContext obj)
    {
        Debug.Log("AStar not implemented yet.");
    }

    public void StartGame()
    {
        AllManagers.Instance.InputManager.EnableActionMapPermanent(Enums.ActionMap.Global);
        cameraController.Initialize(Camera.main);
        interactionController.Initialize(Camera.main);
        maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze>(Enums.Utils.Maze);
        stageMachine = new StageMachine();
    }
}