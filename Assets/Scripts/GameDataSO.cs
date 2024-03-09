using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data/Create new", order = 0)]
public class GameDataSO : ScriptableObject
{
    [Header("Tile")]
    [SerializeField] private Vector2Int size;
    [SerializeField] private float tileLength = 1;
    [SerializeField] private float tileHeight = 0.5f;
    [SerializeField] private float tileHighlightValue = 0.2f;
    [SerializeField] private TileColors tileColors;
    [SerializeField] private MarkerColors markerColors;
    [SerializeField] private float markerColorAlpha = 0.6f;
    
    [Header("Miscellaneous")]
    [SerializeField] private WaitingTimeData waitingTimeData;
    [SerializeField] private Enums.PermittedDirection[] permittedDirections;
    private readonly Dictionary<Enums.PermittedDirection, Vector2Int> permittedDirectionsLookup = new Dictionary<Enums.PermittedDirection, Vector2Int>
    {
        {Enums.PermittedDirection.Up, Vector2Int.up}, 
        {Enums.PermittedDirection.Down, Vector2Int.down}, 
        {Enums.PermittedDirection.Left, Vector2Int.left}, 
        {Enums.PermittedDirection.Right, Vector2Int.right}, 
        {Enums.PermittedDirection.UpRight, new Vector2Int(1, 1)}, 
        {Enums.PermittedDirection.DownRight, new Vector2Int(1, -1)}, 
        {Enums.PermittedDirection.DownLeft, new Vector2Int(-1, -1)}, 
        {Enums.PermittedDirection.UpLeft, new Vector2Int(-1, 1)}
    };

    public Vector2Int Size => size;
    public float TileLength => tileLength;
    public float TileHeight => tileHeight;
    public float TileHighlightValue => tileHighlightValue;
    public float MarkerColorAlpha => markerColorAlpha;
    
    public Color GetPermanentColor(Enums.TileType tileType)
    {
        return tileType switch
        {
            Enums.TileType.Default => tileColors.Default,
            Enums.TileType.Blocked => tileColors.Blocked,
            Enums.TileType.Start => tileColors.Start,
            Enums.TileType.Destination => tileColors.Destination,
            _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null)
        };
    }
    
    public Color GetMarkerColor(Enums.MarkerType markerType)
    {
        return markerType switch
        {
            Enums.MarkerType.None => markerColors.None,
            Enums.MarkerType.ReadyToCheck => markerColors.ReadyToCheck,
            Enums.MarkerType.Checked => markerColors.Checked,
            Enums.MarkerType.Path => markerColors.Path,
            _ => throw new ArgumentOutOfRangeException(nameof(markerType), markerType, null)
        };
    }
    
    public float GetWaitingTimeData(Enums.WaitingTime waitingTime)
    {
        return waitingTime switch
        {
            Enums.WaitingTime.AfterNewNodeEnqueuing => waitingTimeData.AfterNewNodeEnqueuing,
            Enums.WaitingTime.AfterNodeChecking => waitingTimeData.AfterNodeChecking,
            Enums.WaitingTime.AfterCursorPositionChange => waitingTimeData.AfterCursorPositionChange,
            Enums.WaitingTime.AfterPathNodeSetting => waitingTimeData.AfterPathNodeSetting,
            _ => throw new ArgumentOutOfRangeException(nameof(waitingTime), waitingTime, null)
        };
    }
    
    public Vector2Int[] GetPermittedDirections()
    {
        int length = permittedDirections.Length;
        var result = new Vector2Int[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = permittedDirectionsLookup[permittedDirections[i]];
        }
    
        return result;
    }
    
    private void OnValidate()
    {
        waitingTimeData.ManageForcingGeneralValue();
    }

    // Todo: Change all these structs and made serialized dictionaries from them.
    [Serializable]
    private struct TileColors
    {
        public Color Default;
        public Color Start;
        public Color Destination;
        public Color Blocked;
    }

    [Serializable]
    private struct MarkerColors
    {
        public Color None;
        public Color ReadyToCheck;
        public Color Checked;
        public Color Path;
    }
}

[Serializable]
public struct WaitingTimeData
{
    [field: Header("General value")]
    [SerializeField] private bool forceGeneralValue;
    [SerializeField] private float value;
    
    [field: Header("Values")]
    [field: SerializeField] public float AfterNewNodeEnqueuing { get; private set; }
    [field: SerializeField] public float AfterNodeChecking { get; private set; }
    [field: SerializeField] public float AfterCursorPositionChange { get; private set; }
    [field: SerializeField] public float AfterPathNodeSetting { get; private set; }

    // Todo: Change it to foreach loop.
    public void ManageForcingGeneralValue()
    {
        if (!forceGeneralValue) return;
        AfterNewNodeEnqueuing = value;
        AfterNodeChecking = value;
        AfterCursorPositionChange = value;
        AfterPathNodeSetting = value;
    }
}

[Serializable]
public struct PermittedDirection
{
    [field: SerializeField] private Enums.PermittedDirection permittedDirection;
    [field: SerializeField] public Vector2Int Direction { get; private set; }
}