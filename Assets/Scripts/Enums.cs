using System;

public static class Enums
{
    #region Tiles

    public enum TileType
    {
        Default,
        Blocked,
        Start,
        Destination
    }
    
    public enum MarkerType
    {
        None,
        ReadyToCheck,
        Checked,
        Path
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

    public enum Utils
    {
        Tile, Maze
    }

    #endregion

    // Todo: Delete it.
    #region InputSystem

    public enum ActionMap
    {
        Global, StageSelection, MazeModification, Algorithm
    }
    
    public enum InputAction
    {
        CameraMovement,
        ClickInteraction,
        ExitStage,
        MazeModification,
        BFS,
        AStar,
        SetStartNode,
        SetDestinationNode,
        SetBlockedNode,
        SetDefaultNode,
        Play,
        Pause,
        Step,
        Refresh,
        Stop
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

    #endregion
    
    public enum CameraMovementMode
    {
        Border, Key
    }

    public enum WaitingTime
    {
        AfterNewNodeEnqueuing,
        AfterNodeChecking,
        AfterCursorPositionChange,
        AfterPathNodeSetting
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