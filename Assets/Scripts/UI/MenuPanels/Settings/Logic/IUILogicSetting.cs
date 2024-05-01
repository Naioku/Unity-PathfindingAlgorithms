using System;
using Settings;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.Logic
{
    public interface IUILogicSetting
    {
        Enums.SettingName Name { get; }
        Enums.SettingGroupName GroupName { get; }
        bool ChangedThroughPopup { get; set; }
        ViewSetting ViewSetting { get; }
        event Action<EntryPosition> OnSelect;
        void SetValue(ISetting setting, Enums.SettingLoadingParam param);
        void InitBaseLogic(Enums.SettingName name, Enums.SettingGroupName groupName);
        void InitUI(RectTransform uiParent);
        void CalcEntryPosRelatedTo(RectTransform contentRoot);
        void SetNavigation(SettingNavigation navigation);
    }
}