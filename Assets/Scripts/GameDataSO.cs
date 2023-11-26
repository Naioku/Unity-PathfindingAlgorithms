using System;
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