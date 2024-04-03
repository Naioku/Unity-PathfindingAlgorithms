using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels.Settings.SettingEntries;
using UI.MenuPanels.Settings.SettingGroupPanels;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingGroups
{
    [Serializable]
    public class AlgorithmStagesDelayGroup : SettingGroupInGame<AlgorithmStagesDelayGroup.SettingName, AlgorithmsGroupPanel.SettingGroupName>
    {
        private const string DisplayName = "Algorithm stages delay";

        private Dictionary<Enums.AlgorithmStageDelay, SettingEntry<float>> delays = new Dictionary<Enums.AlgorithmStageDelay, SettingEntry<float>>
        {
            { Enums.AlgorithmStageDelay.AfterNewNodeEnqueuing, new SettingEntry<float>(DisplayName, "After new node enqueuing") },
            { Enums.AlgorithmStageDelay.AfterNodeChecking, new SettingEntry<float>(DisplayName, "After node checking") },
            { Enums.AlgorithmStageDelay.AfterCursorPositionChange, new SettingEntry<float>(DisplayName, "After cursor position change") },
            { Enums.AlgorithmStageDelay.AfterPathNodeSetting, new SettingEntry<float>(DisplayName, "After path node setting") },
        };
        
        public override AlgorithmsGroupPanel.SettingGroupName Name => AlgorithmsGroupPanel.SettingGroupName.AlgorithmStagesDelay;

        public AlgorithmStagesDelay Setting
        {
            get
            {
                AlgorithmStagesDelay setting = new AlgorithmStagesDelay();
                foreach (var entry in delays)
                {
                    setting.SetValue(entry.Key, entry.Value.Value);
                }

                return setting;
            }
            set
            {
                foreach (var entry in delays)
                {
                    entry.Value.Value = value.GetValue(entry.Key);
                }
            }
        }

        public override void InitUI(Transform uiParent) => InitUI(uiParent, DisplayName);

        public override void BuildLookup()
        {
            if (settingsLookup != null) return;
            
            settingsLookup = new Dictionary<SettingName, ISettingEntry>();
            foreach (var color in delays)
            {
                settingsLookup.Add((SettingName)color.Key, color.Value);
            }
        }
        
        public enum SettingName
        {
            AfterNewNodeEnqueuing = 0,
            AfterNodeChecking = 1,
            AfterCursorPositionChange = 2,
            AfterPathNodeSetting = 3
        }
    }
}