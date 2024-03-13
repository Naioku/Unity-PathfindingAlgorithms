using System.Linq;
using CustomInputSystem;
using UI.HUDPanels;
using UnityEngine;

namespace StageMachineSystem
{
    public class MazeModificationStage : BaseStage
    {
        private InputManager inputManager;
        private Maze maze;
        private readonly HUDControllerMazeModification hudControllerMazeModification;
        
        private Enums.TileType currentTileTypeToSet;
        private Enums.TileType CurrentTileTypeToSet
        {
            get => currentTileTypeToSet;
            set
            {
                currentTileTypeToSet = value;
                hudControllerMazeModification.UpdateCurrentNodeLabel(value);
            }
        }

        private Vector2Int? currentCoords;

        public MazeModificationStage(HUDControllerMazeModification hudControllerMazeModification)
        {
            this.hudControllerMazeModification = hudControllerMazeModification;
        }
        
        public override void Enter()
        {
            base.Enter();
            inputManager = AllManagers.Instance.InputManager;
            maze = sharedData.Maze;
            InitInput();
            InitInteractions();
            hudControllerMazeModification.Initialize
            (
                StartSettingNodeDefault,
                StartSettingNodeStart,
                StartSettingNodeDestination,
                StartSettingNodeBlocked,
                sharedData.OnBack
            );
            hudControllerMazeModification.Show();
        }

        public override void Exit()
        {
            base.Exit();
            hudControllerMazeModification.Hide();
            hudControllerMazeModification.Deinitialize();
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
            inputManager.MazeModificationMap.OnSetDefaultNodeData.Performed += StartSettingNodeDefault;
            inputManager.MazeModificationMap.OnSetStartNodeData.Performed += StartSettingNodeStart;
            inputManager.MazeModificationMap.OnSetDestinationNodeData.Performed += StartSettingNodeDestination;
            inputManager.MazeModificationMap.OnSetBlockedNodeData.Performed += StartSettingNodeBlocked;

            inputManager.MazeModificationMap.Enable();
        }

        private void RemoveInput()
        {
            inputManager.MazeModificationMap.OnSetDefaultNodeData.Performed -= StartSettingNodeDefault;
            inputManager.MazeModificationMap.OnSetStartNodeData.Performed -= StartSettingNodeStart;
            inputManager.MazeModificationMap.OnSetDestinationNodeData.Performed -= StartSettingNodeDestination;
            inputManager.MazeModificationMap.OnSetBlockedNodeData.Performed -= StartSettingNodeBlocked;
        }

        private void StartSettingNodeDefault() => CurrentTileTypeToSet = Enums.TileType.Default;
        private void StartSettingNodeStart() => CurrentTileTypeToSet = Enums.TileType.Start;
        private void StartSettingNodeDestination() => CurrentTileTypeToSet = Enums.TileType.Destination;
        private void StartSettingNodeBlocked() => CurrentTileTypeToSet = Enums.TileType.Blocked;

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