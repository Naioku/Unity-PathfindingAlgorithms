using DefaultNamespace;
using UnityEngine;

// [ExecuteAlways]
public class Tile : MonoBehaviour
{
    // [SerializeField] private Node node;
    [SerializeField] private bool isBlocked;
    private Vector2Int coordinates;
    private Enums.TileType tileType = Enums.TileType.Default;
    private bool highlighted;
    // private Node previousNode;

    private GameDataSO gameDataSO;
    private MeshRenderer meshRenderer;
    
    public Enums.TileType TileType
    {
        set
        {
            tileType = value;
            
            Enums.TileViewUpdateParam material = Enums.TileViewUpdateParam.Material;
            Enums.TileViewUpdateParam materialAndHighlight = Enums.TileViewUpdateParam.Material | Enums.TileViewUpdateParam.Highlight;
            UpdateView(highlighted ? materialAndHighlight : material);
        }
    }

    private bool Highlighted
    {
        set
        {
            highlighted = value;
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

    public void Select() => Highlighted = true;
    public void Deselect() => Highlighted = false;

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

            if (highlighted)
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
