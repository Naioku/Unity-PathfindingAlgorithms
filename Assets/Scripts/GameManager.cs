using System;
using BreadthFirstSearch.Scripts;
using CustomInputSystem;
using InteractionSystem;
using StageMachineSystem;
using StageMachineSystem.Algorithm;
using UI;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class GameManager
{
    [SerializeField] private UIMenuController uiMenuControllerPrefab;
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InteractionController interactionController;

    private UIMenuController uiMenuController;
    private InputManager inputManager;
    private StageMachine stageMachine;
    private Maze maze;
    
    public GameDataSO GameDataSO => gameDataSO;

    public void Initialize()
    {
        uiMenuController = Object.Instantiate(uiMenuControllerPrefab);
        uiMenuController.Initialize(
            StartMazeModification,
            StartBFS,
            StartAStar,
            Quit);
        inputManager = AllManagers.Instance.InputManager;
        InitInput();
    }

    public void Destroy()
    {
        RemoveInput();
        cameraController.Destroy();
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
        if (!stageMachine.SetStage(null)) return;
        
        uiMenuController.Open();
        cameraController.StopMovement();
        interactionController.StopInteracting();
        inputManager.StageSelectionMap.Enable();
    }
    
    private void EnterStage(BaseStage stage)
    {
        if (!stageMachine.SetStage(stage)) return;

        cameraController.StartMovement();
        interactionController.StartInteracting();
        uiMenuController.Close();
    }

    private void StartMazeModification() => EnterStage(new MazeModificationStage(maze));

    private void StartBFS() => EnterStage(new AlgorithmStage(maze, new BFS()));

    private void StartAStar() => Debug.Log("AStar not implemented yet.");
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void StartGame()
    {
        inputManager.GlobalMap.Enable();
        cameraController.Initialize(Camera.main);
        interactionController.Initialize(Camera.main);
        maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze>(Enums.Utils.Maze);
        stageMachine = new StageMachine();
    }
}