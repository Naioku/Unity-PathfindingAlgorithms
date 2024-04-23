using System;
using System.Globalization;
using Settings;
using UI.MenuPanels.Settings.View;
using UnityEngine;

namespace UI.MenuPanels.Settings.Logic
{
    public class UILogicSetting<T> : IUILogicSetting
    {
        protected T value;
        protected Enums.SettingGroupStaticKey groupNameStaticKey;
        protected StaticTextManager staticTextManager;

        public Enums.SettingName Name { get; private set; }
        public bool ChangedThroughPopup { get; private set; }
        public ViewSetting ViewSetting { get; private set; }

        public event Action<EntryPosition> OnSelect;

        public T Value => value;

        // Todo: Consider moving ChangedThroughPopup into the SetValueInternal().
        public virtual void SetValue(ISetting setting)
        {
            ChangedThroughPopup = false;
            SetValueInternal(((Setting<T>)setting).Value);
        }

        public void Init(Enums.SettingName name, Enums.SettingGroupStaticKey groupNameStaticKey)
        {
            Name = name;
            this.groupNameStaticKey = groupNameStaticKey;
            staticTextManager = AllManagers.Instance.StaticTextManager;
        }

        public void InitUI(RectTransform uiParent)
        {
            ViewSetting = AllManagers.Instance.UIManager.UISpawner.CreateObject<ViewSetting>(Enums.UISpawned.SettingEntry, uiParent);
            ViewSetting.Initialize(Name, OnButtonPress, OnButtonSelect);
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

        protected virtual void OnButtonPress()
        {
            AllManagers.Instance.UIManager.OpenPopupInput
            (
                $"{staticTextManager.GetValue(groupNameStaticKey)}: {staticTextManager.GetValue(Name)}",
                value,
                OnClosePanel
            );
        }

        protected void OnClosePanel(T value)
        {
            SetValueInternal(value);
            ChangedThroughPopup = true;
        }

        private void OnButtonSelect(EntryPosition position) => OnSelect?.Invoke(position);

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
                    ViewSetting.Button.Label = Utility.Utility.ColorToHexString(colorValue, true);
                    break;
            }
        }
    }
}