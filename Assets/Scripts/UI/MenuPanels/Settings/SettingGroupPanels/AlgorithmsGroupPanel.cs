using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels.Settings.SettingGroups;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingGroupPanels
{
    [Serializable]
    public class AlgorithmsGroupPanel : SettingGroupPanel<AlgorithmsGroupPanel.SettingGroupName>
    {
        [SerializeField] private AlgorithmStagesDelayGroup stagesDelay;

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
        
        public enum SettingGroupName
        {
            AlgorithmStagesDelay
        }
    }
}