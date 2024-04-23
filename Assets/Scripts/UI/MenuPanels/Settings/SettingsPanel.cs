using System;
using System.Collections.Generic;
using Settings;
using UI.Buttons;
using UI.MenuPanels.Settings.Logic;
using UnityEngine;

namespace UI.MenuPanels.Settings
{
    public class SettingsPanel : BasePanel
    {
        private const string ResetToDefaultPopupHeader = "Reset to default?";
        private const string ResetToDefaultPopupMessage = "Are You sure You want reset all settings to default?";
        
        [SerializeField] private float scrollingDisplacementMargin = 30;
        
        [SerializeField] private Button resetButton;
        [SerializeField] private Button resetToDefaultButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform panelsUIParent;

        [SerializeField] private UILogicSettingGroupPanel[] panels;

        private readonly Dictionary<Enums.SettingName, IUILogicSetting> settingEntries = new Dictionary<Enums.SettingName, IUILogicSetting>();

        private Action onResetToDefault;
        private Action<GameSettings, Enums.SettingsReloadingParam> onSave;
        
        public void Initialize(Action onBack, Action onResetToDefault, Action<GameSettings, Enums.SettingsReloadingParam> onSave)
        {
            base.Initialize(onBack);
            this.onResetToDefault = onResetToDefault;
            this.onSave = onSave;
            resetToDefaultButton.OnPressAction += OnResetValuesToDefault;
            resetButton.OnPressAction += ResetPanel;
            saveButton.OnPressAction += Save;
            Init();
            BuildLookup();
            InitUI();
            CalcEntryPosRelatedToRoot();
            InitButtonsNavigation();
            LoadInputValues();
        }

        protected override void SelectDefaultButton() => backButton.Select();

        protected override void ResetPanel() => LoadInputValues();

        private void Init()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;
            
            foreach (UILogicSettingGroupPanel panel in panels)
            {
                panel.Init(gameSettings);
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
            Utility.Utility.RefreshLayoutGroupsImmediate(scrollRect.Content);
            Utility.Utility.RefreshLayoutGroupsImmediate(scrollRect.Content);
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
                ResetToDefaultPopupHeader,
                ResetToDefaultPopupMessage,
                ResetValuesToDefault
            );

        private void ResetValuesToDefault()
        {
            onResetToDefault.Invoke();
            LoadInputValues(true);
        }

        private void LoadInputValues(bool loadDefault = false)
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            if (loadDefault)
            {
                gameSettings.LoadDefault();
            }
            
            foreach (KeyValuePair<Enums.SettingName, IUILogicSetting> entry in settingEntries)
            {
                entry.Value.SetValue(gameSettings.GetSetting(entry.Key));
            }
        }

        private void Save()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;
            foreach (KeyValuePair<Enums.SettingName, IUILogicSetting> entry in settingEntries)
            {
                gameSettings.GetSetting(entry.Key).SetValue(entry.Value);
            }

            Enums.SettingsReloadingParam reloadingParam = Enums.SettingsReloadingParam.None;
            List<Enums.SettingGroupStaticKey> settingGroupChangedValues = new List<Enums.SettingGroupStaticKey>();

            foreach (UILogicSettingGroupPanel panel in panels)
            {
                settingGroupChangedValues.AddRange(panel.ChangedValues);
            }
            
            if (settingGroupChangedValues.Contains(Enums.SettingGroupStaticKey.BoardSize) ||
                settingGroupChangedValues.Contains(Enums.SettingGroupStaticKey.TileDimensions))
            {
                reloadingParam = Enums.SettingsReloadingParam.Maze;
            }
            else if (settingGroupChangedValues.Contains(Enums.SettingGroupStaticKey.TileColors))
            {
                reloadingParam = Enums.SettingsReloadingParam.TileColors;
            }
            
            onSave.Invoke(gameSettings, reloadingParam);
        }
    }
}