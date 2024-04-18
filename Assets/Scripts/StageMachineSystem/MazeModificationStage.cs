﻿using System.Collections.Generic;
using System.Linq;
using CustomInputSystem;
using CustomInputSystem.ActionMaps;
using UI.HUDPanels;
using UnityEngine;

namespace StageMachineSystem
{
    public class MazeModificationStage : BaseStage
    {
        private InputManager inputManager;
        private Maze.Maze maze;
        private readonly HUDControllerMazeModification hudController;
        
        private Enums.TileType currentTileTypeToSet;
        private Enums.TileType CurrentTileTypeToSet
        {
            get => currentTileTypeToSet;
            set
            {
                currentTileTypeToSet = value;
                hudController.UpdateCurrentNodeLabel(value);
            }
        }

        private Vector2Int? currentCoords;
        private ActionMap.ActionData inputOnSetDefaultNodeData;
        private ActionMap.ActionData inputOnSetStartNodeData;
        private ActionMap.ActionData inputOnSetDestinationNodeData;
        private ActionMap.ActionData inputOnSetBlockedNodeData;

        public MazeModificationStage()
        {
            hudController = AllManagers.Instance.UIManager.HUDControllerMazeModification;
            InitInput();
        }

        public override void Enter()
        {
            base.Enter();
            maze = sharedData.Maze;
            InitInput();
            AddInput();
            InitInteractions();
        
            hudController.Initialize
            (
                new ButtonData{ Action = ExitStage, Label = $"Back ({inputOnExitStageData.Binding})" },
                new Dictionary<Enums.TileType, ButtonData>
                {
                    { Enums.TileType.Default, new ButtonData { Action = StartSettingNodeDefault, Label = $"Default ({inputOnSetDefaultNodeData.Binding})" } },
                    { Enums.TileType.Start, new ButtonData { Action = StartSettingNodeStart, Label = $"Start ({inputOnSetStartNodeData.Binding})" } },
                    { Enums.TileType.Destination, new ButtonData { Action = StartSettingNodeDestination, Label = $"Destination ({inputOnSetDestinationNodeData.Binding})" } },
                    { Enums.TileType.Blocked, new ButtonData { Action = StartSettingNodeBlocked, Label = $"Blocked ({inputOnSetBlockedNodeData.Binding})" } },
                }
            );
            hudController.Show();
        }

        public override void Exit()
        {
            base.Exit();
            hudController.Hide();
            hudController.Deinitialize();
            RemoveInteractions();
            RemoveInput();
            if (currentCoords != null)
            {
                maze.DeselectTile(currentCoords.Value);
            }
        }
        
        private void StartSettingNodeDefault() => CurrentTileTypeToSet = Enums.TileType.Default;
        private void StartSettingNodeStart() => CurrentTileTypeToSet = Enums.TileType.Start;
        private void StartSettingNodeDestination() => CurrentTileTypeToSet = Enums.TileType.Destination;
        private void StartSettingNodeBlocked() => CurrentTileTypeToSet = Enums.TileType.Blocked;

        #region Input

        private void InitInput()
        {
            inputManager = AllManagers.Instance.InputManager;

            inputOnSetDefaultNodeData = inputManager.MazeModificationMap.OnSetDefaultNodeData;
            inputOnSetStartNodeData = inputManager.MazeModificationMap.OnSetStartNodeData;
            inputOnSetDestinationNodeData = inputManager.MazeModificationMap.OnSetDestinationNodeData;
            inputOnSetBlockedNodeData = inputManager.MazeModificationMap.OnSetBlockedNodeData;
        }
        
        private void AddInput()
        {
            inputOnSetDefaultNodeData.Performed += InputStartSettingNodeDefault;
            inputOnSetStartNodeData.Performed += InputStartSettingNodeStart;
            inputOnSetDestinationNodeData.Performed += InputStartSettingNodeDestination;
            inputOnSetBlockedNodeData.Performed += InputStartSettingNodeBlocked;

            inputManager.MazeModificationMap.Enable();
        }

        private void RemoveInput()
        {
            inputOnSetDefaultNodeData.Performed -= InputStartSettingNodeDefault;
            inputOnSetStartNodeData.Performed -= InputStartSettingNodeStart;
            inputOnSetDestinationNodeData.Performed -= InputStartSettingNodeDestination;
            inputOnSetBlockedNodeData.Performed -= InputStartSettingNodeBlocked;
        }

        private void InputStartSettingNodeDefault()
        {
            StartSettingNodeDefault();
            hudController.SelectButton(Enums.TileType.Default);
        }
        
        private void InputStartSettingNodeStart()
        {
            StartSettingNodeStart();
            hudController.SelectButton(Enums.TileType.Start);
        }
        
        private void InputStartSettingNodeDestination()
        {
            StartSettingNodeDestination();
            hudController.SelectButton(Enums.TileType.Destination);
        }
        
        private void InputStartSettingNodeBlocked()
        {
            StartSettingNodeBlocked();
            hudController.SelectButton(Enums.TileType.Blocked);
        }

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