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
        Opened = 1,
        Closed = 2,
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
        Tile, Maze, Camera
    }
    
    public enum UISpawned
    {
        Menu,
        HUDMazeModification,
        HUDAlgorithm,
        SettingGroupPanel,
        SettingGroupEntry,
        SettingEntry,
        EntryPermittedDirections,
        EntryLanguages
    }
    
    public enum UIPopupType
    {
        Info, InputFloat, InputInt, InputColor, InputPermittedDirections, InputLanguages, Confirmation
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
        MazeModification, BFS, AStar, Settings
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

    [Flags]
    public enum SettingsReloadingParam
    {
        None = 0,
        Maze = 1 << 0,
        TileColors = 1 << 1,
        Language = 1 << 2
    }

    public enum SettingLoadingParam
    {
        Standard, Init, Reset
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
        MarkerColorOpened,
        MarkerColorClosed,
        MarkerColorPath,
        MarkerColorAlpha,
        AlgorithmStageDelayAfterNewNodeEnqueuing,
        AlgorithmStageDelayAfterNodeChecking,
        AlgorithmStageDelayAfterCursorPositionChange,
        AlgorithmStageDelayAfterPathNodeSetting,
        PermittedDirections,
        Language
    }
    
    public enum SettingGroupName
    {
        BoardSize,
        TileDimensions,
        TileColors,
        MarkerColors,
        AlgorithmStageDelays,
        Miscellaneous
    }

    public enum SettingGroupPanelName
    {
        Maze,
        Algorithms
    }
    
    public enum Language
    {
        Auto,
        English,
        Polish
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

    public enum PopupText
    {
        QuitGameHeader,
        QuitGameMessage,
        SettingsResetToDefaultHeader,
        SettingsResetToDefaultMessage,
        SettingsSavedHeader,
        SettingsSavedMessage,
        ConfirmationButtonYes,
        ConfirmationButtonNo,
        AlgorithmCannotEnterHeader,
        AlgorithmCannotEnterMessage
    }
    
    public enum GeneralText
    {
        ButtonBack,
        HUDCurrentNodeLabel,
        HUDAlgorithmStateLabel,
        SettingsHeader,
        SettingsButtonReset,
        SettingsButtonResetToDefault,
        SettingsButtonSave,
        MainMenuHeader,
        MainMenuButtonQuit
    }
}