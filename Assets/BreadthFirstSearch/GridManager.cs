using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public readonly Vector3 Grid = new Vector3(10f, 0.5f, 10f);
    
    [SerializeField] private GameObject gridGameObj;
    private readonly Dictionary<Vector2Int, Node> _tileGrid = new Dictionary<Vector2Int, Node>();

    private void Awake()
    {
        foreach (Transform child in gridGameObj.transform)
        {
            var node = child.GetComponent<Tile>().Node;
            _tileGrid.Add(node.Coordinates, node);
        }
    }

    public Node GetNodeFromCoordinates(Vector2Int coordinates)
    {
        return _tileGrid.ContainsKey(coordinates) ? _tileGrid[coordinates] : null;
    }

    public Vector2Int GetCoordinatesFromPosition(Vector3 position)
    {
        return new Vector2Int((int) (position.x / Grid.x), (int) (position.z / Grid.z));
    }
}
