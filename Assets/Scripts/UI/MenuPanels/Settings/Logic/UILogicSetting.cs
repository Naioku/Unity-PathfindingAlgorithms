using System;
using System.Globalization;
using Settings;
using UI.Localization;
using UI.MenuPanels.Settings.View;
using UnityEngine;
using UnityEngine.Localization;

namespace UI.MenuPanels.Settings.Logic
{
    public class UILogicSetting<T> : IUILogicSetting
    {
        protected T value;
        protected LocalizedString nameText;
        protected LocalizedString groupNameText;

        public Enums.SettingName Name { get; private set; }
        public Enums.SettingGroupName GroupName { get; private set; }
        public bool ChangedThroughPopup { get; set; }
        public ViewSetting ViewSetting { get; private set; }

        public event Action<EntryPosition> OnSelect;

        public T Value => value;

        public virtual void SetValue(ISetting setting, Enums.SettingLoadingParam param)
        {
            switch (param)
            {
                case Enums.SettingLoadingParam.Standard:
                    ChangedThroughPopup = true;
                    break;
                
                case Enums.SettingLoadingParam.Reset:
                    ChangedThroughPopup = false;
                    break;
            }
            
            SetValueInternal(((Setting<T>)setting).Value);
        }

        public void InitBaseLogic(Enums.SettingName name, Enums.SettingGroupName groupName)
        {
            Name = name;
            GroupName = groupName;
            LocalizedTextManager localizedTextManager = AllManagers.Instance.LocalizedTextManager;
            nameText = localizedTextManager.GetLocalizedString(name);
            groupNameText = localizedTextManager.GetLocalizedString(groupName);
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
                $"{groupNameText.GetLocalizedString()}: {nameText.GetLocalizedString()}",
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