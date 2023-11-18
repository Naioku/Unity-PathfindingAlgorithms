using System;

public static class Enums
{
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

    public enum Scene
    {
        Menu, BFS, AStar
    }
    
    public enum EmptyEnum {}

    public enum Utils
    {
        Tile, Maze
    }

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

    public enum CameraMovementMode
    {
        Border, Key
    }
    
    [Flags]
    public enum TileViewUpdateParam
    {
        Material = 1 << 0,
        Highlight = 1 << 1
    }
}