using System;
using BreadthFirstSearch.Scripts;
using CustomInputSystem;
using InteractionSystem;
using Settings;
using StageMachineSystem;
using UI;
using UnityEngine;

[Serializable]
public class GameManager
{
    [SerializeField] private DefaultSettingsSO defaultSettingsSO;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InteractionController interactionController;

    private Maze maze;
    private GameSettings gameSettings;
    private MenuController menuController;
    private InputManager inputManager;
    private StageMachine stageMachine;
    
    public GameSettings GameSettings => gameSettings;

    public void Initialize()
    {
        defaultSettingsSO.Initialize();
        gameSettings = defaultSettingsSO.Settings;
        menuController = AllManagers.Instance.UIManager.MenuController;
        menuController.Initialize
        (
            StartMazeModification,
            StartBFS,
            StartAStar,
            UpdateGameSettings,
            Quit
        );
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
        maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze>(Enums.SpawnedUtils.Maze);
        stageMachine = new StageMachine(maze);
    }
    
    public void ExitStage()
    {
        if (!stageMachine.SetStage(null)) return;
        
        menuController.Open();
        cameraController.StopMovement();
        interactionController.StopInteracting();
        inputManager.StageSelectionMap.Enable();
    }

    private void InitInput()
    {
        inputManager.StageSelectionMap.OnMazeModificationData.Performed += StartMazeModification;
        inputManager.StageSelectionMap.OnBFSData.Performed += StartBFS;
        inputManager.StageSelectionMap.OnAStarData.Performed += StartAStar;
        
        inputManager.StageSelectionMap.Enable();
    }

    private void RemoveInput()
    {
        inputManager.StageSelectionMap.Disable();
        
        inputManager.StageSelectionMap.OnMazeModificationData.Performed -= StartMazeModification;
        inputManager.StageSelectionMap.OnBFSData.Performed -= StartBFS;
        inputManager.StageSelectionMap.OnAStarData.Performed -= StartAStar;
    }

    private void EnterStage(BaseStage stage)
    {
        if (!stageMachine.SetStage(stage))
        {
            return;
        }

        cameraController.StartMovement();
        interactionController.StartInteracting();
        menuController.Close();
    }

    private void StartMazeModification() => EnterStage(new MazeModificationStage());
    private void StartBFS() => EnterStage(new AlgorithmStage(new BFS()));
    private void StartAStar() => Debug.Log("AStar not implemented yet.");
    private void UpdateGameSettings(GameSettings settings)
    {
        gameSettings = settings;
        AllManagers.Instance.UtilsSpawner.DestroyObject(maze.gameObject);
        maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze>(Enums.SpawnedUtils.Maze);
        stageMachine.Maze = maze;
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}