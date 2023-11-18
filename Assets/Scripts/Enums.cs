using System;

public static class Enums
{
    #region Tiles

    public enum TileType
    {
        Blocked,
        ReadyToCheck,
        Checked,
        Path,
        Default,
        Start,
        Destination
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

    public enum Scene
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
        Global, MazeModification
    }
    
    public enum InputAction
    {
        CameraMovement,
        ClickInteraction,
        SetStartNode,
        SetDestinationNode,
        SetBlockedNode,
        SetDefaultNode
    }

    #endregion

    public enum CameraMovementMode
    {
        Border, Key
    }
}