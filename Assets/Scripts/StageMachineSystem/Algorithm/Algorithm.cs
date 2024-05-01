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

        private readonly CoroutineManager.CoroutineCaller coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        protected readonly Vector2Int[] directions = AllManagers.Instance.GameManager.GameSettings.GetPermittedDirections();
        private readonly GameManager gameManager = AllManagers.Instance.GameManager;
        protected Maze.Maze maze;
        private Vector2Int startCoords;
        private Vector2Int destinationCoords;

        #endregion

        #region VariableValues

        protected readonly Queue<Node> nodesToCheck = new();
        protected Node currentNode;
        private Vector2Int? cursorPosition;
        private Guid processCoroutineId;
        private bool stopAtNextWaitCheckpoint;
        private bool performingStep;

        #endregion
        
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

        protected Action onFinish;
        
        protected IWait GetWaitObject(Enums.AlgorithmStageDelay algorithmStageDelay)
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
            currentNode = new Node(startCoords, null);
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

        public void Refresh()
        {
            currentNode = null;
            CursorPosition = null;
            maze.RefreshMarkers();
            nodesToCheck.Clear();
        }

        public void Stop()
        {
            coroutineCaller.StopCoroutine(ref processCoroutineId);
        }

        protected abstract IEnumerator ProcessCoroutine();
        
        /// <summary>
        /// Checking current node.
        /// </summary>
        /// <returns>True if current node is the destination node.</returns>
        protected bool CheckNode()
        {
            maze.SetMarkerType(currentNode.Coords, Enums.MarkerType.Checked);
            return currentNode.Coords == destinationCoords;
        }

        /// <summary>
        /// Reloads current node.
        /// </summary>
        /// <returns>False if there is no node left to check.</returns>
        protected bool ReloadNode()
        {
            if (nodesToCheck.Count == 0) return false;
            
            currentNode = nodesToCheck.Dequeue();
            CursorPosition = currentNode.Coords;
            return true;
        }
        
        protected IEnumerator DrawPath()
        {
            var path = new Stack<Node>();
            while (!maze.CheckTileType(currentNode.Coords, Enums.TileType.Start))
            {
                path.Push(currentNode);
                currentNode = currentNode.PreviousNode;
            }
            
            path.Push(currentNode);
        
            while (path.Count > 0)
            {
                maze.SetMarkerType(path.Pop().Coords, Enums.MarkerType.Path);
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterPathNodeSetting);
            }
        }
        
        protected class Node
        {
            public Vector2Int Coords { get; private set; }
            public Node PreviousNode { get; private set; }

            public Node(Vector2Int coords, Node previousNode)
            {
                Coords = coords;
                PreviousNode = previousNode;
            }
        }
    }
}