using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpdateSystem.CoroutineSystem;
using WaitForSeconds = UpdateSystem.CoroutineSystem.WaitForSeconds;
using WaitUntil = UpdateSystem.CoroutineSystem.WaitUntil;

namespace StageMachineSystem.Algorithm
{
    public abstract class Algorithm
    {
        #region StaticValues

        protected Vector2Int startCoords;
        protected Vector2Int destinationCoords;
        private readonly CoroutineManager.CoroutineCaller coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        private readonly Vector2Int[] directions = AllManagers.Instance.GameManager.GameSettings.GetPermittedDirections();
        private readonly GameManager gameManager = AllManagers.Instance.GameManager;
        private Maze.Maze maze;
        private Action onFinish;

        #endregion

        #region VariableValues
        
        private Vector2Int? cursorPosition;
        private Guid processCoroutineId;
        private bool stopAtNextWaitCheckpoint;
        private bool performingStep;

        #endregion
        
        protected abstract NodeBase StartNode { get; }
        protected abstract NodeBase CurrentNode { get; set; }
        
        protected Vector2Int? CursorPosition
        {
            set
            {
                if (cursorPosition.HasValue) maze.DeselectTile(cursorPosition.Value);
                cursorPosition = value;
                if (cursorPosition.HasValue) maze.SelectTile(cursorPosition.Value);
            }

            get
            {
                if (!cursorPosition.HasValue)
                {
                    throw new NullReferenceException("Cursor position have to be set before getting access.");
                }
                return cursorPosition.Value;
            }
        }

        private IWait GetWaitObject(Enums.AlgorithmStageDelay algorithmStageDelay)
        {
            if (performingStep)
            {
                performingStep = false;
                Pause();
            }
            
            if (stopAtNextWaitCheckpoint)
            {
                return new WaitUntil(() => !stopAtNextWaitCheckpoint);
            }
            else
            {
                return new WaitForSeconds(gameManager.GameSettings.GetDelay(algorithmStageDelay));
            }
        }

        public void Initialize(Maze.Maze maze, Action onFinish)
        {
            this.maze = maze;
            startCoords = maze.UniqueTilesCoordsLookup[Enums.TileType.Start].Value;
            destinationCoords = maze.UniqueTilesCoordsLookup[Enums.TileType.Destination].Value;
            this.onFinish = onFinish;
            Refresh();
        }

        public void Play()
        {
            stopAtNextWaitCheckpoint = false;
            processCoroutineId = coroutineCaller.StartCoroutine(ProcessCoroutine());
            CurrentNode = StartNode;
            CursorPosition = startCoords;
        }

        public void Step(bool algorithmStarted)
        {
            if (algorithmStarted)
            {
                Resume();
            }
            else
            {
                Play();
            }

            performingStep = true;
        }

        public void Resume()
        {
            stopAtNextWaitCheckpoint = false;
        }

        public void Pause()
        {
            stopAtNextWaitCheckpoint = true;
        }

        public virtual void Refresh()
        {
            CurrentNode = null;
            CursorPosition = null;
            maze.RefreshMarkers();
        }

        public void Stop()
        {
            coroutineCaller.StopCoroutine(ref processCoroutineId);
        }

        private IEnumerator ProcessCoroutine()
        {
            while (!CloseNode())
            {
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterNodeChecking);
                yield return OpenNeighborNodes();
                
                if (!ReloadNode())
                {
                    Debug.Log("Path cannot be found!");
                    Stop();
                }
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterCursorPositionChange);
            }
         
            yield return DrawPath();
            Debug.Log("Done!");
            Stop();
            onFinish?.Invoke();
        }
        
        /// <summary>
        /// Checking current node.
        /// </summary>
        /// <returns>True if current node is the destination node.</returns>
        private bool CloseNode()
        {
            maze.SetMarkerType(CurrentNode.Coords, Enums.MarkerType.Closed);
            return CurrentNode.Coords == destinationCoords;
        }
        
        private IEnumerator OpenNeighborNodes()
        {
            foreach (var direction in directions)
            {
                CursorPosition = CurrentNode.Coords + direction;
                Vector2Int position = CursorPosition.Value;
                
                if (!maze.CheckTileType(position, Enums.TileType.Default, Enums.TileType.Destination)) continue;
                if (!maze.CheckMarkerType(position, Enums.MarkerType.None)) continue;
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterCursorPositionChange);

                OpenNode(position);
                maze.SetMarkerType(position, Enums.MarkerType.Opened);
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterNewNodeEnqueuing);
            }
        }

        protected abstract void OpenNode(Vector2Int coords);

        /// <summary>
        /// Reloads current node.
        /// </summary>
        /// <returns>False if there is no node left to check.</returns>
        protected abstract bool ReloadNode();

        private IEnumerator DrawPath()
        {
            var path = new Stack<NodeBase>();
            while (!maze.CheckTileType(CurrentNode.Coords, Enums.TileType.Start))
            {
                path.Push(CurrentNode);
                CurrentNode = CurrentNode.PreviousNode;
            }
            
            path.Push(CurrentNode);
        
            while (path.Count > 0)
            {
                maze.SetMarkerType(path.Pop().Coords, Enums.MarkerType.Path);
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterPathNodeSetting);
            }
        }
        
        protected abstract class NodeBase
        {
            public Vector2Int Coords { get; }
            public abstract NodeBase PreviousNode { get; }

            protected NodeBase(Vector2Int coords)
            {
                Coords = coords;
            }
        }
    }
}