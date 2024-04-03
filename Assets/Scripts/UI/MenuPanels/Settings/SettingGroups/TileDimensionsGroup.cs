using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels.Settings.SettingEntries;
using UI.MenuPanels.Settings.SettingGroupPanels;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingGroups
{
    [Serializable]
    public class TileDimensionsGroup : SettingGroupInGame<TileDimensionsGroup.SettingNameTileDimensions, TilesGroupPanel.SettingGroupName>
    {
        private const string SettingGroupName = "Tile dimensions";

        private SettingEntry<float> length = new SettingEntry<float>(SettingGroupName, "Length");
        private SettingEntry<float> height = new SettingEntry<float>(SettingGroupName, "Height");
            
        public override TilesGroupPanel.SettingGroupName Name => TilesGroupPanel.SettingGroupName.TileDimensions;
        public TileDimensions Setting
        {
            get =>
                new TileDimensions
                {
                    Length = length.Value,
                    Height = height.Value
                };
            set
            {
                length.Value = value.Length;
                height.Value = value.Height;
            }
        }

        public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

        public override void BuildLookup()
        {
            settingsLookup = new Dictionary<SettingNameTileDimensions, ISettingEntry>
            {
                { SettingNameTileDimensions.Length, length },
                { SettingNameTileDimensions.Height, height }
            };
        }
            
        public enum SettingNameTileDimensions
        {
            Length,
            Height
        }
    }
}