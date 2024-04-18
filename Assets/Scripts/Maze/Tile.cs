using UnityEngine;

namespace Maze
{
    public class Tile : MonoBehaviour
    {
        private Enums.TileType tileType = Enums.TileType.Default;
        private Enums.MarkerType markerType = Enums.MarkerType.None;
        private bool highlighted;

        private GameManager gameManager;
        private MeshRenderer permanentMeshRenderer;
        private MeshRenderer markerMeshRenderer;
    
        public void Initialize(Vector2Int initialCoords, float tileLength, float tileHeight)
        {
            name = $"({initialCoords.x}; {initialCoords.y})";
            Vector3 scale = new Vector3
            (
                tileLength,
                tileHeight,
                tileLength
            );
            transform.localScale = scale;
            TileType = Enums.TileType.Default;
        }

        private void Awake()
        {
            gameManager = AllManagers.Instance.GameManager;
            permanentMeshRenderer = transform.Find("TileMesh").GetComponent<MeshRenderer>();
            markerMeshRenderer = transform.Find("MarkerMesh").GetComponent<MeshRenderer>();
        }
    
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

        public void Select() => Highlighted = true;
        public void Deselect() => Highlighted = false;
    
        public void UpdateTileView(Enums.TileViewUpdateParam parameters)
        {
            if ((parameters & Enums.TileViewUpdateParam.Material) > 0) UpdateMaterial();
            if ((parameters & Enums.TileViewUpdateParam.Highlight) > 0) UpdateHighlight();
        
            return;
        
            void UpdateMaterial()
            {
                permanentMeshRenderer.material.color = gameManager.GameSettings.GetTileColor(tileType);
            }

            void UpdateHighlight()
            {
                Color color = permanentMeshRenderer.material.color;
                float highlightValue = gameManager.GameSettings.TileHighlight;

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
            Color color = gameManager.GameSettings.GetMarkerColor(markerType);
            color.a = gameManager.GameSettings.MarkerAlpha;
            markerMeshRenderer.material.color = color;
            markerMeshRenderer.gameObject.SetActive(markerType != Enums.MarkerType.None);
        }
    }
}
