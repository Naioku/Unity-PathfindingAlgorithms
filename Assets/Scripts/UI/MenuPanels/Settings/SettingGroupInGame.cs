using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.MenuPanels.Settings
{
    [Serializable]
    public abstract class SettingGroupInGame<TSetting, TSettingGroup> : SettingGroupInGame where TSetting : Enum where TSettingGroup : Enum
    {
        [SerializeField] private List<TSetting> settingsOrder;
        
        protected Dictionary<TSetting, ISettingEntry> settingsLookup;
        public abstract TSettingGroup Name { get; } // Todo: Is it necessary?
        public override ISettingEntry FirstSetting => settingsLookup[settingsOrder.First()];
        public override ISettingEntry LastSetting => settingsLookup[settingsOrder.Last()];

        public override bool AnyValueChanged()
        {
            foreach (var entry in settingsLookup)
            {
                if (entry.Value.ChangedThroughPopup) return false;
            }
            return true;
        }

        protected void InitUI(Transform uiParent, string settingGroupName)
        {
            UISettingGroup uiSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISettingGroup>(Enums.UISpawned.SettingGroupEntry);
            uiSettingGroup.Initialize(uiParent, settingGroupName);
            foreach (TSetting key in settingsOrder)
            {
                uiSettingGroup.AddChild(settingsLookup[key].InitializeUI());
            }
        }

        public override void InitButtonsNavigation(SettingGroupInGame prevGroupSetting, SettingGroupInGame nextGroupSetting)
        {
            List<ISettingEntry> entries = new List<ISettingEntry>();
            foreach (TSetting key in settingsOrder)
            {
                entries.Add(settingsLookup[key]);
            }

            int entriesCount = entries.Count;
            for (var i = 0; i < entriesCount; i++)
            {
                ISettingEntry onUp = null;
                ISettingEntry onDown = null;
                
                int prevIndex = i - 1;
                int nextIndex = i + 1;

                if (prevIndex >= 0)
                {
                    onUp = entries[prevIndex];
                }
                
                if (nextIndex < entriesCount)
                {
                    onDown = entries[nextIndex];
                }
                
                if (i == 0)
                {
                    onUp = prevGroupSetting?.LastSetting;
                }
                
                if (i == entriesCount - 1)
                {
                    onDown = nextGroupSetting?.FirstSetting;
                }

                entries[i].SetNavigation(new SettingNavigation
                {
                    OnUp = onUp,
                    OnDown = onDown
                });
            }
        }
    }
    
    public abstract class SettingGroupInGame
    {
        public abstract ISettingEntry FirstSetting { get; }
        public abstract ISettingEntry LastSetting { get; }
        public abstract void BuildLookup();
        public abstract void InitUI(Transform uiParent);
        public abstract void InitButtonsNavigation(SettingGroupInGame prevGroupSetting, SettingGroupInGame nextGroupSetting);
        public abstract bool AnyValueChanged();
    }
}