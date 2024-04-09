using System;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingEntries
{
    public interface ISettingEntry
    {
        bool ChangedThroughPopup { get; }
        ViewSetting ViewSetting { get; }
        event Action<EntryPosition> OnSelect;
        void InitUI(RectTransform uiParent);
        void CalcEntryPosRelatedTo(RectTransform contentRoot);
        void SetNavigation(SettingNavigation navigation);
    }
}