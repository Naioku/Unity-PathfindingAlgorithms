using System;
using System.Collections.Generic;
using System.Linq;
using Settings;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.Logic
{
    [Serializable]
    public class UILogicSettingGroup
    {
        [SerializeField] private Enums.SettingGroupName name;
        [SerializeField] private Enums.SettingName[] settingNames;
        
        public List<IUILogicSetting> Settings { get; } = new();
        public IUILogicSetting FirstSetting => Settings.First();
        public IUILogicSetting LastSetting => Settings.Last();

        public void InitBaseLogic(GameSettings gameSettings)
        {
            foreach (Enums.SettingName settingName in settingNames)
            {
                ISetting setting = gameSettings.GetSetting(settingName);
                IUILogicSetting uiLogicSetting = setting.UILogicSetting;
                uiLogicSetting.InitBaseLogic(settingName, name);
                Settings.Add(uiLogicSetting);
            }
        }
        
        public void InitUI(RectTransform uiParent, Action<EntryPosition> onSelect)
        {
            ViewSettingGroup viewSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<ViewSettingGroup>(Enums.UISpawned.SettingGroupEntry, uiParent);
            viewSettingGroup.Initialize(name);
            foreach (IUILogicSetting setting in Settings)
            {
                setting.InitUI(viewSettingGroup.UIParent);
                setting.OnSelect += onSelect;
            }
        }
        
        public void CalcEntryPosRelatedTo(RectTransform contentRoot)
        {
            foreach (IUILogicSetting setting in Settings)
            {
                setting.CalcEntryPosRelatedTo(contentRoot);
            }
        }

        public void InitButtonsNavigation(UILogicSettingGroup prevGroupSetting, UILogicSettingGroup nextGroupSetting)
        {
            int entriesCount = Settings.Count;
            for (var i = 0; i < entriesCount; i++)
            {
                IUILogicSetting onUp = null;
                IUILogicSetting onDown = null;
                
                int prevIndex = i - 1;
                int nextIndex = i + 1;

                if (prevIndex >= 0)
                {
                    onUp = Settings[prevIndex];
                }
                
                if (nextIndex < entriesCount)
                {
                    onDown = Settings[nextIndex];
                }
                
                if (i == 0)
                {
                    onUp = prevGroupSetting?.LastSetting;
                }
                
                if (i == entriesCount - 1)
                {
                    onDown = nextGroupSetting?.FirstSetting;
                }

                Settings[i].SetNavigation(new SettingNavigation
                {
                    OnUp = onUp,
                    OnDown = onDown
                });
            }
        }
    }
}