using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UpdateSystem;
using UpdateSystem.CoroutineSystem;

namespace BreadthFirstSearch.Scripts
{
    public class BFS : IAlgorithm
    {
        #region StaticValues

        private readonly CoroutineManager.CoroutineCaller coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        private readonly List<Vector2Int> directions = new List<Vector2Int>
        { 
            Vector2Int.up, 
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1)
        };
        private Maze maze;
        private Vector2Int startCoords;
        private Vector2Int destinationCoords;
        // private float waitTimeBetweenIterations = 0.5f;

        #endregion

        #region VariableValues

        private readonly Queue<Node> nodesToCheck = new Queue<Node>();
        private Node currentNode;
        private Guid processCoroutineId;

        #endregion

        public void Initialize(Maze maze, Vector2Int startCoords, Vector2Int destinationCoords)
        {
            this.maze = maze;
            this.startCoords = startCoords;
            this.destinationCoords = destinationCoords;
            Refresh();
        }
        
        public void Play()
        {
            processCoroutineId = coroutineCaller.StartCoroutine(ProcessCoroutine());
        }

        public void Pause()
        {
            
        }

        public void Step()
        {
            
        }

        public void Refresh()
        {
            currentNode = new Node(startCoords, null);
            maze.Refresh();
            nodesToCheck.Clear();
        }

        public void Stop()
        {
            Refresh();
        }
        
        private IEnumerator<IWait> ProcessCoroutine()
        {
            while (!CheckNode())
            {
                if (ContinueSearching()) yield return null;
                
                coroutineCaller.StopCoroutine(ref processCoroutineId);
                yield return null;
            }
         
            DrawPath();
            coroutineCaller.StopCoroutine(ref processCoroutineId);
            Debug.Log("Done!");
        }

        /// <summary>
        /// Enqueues next part of nodes and reloads current node.
        /// </summary>
        /// <returns><b>False</b> if there is no node left to check.</returns>
        private bool ContinueSearching()
        {
            EnqueueNeighbors();
            // yield return new WaitForSecondsRealtime(waitTimeBetweenIterations);

            if (ReloadNode()) return true;
            
            Debug.Log("Path cannot be found!");
            return false;

        }

        private void EnqueueNeighbors()
        {
            foreach (var direction in directions)
            {
                Vector2Int neighborCoords = currentNode.Coords + direction;
                
                if (!maze.CheckTileType(neighborCoords, Enums.TileType.Default) && 
                    !maze.CheckTileType(neighborCoords, Enums.TileType.Destination)) continue;
                if (!maze.CheckMarkerType(neighborCoords, Enums.MarkerType.None)) continue;
                
                maze.SetMarkerType(neighborCoords, Enums.MarkerType.ReadyToCheck);
                nodesToCheck.Enqueue(new Node(neighborCoords, currentNode));
            }
        }

        /// <summary>
        /// Checking current node.
        /// </summary>
        /// <returns><b>True</b> if current node is the destination node.</returns>
        private bool CheckNode()
        {
            maze.SetMarkerType(currentNode.Coords, Enums.MarkerType.Checked);
            return currentNode.Coords == destinationCoords;
        }

        /// <summary>
        /// Reloads current node.
        /// </summary>
        /// <returns><b>False</b> if there is no node left to check.</returns>
        private bool ReloadNode()
        {
            if (nodesToCheck.Count == 0) return false;
            
            currentNode = nodesToCheck.Dequeue();
            return true;
        }
        
        private void DrawPath()
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
                // yield return new WaitForSecondsRealtime(waitTimeBetweenIterations);
            }
        }

        private class Node
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