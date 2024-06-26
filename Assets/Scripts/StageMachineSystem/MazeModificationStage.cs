﻿using System.Linq;
using CustomInputSystem;
using CustomInputSystem.ActionMaps;
using SpawningSystem;
using UI.HUDPanels;
using UnityEngine;

namespace StageMachineSystem
{
    public class MazeModificationStage : BaseStage
    {
        private readonly SpawnManager<Enums.UISpawned> uiSpawner = AllManagers.Instance.UIManager.UISpawner;
        private readonly HUDControllerMazeModification hudController;
        private InputManager inputManager;
        private Maze.Maze maze;
        private ActionMap.ActionData inputOnSetDefaultNodeData;
        private ActionMap.ActionData inputOnSetStartNodeData;
        private ActionMap.ActionData inputOnSetDestinationNodeData;
        private ActionMap.ActionData inputOnSetBlockedNodeData;
        
        private Enums.TileType currentTileTypeToSet;
        private Vector2Int? currentCoords;

        private Enums.TileType CurrentTileTypeToSet
        {
            get => currentTileTypeToSet;
            set
            {
                currentTileTypeToSet = value;
                hudController.UpdateCurrentNodeLabel(value);
            }
        }

        public MazeModificationStage()
        {
            InitInput();
            hudController = uiSpawner.CreateObject<HUDControllerMazeModification>(Enums.UISpawned.HUDMazeModification);
            hudController.Initialize
            (
                new ButtonData{ Action = ExitStage, Binding = inputOnBackData.Binding },
                new ButtonDataTagged<Enums.TileType>[]
                {
                    new(){ Tag = Enums.TileType.Default, Action = StartSettingNodeDefault, Binding = inputOnSetDefaultNodeData.Binding },
                    new(){ Tag = Enums.TileType.Start, Action = StartSettingNodeStart, Binding = inputOnSetStartNodeData.Binding },
                    new(){ Tag = Enums.TileType.Destination, Action = StartSettingNodeDestination, Binding = inputOnSetDestinationNodeData.Binding },
                    new(){ Tag = Enums.TileType.Blocked, Action = StartSettingNodeBlocked, Binding = inputOnSetBlockedNodeData.Binding }
                }
            );
        }

        public override void Enter()
        {
            base.Enter();
            maze = sharedData.Maze;
            AddInput();
            AddInteractions();
            hudController.Show();
            CurrentTileTypeToSet = Enums.TileType.Default;
        }

        public override void Exit()
        {
            base.Exit();
            hudController.Hide();
            RemoveInteractions();
            RemoveInput();
            if (currentCoords != null)
            {
                maze.DeselectTile(currentCoords.Value);
                currentCoords = null;
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

        private void AddInteractions()
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
                Enums.TileType[] keys = sharedData.Maze.UniqueTilesCoordsLookup.Keys.ToArray();
                foreach (var key in keys)
                {
                    ManageUniqueOneTileTypeData(key);
                }
            }

            void ManageUniqueOneTileTypeData(Enums.TileType tileType)
            {
                Vector2Int? coords = sharedData.Maze.UniqueTilesCoordsLookup[tileType];
                if (CurrentTileTypeToSet == tileType)
                {
                    if (coords.HasValue)
                    {
                        maze.SetTileType(coords.Value, Enums.TileType.Default);
                    }
                
                    sharedData.Maze.UniqueTilesCoordsLookup[tileType] = currentCoords;
                }
                else if (coords.HasValue && coords.Value == currentCoords.Value)
                {
                    sharedData.Maze.UniqueTilesCoordsLookup[tileType] = null;
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