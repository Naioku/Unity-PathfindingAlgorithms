using System;
using System.Collections.Generic;
using DefaultNamespace;
using InteractionSystem;
using UnityEngine;

public class Maze : MonoBehaviour, IInteractable
{
    private GameDataSO gameDataSO;
    private Transform tilesParent;
    private readonly List<Tile> tileInstances = new List<Tile>();
    private Interaction cursorInteraction;
    // private Enums.TileType currentTileTypeToSet;

    public event Action<Vector2Int> OnHoverEnterInteraction;
    public event Action<Vector2Int> OnHoverTick;
    public event Action OnHoverExitInteraction;
        
    public event Action OnClickEnterType;
    public event Action<Vector2Int> OnClickTick;
    public event Action OnClickExitInteraction;
    
    private void Start()
    {
        gameDataSO = AllManagers.Instance.GameManager.GameDataSO;

        CreateGameBoard();
        InitializeInteractions();
    }

    public void Interact(Interaction.InteractionDataSystem interactionDataSystem, Interaction.InteractionDataArgs interactionDataArgs)
    {
        cursorInteraction.Interact(interactionDataSystem, interactionDataArgs);
    }

    public void SelectTile(Vector2Int coords)
    {
        GetTile(coords).Select();
    }
    
    public void DeselectTile(Vector2Int coords)
    {
        GetTile(coords).Deselect();
    }

    public void SetTileType(Vector2Int coords, Enums.TileType type)
    {
        GetTile(coords).TileType = type;
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
        for (int i = 0; i < gameDataSO.Size.y; i++)
        {
            for (int j = 0; j < gameDataSO.Size.x; j++)
            {
                Vector3 position = tilesParent.position + new Vector3(j, 0.5f * gameDataSO.TileHeight, i);
                Tile instance = AllManagers.Instance.UtilsSpawner.CreateObject<Tile>(Enums.Utils.Tile, tilesParent, position);
                instance.Initialize(new Vector2Int(j, i));
                tileInstances.Add(instance);
            }
        }
    }

    private void CreateInteraction()
    {
        Vector3 interactionSize = new Vector3(gameDataSO.Size.x, gameDataSO.TileHeight, gameDataSO.Size.y);
        cursorInteraction = new Interaction
        (
            transform,
            interactionSize,
            gameDataSO.TileLength
        );
    }
    
    private void InitializeInteractions()
    {
        cursorInteraction.SetAction(Enums.InteractionType.Hover, Enums.InteractionState.EnterInteraction, HandleHoverEnterInteraction);
        cursorInteraction.SetAction(Enums.InteractionType.Hover, Enums.InteractionState.Tick, HandleHoverTick);
        cursorInteraction.SetAction(Enums.InteractionType.Hover, Enums.InteractionState.ExitInteraction, HandleHoverExitInteraction);
            
        cursorInteraction.SetAction(Enums.InteractionType.Click, Enums.InteractionState.EnterType, HandleClickEnterType);
        cursorInteraction.SetAction(Enums.InteractionType.Click, Enums.InteractionState.Tick, HandleClickTick);
        cursorInteraction.SetAction(Enums.InteractionType.Click, Enums.InteractionState.ExitInteraction, HandleClickExitInteraction);
    }

    private void HandleHoverEnterInteraction(Interaction.InteractionDataArgs interactionDataArgs)
    {
        // OnHoverEnterInteraction?.Invoke();
        Debug.Log("HandleHoverEnterInteraction");
    }
        
    private void HandleHoverTick(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Vector2Int hitCoords = CalculateCoords(interactionDataArgs.HitInfo.point);
        
        Debug.Log("HandleHoverTick");
        Debug.Log($"HitPoint: {interactionDataArgs.HitInfo.point}");
        Debug.Log($"HitCoords: {hitCoords}");
        Debug.Log($"TileName: {GetTile(hitCoords).name}");
        
        OnHoverTick?.Invoke(hitCoords);
    }

    private void HandleHoverExitInteraction(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Debug.Log("HandleHoverExitInteraction");
        OnHoverExitInteraction?.Invoke();
    }
    
    private void HandleClickEnterType(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Debug.Log("HandleClickEnterType");
        OnClickEnterType?.Invoke();
    }
    
    private void HandleClickTick(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Vector2Int hitCoords = CalculateCoords(interactionDataArgs.HitInfo.point);
        
        Debug.Log("HandleClickTick");
        Debug.Log($"HitPoint: {interactionDataArgs.HitInfo.point}");
        Debug.Log($"HitCoords: {hitCoords}");
        Debug.Log($"TileName: {GetTile(hitCoords).name}");
        
        OnClickTick?.Invoke(hitCoords);
    }
    
    private void HandleClickExitInteraction(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Debug.Log("HandleClickExitInteraction");
        OnClickExitInteraction?.Invoke();
    }
    
    private Tile GetTile(Vector2Int coords)
    {
        int index = coords.y * gameDataSO.Size.x + coords.x;
        return tileInstances[index];
    }
    
    private Vector2Int CalculateCoords(Vector3 hitPoint)
    {
        Vector3 localHitPoint = tilesParent.InverseTransformPoint(hitPoint);
        Vector2Int coordinates = new Vector2Int
        (
            Mathf.Clamp(Mathf.FloorToInt(localHitPoint.x / gameDataSO.TileLength), 0, gameDataSO.Size.x - 1),
            Mathf.Clamp(Mathf.FloorToInt(localHitPoint.z / gameDataSO.TileLength), 0, gameDataSO.Size.x - 1)
        );
        return coordinates;
    }
}