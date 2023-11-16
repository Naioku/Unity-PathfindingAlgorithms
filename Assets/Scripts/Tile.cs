using UnityEngine;

// [ExecuteAlways]
public class Tile : MonoBehaviour
{
    // [SerializeField] private Node node;
    [SerializeField] private bool isBlocked;
    private Vector2Int coordinates;
    private Enums.TileType tileType = Enums.TileType.Default;
    // private Node previousNode;

    private MeshRenderer meshRenderer;
    
    public void Initialize(Vector2Int initialCoords)
    {
        coordinates = initialCoords;
        name = $"({initialCoords.x}; {initialCoords.y})";
    }

    private void Awake()
    {
        
        // var gridManager = FindObjectOfType<GridManager>();
        // var coordinates = gridManager.GetCoordinatesFromPosition(transform.position);
        // node = new Node(coordinates, isBlocked? TileType.Blocked : TileType.Default);
        meshRenderer = transform.Find("Mesh").GetComponent<MeshRenderer>();
    }


    private void Update()
    {
        // _meshRenderer.material = node.GetMaterial();
        //
        // if (!Application.isPlaying)
        // {
        //     node.ChangeTileType(isBlocked ? TileType.Blocked : TileType.Default);
        // }
        
    }
    
    
    
    public Vector2Int Coordinates => coordinates;
    
    public Enums.TileType ChangeTileType(Enums.TileType type) => tileType = type;

    public bool isTypeOf(Enums.TileType type)
    {
        return type.Equals(tileType);
    }
}
