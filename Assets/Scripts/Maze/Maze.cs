﻿using System;
using System.Collections.Generic;
using InteractionSystem;
using Settings;
using UnityEngine;

namespace Maze
{
    public class Maze : MonoBehaviour, IInteractable
    {
        private Transform tilesParent;
        private readonly List<Tile> tileInstances = new();
        private Interaction cursorInteraction;
        
        public Dictionary<Enums.TileType, Vector2Int?> UniqueTilesCoordsLookup { get; set; } = new()
        {
            { Enums.TileType.Start, null },
            { Enums.TileType.Destination, null }
        };
        
        public event Action<Vector2Int> OnHoverTick;
        public event Action OnHoverExitInteraction;
        
        public event Action OnClickEnterType;
        public event Action<Vector2Int> OnClickTick;
        public event Action OnClickExitInteraction;
    
        private void Start()
        {
            CreateGameBoard();
            InitializeInteractions();
        }

        public void Interact(Interaction.InteractionDataSystem interactionDataSystem, Interaction.InteractionDataArgs interactionDataArgs)
        {
            cursorInteraction.Interact(interactionDataSystem, interactionDataArgs);
        }

        public void SelectTile(Vector2Int coords)
        {
            if (!TryGetTile(coords, out Tile tile)) return;
            tile.Select();
        }

        public void DeselectTile(Vector2Int coords)
        {
            if (!TryGetTile(coords, out Tile tile)) return;
            tile.Deselect();
        }

        public void SetTileType(Vector2Int coords, Enums.TileType type)
        {
            if (!TryGetTile(coords, out Tile tile)) return;
            tile.TileType = type;
        }
    
        public void SetMarkerType(Vector2Int coords, Enums.MarkerType type)
        {
            if (!TryGetTile(coords, out Tile tile)) return;
            tile.MarkerType = type;
        }

        /// <summary>
        /// Checks if tile with provided coordinates has the same tile type as provided.
        /// </summary>
        /// <param name="coords">Coordinates of tile You want to check.</param>
        /// <param name="types">Tile types You want to check.</param>
        /// <returns>True if type of the tile with provided coordinates matches one of provided tile types.</returns>
        public bool CheckTileType(Vector2Int coords, params Enums.TileType[] types)
        {
            if (!TryGetTileType(coords, out Enums.TileType currentNodeTileType)) return false;
            foreach (Enums.TileType type in types)
            {
                if (currentNodeTileType == type) return true;
            }
        
            return false;
        }
    
        /// <summary>
        /// Checks if tile with provided coordinates has the same marker type as provided.
        /// </summary>
        /// <param name="coords">Coordinates of tile You want to check.</param>
        /// <param name="type">Marker type You want to check.</param>
        /// <returns>True if tile with provided coordinates has the same marker type as provided.</returns>
        public bool CheckMarkerType(Vector2Int coords, Enums.MarkerType type) =>
            TryGetMarkerType(coords, out Enums.MarkerType currentNodeMarkerType) && currentNodeMarkerType == type;

        public void RefreshMarkers()
        {
            foreach (Tile tile in tileInstances)
            {
                tile.MarkerType = Enums.MarkerType.None;
            }
        }
    
        public void RefreshTiles()
        {
            foreach (Tile tile in tileInstances)
            {
                tile.UpdateTileView(Enums.TileViewUpdateParam.Material);
            }
        }
    
        private void CreateGameBoard()
        {
            CreateTilesParent();
            CreateTiles();
            CreateInteraction();
        }

        private void CreateTilesParent()
        {
            tilesParent = new GameObject("Mesh").transform;
            tilesParent.parent = transform;
        }

        private void CreateTiles()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;
            float boardLength = gameSettings.BoardLength;
            float boardWidth = gameSettings.BoardWidth;
        
            for (int i = 0; i < boardLength; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                {
                    Vector3 position = tilesParent.position + new Vector3
                    (
                        j * gameSettings.TileLength,
                        0.5f * gameSettings.TileHeight,
                        i * gameSettings.TileLength
                    );
                    Tile instance = AllManagers.Instance.UtilsSpawner.CreateObject<Tile>(Enums.SpawnedUtils.Tile, tilesParent, position);
                    instance.Initialize(new Vector2Int(j, i), gameSettings.TileLength, gameSettings.TileHeight);
                    tileInstances.Add(instance);
                }
            }
        }

        private void CreateInteraction()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            Vector3 interactionSize = new Vector3
            (
                gameSettings.BoardWidth * gameSettings.TileLength,
                gameSettings.TileHeight,
                gameSettings.BoardLength * gameSettings.TileLength
            );
            cursorInteraction = new Interaction
            (
                transform,
                interactionSize
            );
        }
    
        private void InitializeInteractions()
        {
            cursorInteraction.SetAction(Enums.InteractionType.Hover, Enums.InteractionState.Tick, HandleHoverTick);
            cursorInteraction.SetAction(Enums.InteractionType.Hover, Enums.InteractionState.ExitInteraction, HandleHoverExitInteraction);
            
            cursorInteraction.SetAction(Enums.InteractionType.Click, Enums.InteractionState.EnterType, HandleClickEnterType);
            cursorInteraction.SetAction(Enums.InteractionType.Click, Enums.InteractionState.Tick, HandleClickTick);
            cursorInteraction.SetAction(Enums.InteractionType.Click, Enums.InteractionState.ExitInteraction, HandleClickExitInteraction);
        }

        private void HandleHoverTick(Interaction.InteractionDataArgs interactionDataArgs)
        {
            Vector2Int hitCoords = CalculateCoords(interactionDataArgs.HitInfo.point);
            OnHoverTick?.Invoke(hitCoords);
        }

        private void HandleHoverExitInteraction(Interaction.InteractionDataArgs interactionDataArgs) => OnHoverExitInteraction?.Invoke();
        private void HandleClickEnterType(Interaction.InteractionDataArgs interactionDataArgs) => OnClickEnterType?.Invoke();

        private void HandleClickTick(Interaction.InteractionDataArgs interactionDataArgs)
        {
            Vector2Int hitCoords = CalculateCoords(interactionDataArgs.HitInfo.point);
            OnClickTick?.Invoke(hitCoords);
        }
    
        private void HandleClickExitInteraction(Interaction.InteractionDataArgs interactionDataArgs) => OnClickExitInteraction?.Invoke();

        private Tile GetTile(Vector2Int coords)
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            if (coords.x < 0 || coords.x >= gameSettings.BoardWidth) return null;
            if (coords.y < 0 || coords.y >= gameSettings.BoardLength) return null;

            int index = coords.y * gameSettings.BoardWidth + coords.x;
            return tileInstances[index];
        }
    
        private bool TryGetTile(Vector2Int coords, out Tile result)
        {
            result = GetTile(coords);
            return result;
        }
    
        // Todo: These two methods below can be merged. They have difference only at 3rd line "result = tile.TileType/MarkerType;".
        private bool TryGetTileType(Vector2Int coords, out Enums.TileType result)
        {
            result = default;
            if (!TryGetTile(coords, out Tile tile)) return false;
            result = tile.TileType;
            return true;
        }
    
        private bool TryGetMarkerType(Vector2Int coords, out Enums.MarkerType result)
        {
            result = default;
            if (!TryGetTile(coords, out Tile tile)) return false;
            result = tile.MarkerType;
            return true;
        }
    
        private Vector2Int CalculateCoords(Vector3 hitPoint)
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            Vector3 localHitPoint = tilesParent.InverseTransformPoint(hitPoint);
            Vector2Int coordinates = new Vector2Int
            (
                Mathf.Clamp(Mathf.FloorToInt(localHitPoint.x / gameSettings.TileLength), 0, gameSettings.BoardWidth - 1),
                Mathf.Clamp(Mathf.FloorToInt(localHitPoint.z / gameSettings.TileLength), 0, gameSettings.BoardLength - 1)
            );
            return coordinates;
        }
    }
}