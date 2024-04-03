﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.MenuPanels.Settings
{
    public abstract class SettingGroupPanel<T> : SettingGroupPanel
    {
        [SerializeField] private Transform uiParent;
        [SerializeField] private List<T> settingGroupsOrder;
        
        protected Dictionary<T, SettingGroupInGame> settingGroupsLookup;

        public override SettingGroupInGame FirstGroup => settingGroupsLookup[settingGroupsOrder.First()];
        public override SettingGroupInGame LastGroup => settingGroupsLookup[settingGroupsOrder.Last()];

        public List<T> ChangedValues
        {
            get
            {
                List<T> result = new List<T>();
                foreach (var entry in settingGroupsLookup)
                {
                    if (entry.Value.AnyValueChanged())
                    {
                        result.Add(entry.Key);
                    }
                }

                return result;
            }
        }

        public virtual void BuildLookup()
        {
            foreach (var entry in settingGroupsLookup)
            {
                entry.Value.BuildLookup();
            }
        }

        public void InitUI()
        {
            foreach (T groupName in settingGroupsOrder)
            {
                settingGroupsLookup[groupName].InitUI(uiParent);
            }
        }

        public void InitButtonsNavigation(SettingGroupPanel prevPanel, SettingGroupPanel nextPanel)
        {
            List<SettingGroupInGame> settingGroups = new List<SettingGroupInGame>();
            foreach (T group in settingGroupsOrder)
            {
                settingGroups.Add(settingGroupsLookup[group]);
            }

            int settingGroupsCount = settingGroups.Count;
            for (var i = 0; i < settingGroupsCount; i++)
            {
                SettingGroupInGame prevGroup = null;
                SettingGroupInGame nextGroup = null;

                int prevIndex = i - 1;
                int nextIndex = i + 1;

                if (prevIndex >= 0)
                {
                    prevGroup = settingGroups[prevIndex];
                }
                
                if (nextIndex < settingGroupsCount)
                {
                    nextGroup = settingGroups[nextIndex];
                }
                
                if (i == 0)
                {
                    prevGroup = prevPanel?.LastGroup;
                }
                
                if (i == settingGroupsCount - 1)
                {
                    nextGroup = nextPanel?.FirstGroup;
                }

                settingGroups[i].InitButtonsNavigation(prevGroup, nextGroup);
            }
        }
    }

    public abstract class SettingGroupPanel
    {
        public abstract SettingGroupInGame FirstGroup { get; }
        public abstract SettingGroupInGame LastGroup { get; }
    }
}