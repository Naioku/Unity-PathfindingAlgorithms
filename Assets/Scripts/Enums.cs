using System;

public static class Enums
{
    #region Tiles

    public enum TileType
    {
        Default = 0,
        Start = 1,
        Destination = 2,
        Blocked = 3
    }
    
    public enum MarkerType
    {
        None = 0,
        ReadyToCheck = 1,
        Checked = 2,
        Path = 3
    }
    
    [Flags]
    public enum TileViewUpdateParam
    {
        Material = 1 << 0,
        Highlight = 1 << 1
    }

    #endregion

    #region InteractionSystem

    public enum InteractionType
    {
        Hover, Click
    }
    
    public enum InteractionState
    {
        Tick,
        EnterType,
        ExitType,
        EnterInteraction,
        ExitInteraction
    }

    #endregion

    #region SpawningSystem

    public enum EmptyEnum {}

    public enum SpawnedUtils
    {
        Tile, Maze
    }
    
    public enum UISpawned
    {
        Menu,
        HUDMazeModification,
        HUDAlgorithm,
        SettingGroupPanel,
        SettingGroupEntry,
        SettingEntry
    }
    
    public enum UIPopupType
    {
        Info, InputFloat, InputInt, InputColor, InputChoice, Confirmation
    }

    #endregion

    #region UI

    public enum AlgorithmState
    {
        Initial, Playing, Paused, Finished
    }

    public enum AlgorithmAction
    {
        Play, Pause, Step, Stop
    }
    
    public enum MainMenuPanelButtonTag
    {
        MazeModification, BFS, AStar, Settings, Help
    }
    
    public enum Direction
    {
        Backward = -1,
        Forward = 1,
    }
    
    public enum ButtonsNaviDirection
    {
        Up, Down, Left, Right
    }

    #endregion

    #region Settings

    public enum SettingsReloadingParam
    {
        None, Maze, TileColors
    }
    
    #endregion
    
    public enum CameraMovementMode
    {
        Border, Key
    }

    public enum AlgorithmStageDelay
    {
        AfterNewNodeEnqueuing = 0,
        AfterNodeChecking = 1,
        AfterCursorPositionChange = 2,
        AfterPathNodeSetting = 3
    }

    public enum SettingName
    {
        BoardWidth,
        BoardLength,
        TileDimensionLength,
        TileDimensionHeight,
        TileColorDefault,
        TileColorStart,
        TileColorDestination,
        TileColorBlocked,
        TileColorHighlightValue,
        MarkerColorNone,
        MarkerColorReadyToCheck,
        MarkerColorChecked,
        MarkerColorPath,
        MarkerColorAlpha,
        AlgorithmStageDelayAfterNewNodeEnqueuing,
        AlgorithmStageDelayAfterNodeChecking,
        AlgorithmStageDelayAfterCursorPositionChange,
        AlgorithmStageDelayAfterPathNode,
        PermittedDirections
    }
    
    public enum SettingGroupStaticKey
    {
        BoardSize,
        TileDimensions,
        TileColors,
        MarkerColors,
        AlgorithmStageDelays,
        PermittedDirections
    }

    public enum SettingGroupPanelStaticKey
    {
        Tiles,
        Algorithms
    }
    
    public enum PermittedDirection
    {
        Up,
        Down,
        Left,
        Right,
        UpRight,
        DownRight,
        DownLeft,
        UpLeft
    }
}