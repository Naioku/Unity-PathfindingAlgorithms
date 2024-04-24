using System;
using Settings;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.Logic
{
    public interface IUILogicSetting
    {
        Enums.SettingName Name { get; }
        Enums.SettingGroupStaticKey SettingGroup { get; }
        bool ChangedThroughPopup { get; set; }
        ViewSetting ViewSetting { get; }
        event Action<EntryPosition> OnSelect;
        void SetValue(ISetting setting, Enums.SettingLoadingParam param);
        void Init(Enums.SettingName name, Enums.SettingGroupStaticKey groupNameStaticKey);
        void InitUI(RectTransform uiParent);
        void CalcEntryPosRelatedTo(RectTransform contentRoot);
        void SetNavigation(SettingNavigation navigation);
    }
}