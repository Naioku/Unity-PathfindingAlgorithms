using System;
using Settings;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.Logic
{
    public interface IUILogicSetting
    {
        Enums.SettingName Name { get; }
        bool ChangedThroughPopup { get; }
        ViewSetting ViewSetting { get; }
        event Action<EntryPosition> OnSelect;
        void SetValue(ISetting setting);
        void Init(Enums.SettingName name, Enums.SettingGroupStaticKey groupNameStaticKey);
        void InitUI(RectTransform uiParent);
        void CalcEntryPosRelatedTo(RectTransform contentRoot);
        void SetNavigation(SettingNavigation navigation);
    }
}