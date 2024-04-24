using System;
using System.Collections.Generic;
using System.Linq;
using Settings;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.Logic
{
    [Serializable]
    public class UILogicSettingGroupPanel
    {
        [SerializeField] private Enums.SettingGroupPanelStaticKey nameStaticKey;
        [SerializeField] private UILogicSettingGroup[] groups;

        public UILogicSettingGroup FirstGroup => groups.First();
        public UILogicSettingGroup LastGroup => groups.Last();
        
        public List<IUILogicSetting> Settings
        {
            get
            {
                List<IUILogicSetting> result = new List<IUILogicSetting>();
                foreach (UILogicSettingGroup settingGroup in groups)
                {
                    result.AddRange(settingGroup.Settings);
                }

                return result;
            }
        }

        public void Init(GameSettings gameSettings)
        {
            foreach (UILogicSettingGroup group in groups)
            {
                group.Init(gameSettings);
            }
        }
        
        public void InitUI(RectTransform uiParent, Action<EntryPosition> onSelect)
        {
            ViewSettingGroupPanel viewSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<ViewSettingGroupPanel>(Enums.UISpawned.SettingGroupPanel, uiParent);
            viewSettingGroup.Initialize(nameStaticKey);
            foreach (UILogicSettingGroup group in groups)
            {
                group.InitUI(viewSettingGroup.UIParent, onSelect);
            }
        }

        public void CalcEntryPosRelatedTo(RectTransform contentRoot)
        {
            foreach (UILogicSettingGroup group in groups)
            {
                group.CalcEntryPosRelatedTo(contentRoot);
            }
        }
        
        public void InitButtonsNavigation(UILogicSettingGroupPanel prevPanel, UILogicSettingGroupPanel nextPanel)
        {
            int settingGroupsCount = groups.Length;
            for (var i = 0; i < settingGroupsCount; i++)
            {
                UILogicSettingGroup prevGroup = null;
                UILogicSettingGroup nextGroup = null;

                int prevIndex = i - 1;
                int nextIndex = i + 1;

                if (prevIndex >= 0)
                {
                    prevGroup = groups[prevIndex];
                }
                
                if (nextIndex < settingGroupsCount)
                {
                    nextGroup = groups[nextIndex];
                }
                
                if (i == 0)
                {
                    prevGroup = prevPanel?.LastGroup;
                }
                
                if (i == settingGroupsCount - 1)
                {
                    nextGroup = nextPanel?.FirstGroup;
                }

                groups[i].InitButtonsNavigation(prevGroup, nextGroup);
            }
        }
    }
}