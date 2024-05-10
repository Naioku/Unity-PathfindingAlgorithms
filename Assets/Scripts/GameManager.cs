using System;
using BreadthFirstSearch.Scripts;
using CustomInputSystem;
using InteractionSystem;
using Settings;
using SpawningSystem;
using StageMachineSystem;
using UI;
using UI.Localization;
using UnityEngine;
using Utilities;

[Serializable]
public class GameManager
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InteractionController interactionController;

    private SpawnManager<Enums.UISpawned> uiSpawner;
    private Maze.Maze maze;
    private MenuController menuController;
    private InputManager inputManager;
    private StageMachine stageMachine;
    private MazeModificationStage mazeModificationStage;
    private AlgorithmStage algorithmStage;
    private LocalizedContentCache localizedContentCache;

    public GameSettings GameSettings => gameSettings;

    public void Awake()
    {
        uiSpawner = AllManagers.Instance.UIManager.UISpawner;
        AllManagers.Instance.SavingManager.LoadSettings(gameSettings);
        menuController = uiSpawner.CreateObject<MenuController>(Enums.UISpawned.Menu);
        menuController.Initialize
        (
            StartMazeModification,
            StartBFS,
            StartAStar,
            ResetSettingsToDefault,
            UpdateGameSettings,
            Quit
        );
        inputManager = AllManagers.Instance.InputManager;
        localizedContentCache = new LocalizedContentCache(Enums.PopupText.QuitGameHeader, Enums.PopupText.QuitGameMessage);
        mazeModificationStage = new MazeModificationStage();
        algorithmStage = new AlgorithmStage();
    }

    public void StartGame()
    {
        inputManager.GlobalMap.Enable();
        cameraController = AllManagers.Instance.UtilsSpawner.CreateObject<CameraController>(Enums.SpawnedUtils.Camera);
        interactionController.Initialize(Camera.main);
        maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze.Maze>(Enums.SpawnedUtils.Maze);
        stageMachine = new StageMachine(maze);
    }
    
    public void ExitStage()
    {
        if (!stageMachine.SetStage(null)) return;
        
        menuController.Open();
        cameraController.StopMovement();
        interactionController.StopInteracting();
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

    private void StartMazeModification() => EnterStage(mazeModificationStage);
    private void StartBFS()
    {
        algorithmStage.ChangeAlgorithm(new BFS());
        EnterStage(algorithmStage);
    }

    private void StartAStar()
    {
        algorithmStage.ChangeAlgorithm(new AStar());
        EnterStage(algorithmStage);
    }

    private void ResetSettingsToDefault()
    {
        gameSettings.LoadDefault();
        ReloadMaze();
    }

    private void UpdateGameSettings(Enums.SettingsReloadingParam reloadParam)
    {
        if (Utility.ContainsBit(reloadParam, Enums.SettingsReloadingParam.Maze))
        {
            ReloadMaze();
            cameraController.UpdateScreenLimits();
            cameraController.SetInitPosition();
        }
        else if (Utility.ContainsBit(reloadParam, Enums.SettingsReloadingParam.TileColors))
        {
            ReloadTileColors();
        }
        
        if (Utility.ContainsBit(reloadParam, Enums.SettingsReloadingParam.Language))
        {
            ReloadLanguage();
        }
        
        AllManagers.Instance.SavingManager.SaveSettings();
    }

    private void ReloadMaze()
    {
        AllManagers.Instance.UtilsSpawner.DestroyObject(maze.gameObject);
        maze = AllManagers.Instance.UtilsSpawner.CreateObject<Maze.Maze>(Enums.SpawnedUtils.Maze);
        stageMachine.Maze = maze;
    }

    private void ReloadTileColors() => maze.RefreshTiles();
    private void ReloadLanguage() => AllManagers.Instance.LocalizedTextManager.ReloadLanguage();

    private void Quit()
    {
        AllManagers.Instance.UIManager.OpenPopupConfirmation
        (
            localizedContentCache.GetValue(Enums.PopupText.QuitGameHeader),
            localizedContentCache.GetValue(Enums.PopupText.QuitGameMessage),
            QuitInternal
        );
    }

    private void QuitInternal()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}