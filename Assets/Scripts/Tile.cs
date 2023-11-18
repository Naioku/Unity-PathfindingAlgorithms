using DefaultNamespace;
using UnityEngine;

// [ExecuteAlways]
public class Tile : MonoBehaviour
{
    // [SerializeField] private Node node;
    [SerializeField] private bool isBlocked;
    private Vector2Int coordinates;
    private Enums.TileType tileType = Enums.TileType.Default;
    private bool highlight;
    // private Node previousNode;

    private GameDataSO gameDataSO;
    private MeshRenderer meshRenderer;
    
    public Enums.TileType TileType
    {
        set
        {
            tileType = value;
            UpdateView(Enums.TileViewUpdateParam.Material | Enums.TileViewUpdateParam.Highlight);
        }
    }

    private bool Highlight
    {
        set
        {
            highlight = value;
            UpdateView(Enums.TileViewUpdateParam.Highlight);
        }
    }
    
    public void Initialize(Vector2Int initialCoords)
    {
        coordinates = initialCoords;
        name = $"({initialCoords.x}; {initialCoords.y})";
    }

    private void Awake()
    {
        gameDataSO = AllManagers.Instance.GameManager.GameDataSO;
        meshRenderer = transform.Find("Mesh").GetComponent<MeshRenderer>();
    }

    public void Select() => Highlight = true;
    public void Deselect() => Highlight = false;

    // public Vector2Int Coordinates => coordinates;
    //
    // public bool isTypeOf(Enums.TileType type)
    // {
    //     return type.Equals(tileType);
    // }
    
    private void UpdateView(Enums.TileViewUpdateParam parameters)
    {
        if ((parameters & Enums.TileViewUpdateParam.Material) > 0) UpdateMaterial();
        if ((parameters & Enums.TileViewUpdateParam.Highlight) > 0) UpdateHighlight();
        
        return;
        
        void UpdateMaterial()
        {
            meshRenderer.material = gameDataSO.GetTileMaterial(tileType);
        }

        void UpdateHighlight()
        {
            Color color = meshRenderer.material.color;
            float highlightValue = gameDataSO.TileHighlightValue;

            if (highlight)
            {
                color += new Color(highlightValue, highlightValue, highlightValue);
            }
            else
            {
                color -= new Color(highlightValue, highlightValue, highlightValue);
            }

            meshRenderer.material.color = color;
        }
    }
}
