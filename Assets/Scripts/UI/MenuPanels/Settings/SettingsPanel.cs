using System;
using System.Collections.Generic;
using Settings;
using UI.Buttons;
using UI.Localization;
using UI.MenuPanels.Settings.Logic;
using UnityEngine;
using Utilities;

namespace UI.MenuPanels.Settings
{
    public class SettingsPanel : BasePanel
    {
        [SerializeField] private float scrollingDisplacementMargin = 30;
        
        [SerializeField] private LocalizedTextMeshPro header;
        [SerializeField] private ButtonTextLocalized resetButton;
        [SerializeField] private ButtonTextLocalized resetToDefaultButton;
        [SerializeField] private ButtonTextLocalized saveButton;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform panelsUIParent;
        [SerializeField] private UILogicSettingGroupPanel[] panels;

        private LocalizedContentCache localizedContentCache;
        private readonly Dictionary<Enums.SettingName, IUILogicSetting> settingEntries = new();
        private readonly Dictionary<Enums.SettingGroupName, Enums.SettingsReloadingParam> reloadParamsLookup = new()
        {
            { Enums.SettingGroupName.BoardSize, Enums.SettingsReloadingParam.Maze },
            { Enums.SettingGroupName.TileDimensions, Enums.SettingsReloadingParam.Maze },
            { Enums.SettingGroupName.TileColors, Enums.SettingsReloadingParam.TileColors },
            { Enums.SettingGroupName.Miscellaneous, Enums.SettingsReloadingParam.Language }
        };

        private Action onResetToDefault;
        private Action<Enums.SettingsReloadingParam> onSave;

        private void Awake() => localizedContentCache = new LocalizedContentCache
        (
            Enums.PopupText.SettingsResetToDefaultHeader,
            Enums.PopupText.SettingsResetToDefaultMessage,
            Enums.PopupText.SettingsSavedHeader,
            Enums.PopupText.SettingsSavedMessage
        );

        public void Initialize(Action onBack, Action onResetToDefault, Action<Enums.SettingsReloadingParam> onSave)
        {
            base.Initialize(onBack);
            this.onResetToDefault = onResetToDefault;
            this.onSave = onSave;
            resetToDefaultButton.OnPressAction += OnResetValuesToDefault;
            resetButton.OnPressAction += ResetPanel;
            saveButton.OnPressAction += OnSave;
            resetToDefaultButton.Initialize(Enums.GeneralText.SettingsButtonResetToDefault);
            resetButton.Initialize(Enums.GeneralText.SettingsButtonReset);
            saveButton.Initialize(Enums.GeneralText.SettingsButtonSave);
            header.Initialize(Enums.GeneralText.SettingsHeader);
            InitBaseLogic();
            BuildLookup();
            InitUI();
            CalcEntryPosRelatedToRoot();
            InitButtonsNavigation();
            LoadInputValues(Enums.SettingLoadingParam.Init);
        }

        private void OnDestroy() => header.Destroy();

        protected override void SelectDefaultButton() => backButton.Select();

        protected override void ResetPanel() => LoadInputValues(Enums.SettingLoadingParam.Reset);

        private void InitBaseLogic()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;
            
            foreach (UILogicSettingGroupPanel panel in panels)
            {
                panel.InitBaseLogic(gameSettings);
            }
        }

        private void BuildLookup()
        {
            foreach (UILogicSettingGroupPanel panel in panels)
            {
                foreach (IUILogicSetting setting in panel.Settings)
                {
                    settingEntries.Add(setting.Name, setting);
                }
            }
        }

        private void InitUI()
        {
            foreach (UILogicSettingGroupPanel panel in panels)
            {
                panel.InitUI(panelsUIParent, OnSelectEntry);
            }
            
            // Hack for Unity's odd UI working... It has to be called twice.
            Utility.RefreshLayoutGroupsImmediate(scrollRect.Content);
            Utility.RefreshLayoutGroupsImmediate(scrollRect.Content);
        }
        
        private void OnSelectEntry(EntryPosition position)
        {
            float yOffset = 0;

            float offsetFromTop = scrollRect.CalcOffsetFromViewEdgeTop(position.Max);
            float offsetFromBottom = scrollRect.CalcOffsetFromViewEdgeBottom(position.Min);

            if (offsetFromTop > 0)
            {
                yOffset = offsetFromTop + scrollingDisplacementMargin;
            }
            else if (offsetFromBottom < 0)
            {
                yOffset = offsetFromBottom - scrollingDisplacementMargin;
            }
            
            scrollRect.MoveScrollViewBy(new Vector2(0, yOffset));
        }

        private void CalcEntryPosRelatedToRoot()
        {
            foreach (UILogicSettingGroupPanel panel in panels)
            {
                panel.CalcEntryPosRelatedTo(scrollRect.Content);
            }
        }

        private void InitButtonsNavigation()
        {
            UILogicSettingGroupPanel firstPanel = panels[0];
            backButton.SetNavigation(Enums.ButtonsNaviDirection.Down, firstPanel.FirstGroup.FirstSetting.ViewSetting.Button);
            resetButton.SetNavigation(Enums.ButtonsNaviDirection.Down, firstPanel.FirstGroup.FirstSetting.ViewSetting.Button);
            resetToDefaultButton.SetNavigation(Enums.ButtonsNaviDirection.Down, firstPanel.FirstGroup.FirstSetting.ViewSetting.Button);
            saveButton.SetNavigation(Enums.ButtonsNaviDirection.Down, firstPanel.FirstGroup.FirstSetting.ViewSetting.Button);

            for (var i = 0; i < panels.Length; i++)
            {
                var panel = panels[i];
                
                if (i == 0)
                {
                    panel.InitButtonsNavigation(null, panels[i + 1]);
                }
                else if (i == panels.Length - 1)
                {
                    panel.InitButtonsNavigation(panels[i - 1], null);
                }
                else
                {
                    panel.InitButtonsNavigation(panels[i - 1], panels[i + 1]);
                }
            }

            // Todo: When You delete upper buttons, delete also this line.
            firstPanel.FirstGroup.FirstSetting.ViewSetting.Button.SetNavigation(Enums.ButtonsNaviDirection.Up, saveButton);
        }

        private void OnResetValuesToDefault() =>
            AllManagers.Instance.UIManager.OpenPopupConfirmation
            (
                localizedContentCache.GetValue(Enums.PopupText.SettingsResetToDefaultHeader),
                localizedContentCache.GetValue(Enums.PopupText.SettingsResetToDefaultMessage),
                ResetValuesToDefault
            );

        private void ResetValuesToDefault()
        {
            onResetToDefault.Invoke();
            LoadInputValues(Enums.SettingLoadingParam.Standard);
            Save(false);
        }

        private void LoadInputValues(Enums.SettingLoadingParam param)
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            if (param == Enums.SettingLoadingParam.Standard)
            {
                gameSettings.LoadDefault();
            }
            
            foreach (KeyValuePair<Enums.SettingName, IUILogicSetting> entry in settingEntries)
            {
                entry.Value.SetValue(gameSettings.GetSetting(entry.Key), param);
            }
        }

        private void OnSave() => Save(true);
        
        private void Save(bool openPopup)
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;
            // List<Enums.SettingGroupName> settingGroupChangedValues = new List<Enums.SettingGroupName>();
            Enums.SettingsReloadingParam reloadingParam = default;
            foreach (KeyValuePair<Enums.SettingName, IUILogicSetting> entry in settingEntries)
            {
                IUILogicSetting setting = entry.Value;
                if (!setting.ChangedThroughPopup) continue;
                
                gameSettings.GetSetting(entry.Key).SetValue(setting);
                if (reloadParamsLookup.TryGetValue(setting.GroupName, out Enums.SettingsReloadingParam parameter))
                {
                    reloadingParam |= parameter;
                    setting.ChangedThroughPopup = false;
                }
            }
            
            // if (settingGroupChangedValues.Contains(Enums.SettingGroupName.BoardSize) ||
            //     settingGroupChangedValues.Contains(Enums.SettingGroupName.TileDimensions))
            // {
            //     reloadingParam |= Enums.SettingsReloadingParam.Maze;
            // }
            //
            // if (settingGroupChangedValues.Contains(Enums.SettingGroupName.TileColors))
            // {
            //     reloadingParam |= Enums.SettingsReloadingParam.TileColors;
            // }
            //
            // if (settingGroupChangedValues.Contains(Enums.SettingGroupName.Miscellaneous))
            // {
            //     reloadingParam |= Enums.SettingsReloadingParam.Language;
            // }
            
            onSave.Invoke(reloadingParam);

            if (openPopup)
            {
                AllManagers.Instance.UIManager.OpenPopupInfo
                (
                    localizedContentCache.GetValue(Enums.PopupText.SettingsSavedHeader),
                    localizedContentCache.GetValue(Enums.PopupText.SettingsSavedMessage)
                );
            }
        }
    }
}