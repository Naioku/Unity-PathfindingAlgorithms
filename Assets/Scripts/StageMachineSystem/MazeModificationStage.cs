using CustomInputSystem;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StageMachineSystem
{
    public class MazeModificationStage : BaseStage
    {
        private InputManager inputManager = AllManagers.Instance.InputManager;
        private Enums.TileType temp;
        private Enums.TileType currentTileTypeToSet
        {
            get => temp;
            set
            {
                temp = value;
                Debug.Log("CurrentTileType: " + temp);
            }
        }

        private Vector2Int? currentCoords;

        public MazeModificationStage(Maze maze) : base(maze) {}
        
        public override void Enter()
        {
            inputManager = AllManagers.Instance.InputManager;
            InitInput();
            InitInteractions();
        }

        public override void Exit()
        {
            RemoveInteractions();
            RemoveInput();
        }
        
        private void InitInput()
        {
            inputManager.SetOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetDefaultNode, StartSettingNodeDefault);
            inputManager.SetOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetStartNode, StartSettingNodeStart);
            inputManager.SetOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetDestinationNode, StartSettingNodeDestination);
            inputManager.SetOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetBlockedNode, StartSettingNodeBlocked);
            inputManager.SetActionMap(Enums.ActionMap.MazeModification);
        }

        private void RemoveInput()
        {
            inputManager.RemoveOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetDefaultNode, StartSettingNodeDefault);
            inputManager.RemoveOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetStartNode, StartSettingNodeStart);
            inputManager.RemoveOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetDestinationNode, StartSettingNodeDestination);
            inputManager.RemoveOnPerformed(Enums.ActionMap.MazeModification, Enums.InputAction.SetBlockedNode, StartSettingNodeBlocked);
        }

        private void StartSettingNodeDefault(InputAction.CallbackContext obj) => currentTileTypeToSet = Enums.TileType.Default;
        private void StartSettingNodeStart(InputAction.CallbackContext obj) => currentTileTypeToSet = Enums.TileType.Start;
        private void StartSettingNodeDestination(InputAction.CallbackContext obj) => currentTileTypeToSet = Enums.TileType.Destination;
        private void StartSettingNodeBlocked(InputAction.CallbackContext obj) => currentTileTypeToSet = Enums.TileType.Blocked;

        #region Interactions

        private void InitInteractions()
        {
            maze.OnHoverTick += HandleHoverTick;
            maze.OnHoverExitInteraction += HandleHoverExitInteraction;
            maze.OnClickEnterType += HandleClickEnterType;
            maze.OnClickTick += HandleClickTick;
            maze.OnClickExitInteraction += HandleClickExitInteraction;
        }

        private void RemoveInteractions()
        {
            maze.OnHoverTick -= HandleHoverTick;
            maze.OnHoverExitInteraction -= HandleHoverExitInteraction;
            maze.OnClickEnterType -= HandleClickEnterType;
            maze.OnClickTick -= HandleClickTick;
            maze.OnClickExitInteraction -= HandleClickExitInteraction;
        }

        private void HandleHoverTick(Vector2Int coords) => ManageTileSelection(coords);
        private void HandleHoverExitInteraction() => ExitInteraction();
        private void HandleClickEnterType() => ManageTileTypeChanging();

        private void HandleClickTick(Vector2Int coords)
        {
            ManageTileSelection(coords);
            ManageTileTypeChanging();
        }

        private void HandleClickExitInteraction() => ExitInteraction();

        private void ManageTileSelection(Vector2Int coords)
        {
            if (currentCoords.HasValue && currentCoords.Value == coords) return;

            if (currentCoords != null)
            {
                maze.DeselectTile(currentCoords.Value);
            }

            currentCoords = coords;
            maze.SelectTile(currentCoords.Value);
        }

        private void ManageTileTypeChanging()
        {
            maze.SetTileType(currentCoords.Value, currentTileTypeToSet);
        }

        private void ExitInteraction()
        {
            maze.DeselectTile(currentCoords.Value);
            currentCoords = null;
        }

        #endregion
    }
}