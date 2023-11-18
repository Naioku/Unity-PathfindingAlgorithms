using System.Collections.Generic;
using UnityEngine;

namespace StageMachineSystem
{
    public abstract class BaseStage
    {
        protected Maze maze;
        protected Dictionary<Enums.TileType, Vector2Int?> uniqueTilesCoordsLookup = new Dictionary<Enums.TileType, Vector2Int?>
        {
            { Enums.TileType.Start, null },
            { Enums.TileType.Destination, null }
        };

        protected BaseStage(Maze maze)
        {
            this.maze = maze;
        }

        public virtual void Enter() {}
        public virtual void Tick() {}
        public virtual void Exit() {}
    }
}