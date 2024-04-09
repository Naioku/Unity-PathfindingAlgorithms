using System;
using System.Collections.Generic;
using UI.MenuPanels.Settings.SettingEntries;
using UI.MenuPanels.Settings.SettingGroupPanels;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingGroups
{
    [Serializable]
    public class BoardSizeGroup : SettingGroupInGame<BoardSizeGroup.SettingNameSize, TilesGroupPanel.SettingGroupName>
    {
        private const string DisplayName = "Board size";

        private SettingEntry<int> width = new SettingEntry<int>(DisplayName, "Width");
        private SettingEntry<int> length = new SettingEntry<int>(DisplayName, "Length");

        public override TilesGroupPanel.SettingGroupName Name => TilesGroupPanel.SettingGroupName.Size;
        protected override string SettingGroupName => DisplayName;

        public Vector2Int Setting
        {
            get => new Vector2Int(width.Value, length.Value);
            set
            {
                width.Value = value.x;
                length.Value = value.y;
            }
        }

        public override void BuildLookup()
        {
            settingsLookup = new Dictionary<SettingNameSize, ISettingEntry>
            {
                { SettingNameSize.Width, width },
                { SettingNameSize.Length, length }
            };
        }
            
        public enum SettingNameSize
        {
            Width,
            Length
        }
    }
}