using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : MonoBehaviour
{
    [SerializeField] private Vector2Int startingCoords = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int destinationCoords = new Vector2Int(1, 1);
    [SerializeField] private float waitTimeBetweenIterations = 0.5f;

    /*private readonly List<Vector2Int> _directions = new List<Vector2Int>
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

    private Vector2Int _currentPositionCoordinates;
    private GridManager _gridManager;
    private readonly Queue<Node> _nodesToCheck = new Queue<Node>();

    private void Awake()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    private void Start()
    {
        _currentPositionCoordinates = startingCoords;

        SetUpStartingAndDestinationTilesColour();
    }

    private void SetUpStartingAndDestinationTilesColour()
    {
        var startingNode = _gridManager.GetNodeFromCoordinates(startingCoords);
        startingNode.ChangeTileType(TileType.Start);
        var destinationNode = _gridManager.GetNodeFromCoordinates(destinationCoords);
        destinationNode.ChangeTileType(TileType.Destination);
    }

    private void OnStartBFS()
    {
        StartCoroutine(StartBFS());
    }

    private IEnumerator StartBFS()
    {
        while (!_currentPositionCoordinates.Equals(destinationCoords))
        {
            EnqueueNeighbors();
            yield return new WaitForSecondsRealtime(waitTimeBetweenIterations);

            CheckNode();
        }
        
        yield return DrawPath();
        Debug.Log("Done!");
    }

    private void EnqueueNeighbors()
    {
        foreach (var direction in _directions)
        {
            Vector2Int neighborCoords = _currentPositionCoordinates + direction;
            var neighborNode = _gridManager.GetNodeFromCoordinates(neighborCoords);

            if (neighborNode != null &&
                !_nodesToCheck.Contains(neighborNode) &&
                (neighborNode.isTypeOf(TileType.Default) || neighborNode.isTypeOf(TileType.Destination)))
            {
                _nodesToCheck.Enqueue(neighborNode);
                var currentNode = _gridManager.GetNodeFromCoordinates(_currentPositionCoordinates);
                neighborNode.PreviousNode = currentNode;
                neighborNode.ChangeTileType(TileType.ReadyToCheck);
            }
        }
    }

    private void CheckNode()
    {
        var currentNode = _nodesToCheck.Dequeue();
        _currentPositionCoordinates = currentNode.Coordinates;
        currentNode.ChangeTileType(TileType.Checked);
    }

    private IEnumerator DrawPath()
    {
        var node = _gridManager.GetNodeFromCoordinates(_currentPositionCoordinates);
        node.ChangeTileType(TileType.Destination);

        var path = new Stack<Node>();

        while (true)
        {
            var previousNode = node.PreviousNode;
            if (previousNode.isTypeOf(TileType.Start))
            {
                break;
            }
            path.Push(previousNode);
            node = previousNode;
        }

        while (path.Count > 0)
        {
            path.Pop().ChangeTileType(TileType.Path);
            yield return new WaitForSecondsRealtime(waitTimeBetweenIterations);
        }
    }*/
}
