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
        Global
    }
    
    public enum InputAction
    {
        CameraMovement,
        ClickInteraction
    }

    public enum CameraMovementMode
    {
        Border, Key
    }
}