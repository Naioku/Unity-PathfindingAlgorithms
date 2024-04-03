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
        private const string SettingGroupName = "Board size";

        private SettingEntry<int> width = new SettingEntry<int>(SettingGroupName, "Width");
        private SettingEntry<int> length = new SettingEntry<int>(SettingGroupName, "Length");

        public override TilesGroupPanel.SettingGroupName Name => TilesGroupPanel.SettingGroupName.Size;

        public Vector2Int Setting
        {
            get => new Vector2Int(width.Value, length.Value);
            set
            {
                width.Value = value.x;
                length.Value = value.y;
            }
        }

        public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

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