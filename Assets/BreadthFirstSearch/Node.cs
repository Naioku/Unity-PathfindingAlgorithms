using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Node
{
    [SerializeField] private Vector2Int coordinates;
    [SerializeField] private TileType tileType;
    [SerializeField] private Node previousNode;
    
    public Vector2Int Coordinates => coordinates;
    public Node PreviousNode 
    {
        get => previousNode;
        set => previousNode = value;
    }
    public Node(Vector2Int coordinates, TileType tileType)
    {
        this.coordinates = coordinates;
        this.tileType = tileType;
    }

    public Material GetMaterial()
    {
        var data = Object.FindObjectOfType<Data>();

        return tileType switch
        {
            TileType.Blocked => data.tileBlockedMaterial,
            TileType.ReadyToCheck => data.tileReadyToCheckMaterial,
            TileType.Checked => data.tileCheckedMaterial,
            TileType.Path => data.tilePathMaterial,
            TileType.Default => data.tileDefaultMaterial,
            TileType.Start => data.tileStartMaterial,
            TileType.Destination => data.tileDestinationMaterial,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public TileType ChangeTileType (TileType type) => tileType = type;

    public bool isTypeOf(TileType type)
    {
        return type.Equals(tileType);
    }
}

public enum TileType
{
    Blocked,
    ReadyToCheck,
    Checked,
    Path,
    Default,
    Start,
    Destination
}
