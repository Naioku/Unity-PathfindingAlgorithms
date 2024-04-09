using System;
using System.Collections.Generic;
using Settings;
using UI.MenuPanels.Settings.SettingEntries;
using UI.MenuPanels.Settings.SettingGroupPanels;
using UnityEngine;
using Button = UI.Buttons.Button;

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
        [SerializeField] private TilesGroupPanel tilesGroupPanel;
        [SerializeField] private AlgorithmsGroupPanel algorithmsGroupPanel;
        [SerializeField] private ScrollRect scrollRect;
        
        // Todo: Temporary.
        private Enums.PermittedDirection[] permittedDirections = {
            Enums.PermittedDirection.UpRight,
            Enums.PermittedDirection.Up,
            Enums.PermittedDirection.UpLeft,
            Enums.PermittedDirection.Left,
            Enums.PermittedDirection.DownLeft,
            Enums.PermittedDirection.Down,
            Enums.PermittedDirection.DownRight,
            Enums.PermittedDirection.Right
        };

        private Action onResetToDefault;
        private Action<GameSettings, Enums.SettingsReloadingParam> onSave;
        
        public void Initialize(Action onBack, Action onResetToDefault, Action<GameSettings, Enums.SettingsReloadingParam> onSave)
        {
            base.Initialize(onBack);
            this.onResetToDefault = onResetToDefault;
            this.onSave = onSave;
            resetToDefaultButton.OnPressAction += OnResetValuesToDefault;
            resetButton.OnPressAction += LoadInputValues;
            saveButton.OnPressAction += Save;
            BuildLookups();
            InitUIs();
            CalcEntryPosRelatedToRoot();
            InitButtonsNavigation();
            LoadInputValues();
        }

        protected override void SelectDefaultButton() => backButton.Select();

        protected override void ResetPanel() => LoadInputValues();

        private void BuildLookups()
        {
            tilesGroupPanel.BuildLookup();
            algorithmsGroupPanel.BuildLookup();
        }

        private void InitUIs()
        {
            tilesGroupPanel.InitUI(OnSelectEntry);
            algorithmsGroupPanel.InitUI(OnSelectEntry);
            
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
            tilesGroupPanel.CalcEntryPosRelatedTo(scrollRect.Content);
            algorithmsGroupPanel.CalcEntryPosRelatedTo(scrollRect.Content);
        }

        private void InitButtonsNavigation()
        {
            backButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesGroupPanel.FirstGroup.FirstSetting.ViewSetting.Button);
            resetButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesGroupPanel.FirstGroup.FirstSetting.ViewSetting.Button);
            resetToDefaultButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesGroupPanel.FirstGroup.FirstSetting.ViewSetting.Button);
            saveButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesGroupPanel.FirstGroup.FirstSetting.ViewSetting.Button);
            
            tilesGroupPanel.InitButtonsNavigation(null, algorithmsGroupPanel);
            algorithmsGroupPanel.InitButtonsNavigation(tilesGroupPanel, null);
            
            // Todo: When You delete upper buttons, delete also this line.
            tilesGroupPanel.FirstGroup.FirstSetting.ViewSetting.Button.SetNavigation(Enums.ButtonsNaviDirection.Up, saveButton);
        }

        private void LoadInputValues()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            tilesGroupPanel.SizeSetting = gameSettings.Size;
            tilesGroupPanel.TileDimensionsSetting = gameSettings.TileDimensions;
            tilesGroupPanel.TileColorsSetting = gameSettings.TileColors;
            tilesGroupPanel.MarkerColorsSetting = gameSettings.MarkerColors;
            algorithmsGroupPanel.StagesDelaySetting = gameSettings.AlgorithmStagesDelay;
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
            LoadInputValues();
        }

        private void Save()
        {
            GameSettings gameSettings = new GameSettings
            (
                tilesGroupPanel.SizeSetting,
                tilesGroupPanel.TileDimensionsSetting,
                tilesGroupPanel.TileColorsSetting,
                tilesGroupPanel.MarkerColorsSetting,
                algorithmsGroupPanel.StagesDelaySetting,
                permittedDirections
            );

            Enums.SettingsReloadingParam reloadingParam = Enums.SettingsReloadingParam.None;
            List<TilesGroupPanel.SettingGroupName> settingGroupChangedValues = tilesGroupPanel.ChangedValues;
            
            if (settingGroupChangedValues.Contains(TilesGroupPanel.SettingGroupName.Size) ||
                settingGroupChangedValues.Contains(TilesGroupPanel.SettingGroupName.TileDimensions))
            {
                reloadingParam = Enums.SettingsReloadingParam.Maze;
            }
            else if (settingGroupChangedValues.Contains(TilesGroupPanel.SettingGroupName.Size))
            {
                reloadingParam = Enums.SettingsReloadingParam.TileColors;
            }
            
            onSave.Invoke(gameSettings, reloadingParam);
        }
    }
}