using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    public class GameSettings
    {
        private Vector2Int size;
        private TileDimensions tileDimensions;
        private TileColors tileColors;
        private MarkerColors markerColors;
        private AlgorithmStagesDelay algorithmStagesDelay;
        private Enums.PermittedDirection[] permittedDirections;
    
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

        public GameSettings(Vector2Int size, TileDimensions tileDimensions, TileColors tileColors, MarkerColors markerColors, AlgorithmStagesDelay algorithmStagesDelay, Enums.PermittedDirection[] permittedDirections)
        {
            this.size = size;
            this.tileDimensions = tileDimensions;
            this.tileColors = tileColors;
            this.markerColors = markerColors;
            this.algorithmStagesDelay = algorithmStagesDelay;
            this.permittedDirections = permittedDirections;
        }
    
        public Vector2Int Size => size;
        public TileDimensions TileDimensions => tileDimensions;
        public TileColors TileColors => tileColors;
        public MarkerColors MarkerColors => markerColors;
        public AlgorithmStagesDelay AlgorithmStagesDelay => algorithmStagesDelay;

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

        private void SaveSettings()
        {
        
        }
    }
    
    public struct TileDimensions
    {
        public float Length;
        public float Height;
    }

    public class TileColors : Setting<Enums.TileType, Color>
    {
        public float HighlightValue;
    }

    public class MarkerColors : Setting<Enums.MarkerType, Color>
    {
        public float Alpha;
    }

    public class AlgorithmStagesDelay : Setting<Enums.AlgorithmStageDelay, float> {}
}