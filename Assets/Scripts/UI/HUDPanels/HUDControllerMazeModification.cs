using TMPro;
using UnityEngine;

namespace UI.HUDPanels
{
    public class HUDControllerMazeModification : BaseHUDController<Enums.TileType>
    {
        [SerializeField] protected TextMeshProUGUI currentNodeLabel;

        public override void Show()
        {
            base.Show();
            UpdateCurrentNodeLabel(Enums.TileType.Default);
        }

        public void UpdateCurrentNodeLabel(Enums.TileType tileType)
        {
            Color color = AllManagers.Instance.GameManager.GameSettings.GetTileColor(tileType);
            currentNodeLabel.text = tileType.ToString();
            currentNodeLabel.color = color;
        }
    }
}