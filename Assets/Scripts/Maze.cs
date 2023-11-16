using System;
using System.Collections.Generic;
using DefaultNamespace;
using InteractionSystem;
using UnityEngine;

public class Maze : MonoBehaviour, IInteractable
{
    [SerializeField] private GameDataSO gameDataSO;

    private Transform tilesParent;
    private List<Tile> tileInstances = new List<Tile>();
    private Interaction cursorInteraction;
    
    public event Action<Vector2Int> OnHoverEnterInteraction;
    public event Action<Vector2Int> OnHoverTick;
    public event Action OnHoverExitInteraction;
        
    public event Action OnClickExit;
    
    public void Interact(Interaction.InteractionDataSystem interactionDataSystem, Interaction.InteractionDataArgs interactionDataArgs)
    {
        cursorInteraction.Interact(interactionDataSystem, interactionDataArgs);
    }

    private void Start()
    {
        CreateGameBoard();
        InitializeInteractions();
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
                instance.Initialize(new Vector2Int(i, j));
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
            
        cursorInteraction.SetAction(Enums.InteractionType.Click, Enums.InteractionState.ExitType, HandleClickExit);
    }

    private void HandleHoverEnterInteraction(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Debug.Log("HandleHoverEnterInteraction");
        // OnHoverEnterInteraction?.Invoke(CalculateCoords(interactionDataArgs.HitInfo.point));
    }
        
    private void HandleHoverTick(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Debug.Log("HandleHoverTick");
        // OnHoverTick?.Invoke(CalculateCoords(interactionDataArgs.HitInfo.point));
    }

    private void HandleHoverExitInteraction(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Debug.Log("HandleHoverExitInteraction");
        // OnHoverExitInteraction?.Invoke();
    }
        
    private void HandleClickExit(Interaction.InteractionDataArgs interactionDataArgs)
    {
        Debug.Log("HandleClickExit");
        // OnClickExit?.Invoke();
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