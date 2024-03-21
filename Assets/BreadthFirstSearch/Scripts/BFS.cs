using System.Collections;
using StageMachineSystem.Algorithm;
using UnityEngine;

namespace BreadthFirstSearch.Scripts
{
    public class BFS : Algorithm
    {
        protected override IEnumerator ProcessCoroutine()
        {
            while (!CheckNode())
            {
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterNodeChecking);
                yield return EnqueueNeighbors();

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

        private IEnumerator EnqueueNeighbors()
        {
            foreach (var direction in directions)
            {
                CursorPosition = currentNode.Coords + direction;
                Vector2Int position = CursorPosition.Value;

                if (!maze.CheckTileType(position, Enums.TileType.Default, Enums.TileType.Destination)) continue;
                if (!maze.CheckMarkerType(position, Enums.MarkerType.None)) continue;
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterCursorPositionChange);
                
                maze.SetMarkerType(position, Enums.MarkerType.ReadyToCheck);
                nodesToCheck.Enqueue(new Node(position, currentNode));
                yield return GetWaitObject(Enums.AlgorithmStageDelay.AfterNewNodeEnqueuing);
            }
        }
    }
}