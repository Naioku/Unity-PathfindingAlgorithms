using UnityEngine;

namespace UI.HUDPanels
{
    public class HUDControllerMazeModification : BaseHUDController<Enums.TileType>
    {
        protected override void Initialize()
        {
            header.Initialize(Enums.MainMenuPanelButtonTag.MazeModification);
            staticLabel.Initialize(Enums.GeneralText.HUDAlgorithmStateLabel);
            dynamicLabel.Initialize(Enums.TileType.Default);
        }

        public void UpdateCurrentNodeLabel(Enums.TileType tileType)
        {
            Color color = AllManagers.Instance.GameManager.GameSettings.GetTileColor(tileType);
            dynamicLabel.SetLocalizedTextKey(tileType);
            dynamicLabel.Color = color;
        }
    }
}