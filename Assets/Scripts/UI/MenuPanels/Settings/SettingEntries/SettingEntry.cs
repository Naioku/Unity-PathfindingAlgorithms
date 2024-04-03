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

        public ViewSetting InitializeUI()
        {
            ViewSetting = AllManagers.Instance.UIManager.UISpawner.CreateObject<ViewSetting>(Enums.UISpawned.SettingEntry);
            ViewSetting.Initialize(name, ButtonAction);
            
            return ViewSetting;
        }

        public void SetNavigation(SettingNavigation navigation) => ViewSetting.SetNavigation(new ViewSettingNavigation
        {
            OnUp = navigation.OnUp?.ViewSetting,
            OnDown = navigation.OnDown?.ViewSetting
        });

        public bool IsInteractable() => ViewSetting.IsInteractable();

        private void ButtonAction() => AllManagers.Instance.UIManager.OpenPopupInput
        (
            popupHeader,
            value,
            OnClosePanel
        );

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