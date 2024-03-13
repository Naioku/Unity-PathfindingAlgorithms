using System;
using BreadthFirstSearch.Scripts;
using CustomInputSystem;
using InteractionSystem;
using StageMachineSystem;
using UI;
using UI.HUDPanels;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class GameManager
{
    [SerializeField] private MenuController menuPrefab;
    [SerializeField] private HUDControllerMazeModification hudMazeModificationPrefab;
    [SerializeField] private HUDControllerAlgorithm hudAlgorithmPrefab;
    [SerializeField] private GameDataSO gameDataSO;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InteractionController interactionController;

    private MenuController menuController;
    private HUDControllerMazeModification hudControllerMazeModification;
    private HUDControllerAlgorithm hudControllerAlgorithm;
    private InputManager inputManager;
    private StageMachine stageMachine;
    
    public GameDataSO GameDataSO => gameDataSO;

    public void Initialize()
    {
        menuController = Object.Instantiate(menuPrefab);
        menuController.Initialize(
            StartMazeModification,
            StartBFS,
            StartAStar,
            Quit);
        hudControllerMazeModification = Object.Instantiate(hudMazeModificationPrefab);
        hudControllerAlgorithm = Object.Instantiate(hudAlgorithmPrefab);
        inputManager = AllManagers.Instance.InputManager;
        InitInput();
    }

    public void Destroy()
    {
        RemoveInput();
        cameraController.Destroy();
    }
    
    public void StartGame()
    {
        inputManager.GlobalMap.Enable();
        cameraController.Initialize(Camera.main);
        interactionController.Initialize(Camera.main);
        Maze maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze>(Enums.Utils.Maze);
        stageMachine = new StageMachine(maze, ExitStage);
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

    private void EnterStage(BaseStage stage)
    {
        if (!stageMachine.SetStage(stage)) return;

        cameraController.StartMovement();
        interactionController.StartInteracting();
        menuController.Close();
    }

    private void ExitStage()
    {
        if (!stageMachine.SetStage(null)) return;
        
        menuController.Open();
        cameraController.StopMovement();
        interactionController.StopInteracting();
        inputManager.StageSelectionMap.Enable();
    }

    private void StartMazeModification() => EnterStage(new MazeModificationStage(hudControllerMazeModification));
    private void StartBFS() => EnterStage(new AlgorithmStage(hudControllerAlgorithm, new BFS()));
    private void StartAStar() => Debug.Log("AStar not implemented yet.");
    
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}