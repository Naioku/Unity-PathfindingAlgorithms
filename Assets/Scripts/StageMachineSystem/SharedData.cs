using System.Collections.Generic;
using UnityEngine;

namespace StageMachineSystem
{
    public class SharedData
    {
        public Maze Maze { get; set; }
        
        public Dictionary<Enums.TileType, Vector2Int?> UniqueTilesCoordsLookup { get; set; } = new Dictionary<Enums.TileType, Vector2Int?>
        {
            { Enums.TileType.Start, null },
            { Enums.TileType.Destination, null }
        };
    }
}