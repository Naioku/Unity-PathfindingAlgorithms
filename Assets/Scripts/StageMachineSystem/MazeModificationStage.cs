using System.Linq;
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
        private Enums.TileType CurrentTileTypeToSet
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
            base.Enter();
            inputManager = AllManagers.Instance.InputManager;
            InitInput();
            InitInteractions();
        }

        public override void Exit()
        {
            base.Exit();
            RemoveInteractions();
            RemoveInput();
            if (currentCoords != null)
            {
                maze.DeselectTile(currentCoords.Value);
            }
        }

        #region Input

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

        private void StartSettingNodeDefault(InputAction.CallbackContext obj) => CurrentTileTypeToSet = Enums.TileType.Default;
        private void StartSettingNodeStart(InputAction.CallbackContext obj) => CurrentTileTypeToSet = Enums.TileType.Start;
        private void StartSettingNodeDestination(InputAction.CallbackContext obj) => CurrentTileTypeToSet = Enums.TileType.Destination;
        private void StartSettingNodeBlocked(InputAction.CallbackContext obj) => CurrentTileTypeToSet = Enums.TileType.Blocked;

        #endregion

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

        private void HandleHoverTick(Vector2Int coords)
        {
            if (HaveCoordinatesChanged(coords)) return;
            ManageTileSelection(coords);
        }
        private void HandleHoverExitInteraction() => ExitInteraction();
        private void HandleClickEnterType() => ManageTileTypeChanging();
        private void HandleClickTick(Vector2Int coords)
        {
            if (HaveCoordinatesChanged(coords)) return;
            ManageTileSelection(coords);
            ManageTileTypeChanging();
        }
        private void HandleClickExitInteraction() => ExitInteraction();

        private bool HaveCoordinatesChanged(Vector2Int coords)
        {
            return currentCoords.HasValue && currentCoords.Value == coords;
        }
        
        private void ManageTileSelection(Vector2Int coords)
        {
            if (currentCoords != null)
            {
                maze.DeselectTile(currentCoords.Value);
            }

            currentCoords = coords;
            maze.SelectTile(currentCoords.Value);
        }

        private void ManageTileTypeChanging()
        {
            ManageUniqueTileTypesData();
            maze.SetTileType(currentCoords.Value, CurrentTileTypeToSet);
            return;

            void ManageUniqueTileTypesData()
            {
                Enums.TileType[] keys = sharedData.UniqueTilesCoordsLookup.Keys.ToArray();
                foreach (var key in keys)
                {
                    ManageUniqueOneTileTypeData(key);
                }
            }

            void ManageUniqueOneTileTypeData(Enums.TileType tileType)
            {
                Vector2Int? coords = sharedData.UniqueTilesCoordsLookup[tileType];
                if (CurrentTileTypeToSet == tileType)
                {
                    if (coords.HasValue)
                    {
                        maze.SetTileType(coords.Value, Enums.TileType.Default);
                    }
                
                    sharedData.UniqueTilesCoordsLookup[tileType] = currentCoords;
                }
                else if (coords.HasValue && coords.Value == currentCoords.Value)
                {
                    sharedData.UniqueTilesCoordsLookup[tileType] = null;
                }
            }
        }

        private void ExitInteraction()
        {
            maze.DeselectTile(currentCoords.Value);
            currentCoords = null;
        }

        #endregion
    }
}