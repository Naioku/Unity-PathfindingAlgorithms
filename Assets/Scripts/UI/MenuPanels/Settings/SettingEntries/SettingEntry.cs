using System;
using System.Globalization;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.SettingEntries
{
    public class SettingEntry<T> : ISettingEntry
    {
        private T value;
        private readonly string popupHeader;
        private readonly string name;

        public bool ChangedThroughPopup { get; private set; }
        public ViewSetting ViewSetting { get; private set; }
        public event Action<EntryPosition> OnSelect;

        public SettingEntry(string groupName, string name)
        {
            this.name = name;
            popupHeader = $"{groupName}: {name}";
        }

        public T Value
        {
            get => value;
            set
            {
                ChangedThroughPopup = false;
                SetValueInternal(value);
            }
        }

        public void InitUI(RectTransform uiParent)
        {
            ViewSetting = AllManagers.Instance.UIManager.UISpawner.CreateObject<ViewSetting>(Enums.UISpawned.SettingEntry, uiParent);
            ViewSetting.Initialize(name, OnButtonPress, OnButtonSelect);
        }
        
        public void CalcEntryPosRelatedTo(RectTransform contentRoot)
        {
            ViewSetting.CalcEntryPosRelatedTo(contentRoot);
        }

        public void SetNavigation(SettingNavigation navigation) => ViewSetting.SetNavigation(new ViewSettingNavigation
        {
            OnUp = navigation.OnUp?.ViewSetting,
            OnDown = navigation.OnDown?.ViewSetting
        });

        private void OnButtonPress() => AllManagers.Instance.UIManager.OpenPopupInput
        (
            popupHeader,
            value,
            OnClosePanel
        );

        private void OnButtonSelect(EntryPosition position) => OnSelect?.Invoke(position);

        private void OnClosePanel(T value)
        {
            SetValueInternal(value);
            ChangedThroughPopup = true;
        }

        private void SetValueInternal(T value)
        {
            this.value = value;
            switch (value)
            {
                case float floatValue:
                    ViewSetting.Button.Label = floatValue.ToString(CultureInfo.CurrentCulture);
                    break;

                case int intValue:
                    ViewSetting.Button.Label = intValue.ToString();
                    break;

                case Color colorValue:
                    ViewSetting.Button.Color = colorValue;
                    ViewSetting.Button.Label = Utility.ColorToHexString(colorValue, true);
                    break;
            }
        }
    }
}