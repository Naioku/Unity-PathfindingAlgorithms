using System;
using System.Collections.Generic;
using InteractionSystem;
using Settings;
using UnityEngine;

public class Maze : MonoBehaviour, IInteractable
{
    private GameSettings gameSettings;
    private Transform tilesParent;
    private readonly List<Tile> tileInstances = new List<Tile>();
    private Interaction cursorInteraction;

    public event Action<Vector2Int> OnHoverTick;
    public event Action OnHoverExitInteraction;
        
    public event Action OnClickEnterType;
    public event Action<Vector2Int> OnClickTick;
    public event Action OnClickExitInteraction;
    
    private void Start()
    {
        gameSettings = AllManagers.Instance.GameManager.GameSettings;

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
    public bool CheckMarkerType(Vector2Int coords, Enums.MarkerType type)
    {
        return TryGetMarkerType(coords, out Enums.MarkerType currentNodeMarkerType) && currentNodeMarkerType == type;
    }
    
    public void Refresh()
    {
        foreach (Tile tile in tileInstances)
        {
            tile.MarkerType = Enums.MarkerType.None;
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
        for (int i = 0; i < gameSettings.Size.y; i++)
        {
            for (int j = 0; j < gameSettings.Size.x; j++)
            {
                Vector3 position = tilesParent.position + new Vector3
                (
                    j * gameSettings.TileDimensions.Length,
                    0.5f * gameSettings.TileDimensions.Height,
                    i * gameSettings.TileDimensions.Length
                );
                Tile instance = AllManagers.Instance.UtilsSpawner.CreateObject<Tile>(Enums.SpawnedUtils.Tile, tilesParent, position);
                instance.Initialize(new Vector2Int(j, i), gameSettings.TileDimensions);
                tileInstances.Add(instance);
            }
        }
    }

    private void CreateInteraction()
    {
        Vector3 interactionSize = new Vector3
        (
            gameSettings.Size.x * gameSettings.TileDimensions.Length,
            gameSettings.TileDimensions.Height,
            gameSettings.Size.y * gameSettings.TileDimensions.Length
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
        if (coords.x < 0 || coords.x >= gameSettings.Size.x) return null;
        if (coords.y < 0 || coords.y >= gameSettings.Size.y) return null;

        int index = coords.y * gameSettings.Size.x + coords.x;
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
        Vector3 localHitPoint = tilesParent.InverseTransformPoint(hitPoint);
        Vector2Int coordinates = new Vector2Int
        (
            Mathf.Clamp(Mathf.FloorToInt(localHitPoint.x / gameSettings.TileDimensions.Length), 0, gameSettings.Size.x - 1),
            Mathf.Clamp(Mathf.FloorToInt(localHitPoint.z / gameSettings.TileDimensions.Length), 0, gameSettings.Size.x - 1)
        );
        return coordinates;
    }
}