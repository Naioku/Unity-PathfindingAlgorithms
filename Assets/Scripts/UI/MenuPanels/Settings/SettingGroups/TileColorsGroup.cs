using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels.Settings.SettingEntries;
using UI.MenuPanels.Settings.SettingGroupPanels;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingGroups
{
    [Serializable]
    public class TileColorsGroup : SettingGroupInGame<TileColorsGroup.SettingNameTileColors, TilesGroupPanel.SettingGroupName>
    {
        private const string SettingGroupName = "Tile colors";
        
        private Dictionary<Enums.TileType, SettingEntry<Color>> colors = new Dictionary<Enums.TileType, SettingEntry<Color>>
        {
            { Enums.TileType.Default, new SettingEntry<Color>(SettingGroupName, "Default") },
            { Enums.TileType.Start, new SettingEntry<Color>(SettingGroupName, "Start") },
            { Enums.TileType.Destination, new SettingEntry<Color>(SettingGroupName, "Destination") },
            { Enums.TileType.Blocked, new SettingEntry<Color>(SettingGroupName, "Blocked") }
        };
        private readonly SettingEntry<float> highlight = new SettingEntry<float>(SettingGroupName, "Cursor highlight");
        
        public override TilesGroupPanel.SettingGroupName Name => TilesGroupPanel.SettingGroupName.TileColors;
        public TileColors Setting
        {
            get
            {
                TileColors setting = new TileColors();
                foreach (var entry in colors)
                {
                    setting.SetValue(entry.Key, entry.Value.Value);
                }
                setting.HighlightValue = highlight.Value;
        
                return setting;
            }
            set
            {
                foreach (var entry in colors)
                {
                    entry.Value.Value = value.GetValue(entry.Key);
                }
                highlight.Value = value.HighlightValue;
        
            }
        }
        
        public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

        public override void BuildLookup()
        {
            if (settingsLookup != null) return;

            settingsLookup = new Dictionary<SettingNameTileColors, ISettingEntry>();
            foreach (var color in colors)
            {
                settingsLookup.Add((SettingNameTileColors)color.Key, color.Value);
            }
            settingsLookup.Add(SettingNameTileColors.HighlightValue, highlight);
        }
        
        public enum SettingNameTileColors
        {
            Default = 0,
            Start = 1,
            Destination = 2,
            Blocked = 3,
            HighlightValue = 4
        }
    }
}