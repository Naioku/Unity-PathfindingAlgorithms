using DefaultNamespace;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Enums.TileType tileType = Enums.TileType.Default;
    private Enums.MarkerType markerType = Enums.MarkerType.None;
    private bool highlighted;

    private GameDataSO gameDataSO;
    private MeshRenderer permanentMeshRenderer;
    private MeshRenderer markerMeshRenderer;
    
    public Enums.TileType TileType
    {
        get => tileType;
        set
        {
            tileType = value;
            
            Enums.TileViewUpdateParam material = Enums.TileViewUpdateParam.Material;
            Enums.TileViewUpdateParam materialAndHighlight = Enums.TileViewUpdateParam.Material | Enums.TileViewUpdateParam.Highlight;
            UpdateTileView(highlighted ? materialAndHighlight : material);
        }
    }
    
    public Enums.MarkerType MarkerType
    {
        get => markerType;
        set
        {
            markerType = value;
            UpdateMarkerView();
        }
    }

    private bool Highlighted
    {
        set
        {
            highlighted = value;
            UpdateTileView(Enums.TileViewUpdateParam.Highlight);
        }
    }
    
    public void Initialize(Vector2Int initialCoords)
    {
        name = $"({initialCoords.x}; {initialCoords.y})";
    }

    private void Awake()
    {
        gameDataSO = AllManagers.Instance.GameManager.GameDataSO;
        permanentMeshRenderer = transform.Find("TileMesh").GetComponent<MeshRenderer>();
        markerMeshRenderer = transform.Find("MarkerMesh").GetComponent<MeshRenderer>();
    }

    public void Select() => Highlighted = true;
    public void Deselect() => Highlighted = false;
    
    private void UpdateTileView(Enums.TileViewUpdateParam parameters)
    {
        if ((parameters & Enums.TileViewUpdateParam.Material) > 0) UpdateMaterial();
        if ((parameters & Enums.TileViewUpdateParam.Highlight) > 0) UpdateHighlight();
        
        return;
        
        void UpdateMaterial()
        {
            permanentMeshRenderer.material.color = gameDataSO.GetPermanentColor(tileType);
        }

        void UpdateHighlight()
        {
            Color color = permanentMeshRenderer.material.color;
            float highlightValue = gameDataSO.TileHighlightValue;

            if (highlighted)
            {
                color += new Color(highlightValue, highlightValue, highlightValue);
            }
            else
            {
                color -= new Color(highlightValue, highlightValue, highlightValue);
            }

            permanentMeshRenderer.material.color = color;
        }
    }
    
    private void UpdateMarkerView()
    {
        Color color = gameDataSO.GetMarkerColor(markerType);
        color.a = gameDataSO.MarkerColorAlpha;
        markerMeshRenderer.material.color = color;
        markerMeshRenderer.gameObject.SetActive(markerType != Enums.MarkerType.None);
    }
}
