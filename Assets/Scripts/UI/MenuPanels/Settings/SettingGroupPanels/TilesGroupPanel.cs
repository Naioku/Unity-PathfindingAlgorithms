using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels.Settings.SettingGroups;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingGroupPanels
{
    [Serializable]
    public class TilesGroupPanel : SettingGroupPanel<TilesGroupPanel.SettingGroupName>
    {
        [SerializeField] private BoardSizeGroup size;
        [SerializeField] private TileDimensionsGroup tileDimensions;
        [SerializeField] private TileColorsGroup tileColors;
        [SerializeField] private MarkerColorsGroup markerColors;

        public Vector2Int SizeSetting
        {
            get => size.Setting;
            set => size.Setting = value;
        }

        public TileDimensions TileDimensionsSetting
        {
            get => tileDimensions.Setting;
            set => tileDimensions.Setting = value;
        }

        public TileColors TileColorsSetting
        {
            get => tileColors.Setting;
            set => tileColors.Setting = value;
        }

        public MarkerColors MarkerColorsSetting
        {
            get => markerColors.Setting;
            set => markerColors.Setting = value;
        }

        public override void BuildLookup()
        {
            settingGroupsLookup ??= new Dictionary<SettingGroupName, SettingGroupInGame>()
            {
                { size.Name, size },
                { tileDimensions.Name, tileDimensions },
                { tileColors.Name, tileColors },
                { markerColors.Name, markerColors }
            };
            
            base.BuildLookup();
        }
        
        public enum SettingGroupName
        {
            Size,
            TileDimensions,
            TileColors,
            MarkerColors
        }
    }
}