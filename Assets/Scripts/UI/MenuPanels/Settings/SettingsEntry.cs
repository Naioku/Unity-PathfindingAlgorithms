using System.Globalization;
using UnityEngine;

namespace UI.MenuPanels.Settings
{
    public class SettingEntry<T> : ISettingEntry
    {
        private T value;
        private readonly string popupHeader;
        private readonly string name;

        public bool ChangedThroughPopup { get; private set; }
        public UISetting UISetting { get; private set; }

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

        public UISetting InitializeUI()
        {
            UISetting = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISetting>(Enums.UISpawned.SettingEntry);
            UISetting.Initialize(name, ButtonAction);
            
            return UISetting;
        }

        public void SetNavigation(SettingNavigation navigation) => UISetting.SetNavigation(new UISettingNavigation
        {
            OnUp = navigation.OnUp?.UISetting,
            OnDown = navigation.OnDown?.UISetting
        });

        public bool IsInteractable() => UISetting.IsInteractable();

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
                    UISetting.Button.Label = floatValue.ToString(CultureInfo.CurrentCulture);
                    break;

                case int intValue:
                    UISetting.Button.Label = intValue.ToString();
                    break;

                case Color colorValue:
                    UISetting.Button.Color = colorValue;
                    UISetting.Button.Label = Utility.ColorToHexString(colorValue, true);
                    break;
            }
        }
    }
}