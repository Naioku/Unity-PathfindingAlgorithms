using UnityEngine;

[ExecuteAlways]
public class Tile : MonoBehaviour
{
    [SerializeField] private Node node;
    [SerializeField] private bool isBlocked;

    private MeshRenderer _meshRenderer;

    public Node Node => node;

    private void Awake()
    {
        var gridManager = FindObjectOfType<GridManager>();
        var coordinates = gridManager.GetCoordinatesFromPosition(transform.position);
        node = new Node(coordinates, isBlocked? TileType.Blocked : TileType.Default);
        _meshRenderer = GetComponent<MeshRenderer>();
    }


    private void Update()
    {
        _meshRenderer.material = node.GetMaterial();
        
        if (!Application.isPlaying)
        {
            node.ChangeTileType(isBlocked ? TileType.Blocked : TileType.Default);
        }
        
    }
}
