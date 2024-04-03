using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels.Settings.SettingEntries;
using UI.MenuPanels.Settings.SettingGroupPanels;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingGroups
{
    [Serializable]
    public class MarkerColorsGroup : SettingGroupInGame<MarkerColorsGroup.SettingNameMarkerColors, TilesGroupPanel.SettingGroupName>
    {
        private const string SettingGroupName = "Marker colors";
        
        private Dictionary<Enums.MarkerType, SettingEntry<Color>> colors = new Dictionary<Enums.MarkerType, SettingEntry<Color>>
        {
            { Enums.MarkerType.None, new SettingEntry<Color>(SettingGroupName, "None") },
            { Enums.MarkerType.ReadyToCheck, new SettingEntry<Color>(SettingGroupName, "Ready to check") },
            { Enums.MarkerType.Checked, new SettingEntry<Color>(SettingGroupName, "Checked") },
            { Enums.MarkerType.Path, new SettingEntry<Color>(SettingGroupName, "Path") },
        };
        private readonly SettingEntry<float> alpha = new SettingEntry<float>(SettingGroupName, "Opacity");
        
        public override TilesGroupPanel.SettingGroupName Name => TilesGroupPanel.SettingGroupName.MarkerColors;
        public MarkerColors Setting
        {
            get
            {
                MarkerColors setting = new MarkerColors();
                foreach (var entry in colors)
                {
                    setting.SetValue(entry.Key, entry.Value.Value);
                }
                setting.Alpha = alpha.Value;

                return setting;
            }
            set
            {
                foreach (var entry in colors)
                {
                    entry.Value.Value = value.GetValue(entry.Key);
                }
                alpha.Value = value.Alpha;

            }
        }

        public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

        public override void BuildLookup()
        {
            if (settingsLookup != null) return;
            
            settingsLookup = new Dictionary<SettingNameMarkerColors, ISettingEntry>();
            foreach (var color in colors)
            {
                settingsLookup.Add((SettingNameMarkerColors)color.Key, color.Value);
            }
            settingsLookup.Add(SettingNameMarkerColors.Alpha, alpha);
        }
        
        public enum SettingNameMarkerColors
        {
            None = 0,
            ReadyToCheck = 1,
            Destination = 2,
            Blocked = 3,
            Alpha = 4
        }
    }
}