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

    public enum Scene // Todo: Not needed - delete it.
    {
        Menu, BFS, AStar
    }

    #region SpawningSystem

    public enum EmptyEnum {}

    public enum Utils
    {
        Tile, Maze
    }

    #endregion

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

    public enum CameraMovementMode
    {
        Border, Key
    }
}