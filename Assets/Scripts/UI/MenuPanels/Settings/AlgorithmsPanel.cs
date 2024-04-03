using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace UI.MenuPanels.Settings
{
    [Serializable]
    public class AlgorithmsPanel : SettingGroupPanel<AlgorithmsPanel.SettingGroupName>
    {
        [SerializeField] private AlgorithmStagesDelayPanel stagesDelay;

        public AlgorithmStagesDelay StagesDelaySetting
        {
            get => stagesDelay.Setting;
            set => stagesDelay.Setting = value;
        }

        public override void BuildLookup()
        {
            settingGroupsLookup ??= new Dictionary<SettingGroupName, SettingGroupInGame>
            {
                { stagesDelay.Name, stagesDelay }
            };
            
            base.BuildLookup();
        }

        [Serializable]
        private class AlgorithmStagesDelayPanel : SettingGroupInGame<SettingNameAlgorithmStages, SettingGroupName>
        {
            private const string SettingGroupName = "Algorithm stages delay";

            private Dictionary<Enums.AlgorithmStageDelay, SettingEntry<float>> delays = new Dictionary<Enums.AlgorithmStageDelay, SettingEntry<float>>
            {
                { Enums.AlgorithmStageDelay.AfterNewNodeEnqueuing, new SettingEntry<float>(SettingGroupName, "After new node enqueuing") },
                { Enums.AlgorithmStageDelay.AfterNodeChecking, new SettingEntry<float>(SettingGroupName, "After node checking") },
                { Enums.AlgorithmStageDelay.AfterCursorPositionChange, new SettingEntry<float>(SettingGroupName, "After cursor position change") },
                { Enums.AlgorithmStageDelay.AfterPathNodeSetting, new SettingEntry<float>(SettingGroupName, "After path node setting") },
            };
            
            public override SettingGroupName Name => AlgorithmsPanel.SettingGroupName.AlgorithmStagesDelay;

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

            public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

            public override void BuildLookup()
            {
                if (settingsLookup != null) return;
                
                settingsLookup = new Dictionary<SettingNameAlgorithmStages, ISettingEntry>();
                foreach (var color in delays)
                {
                    settingsLookup.Add((SettingNameAlgorithmStages)color.Key, color.Value);
                }
            }
        }
        
        private enum SettingNameAlgorithmStages
        {
            AfterNewNodeEnqueuing = 0,
            AfterNodeChecking = 1,
            AfterCursorPositionChange = 2,
            AfterPathNodeSetting = 3
        }
        
        public enum SettingGroupName
        {
            AlgorithmStagesDelay
        }
    }
}