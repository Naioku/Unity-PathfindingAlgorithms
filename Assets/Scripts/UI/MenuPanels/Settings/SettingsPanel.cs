using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using Button = UI.Buttons.Button;

namespace UI.MenuPanels.Settings
{
    public class SettingsPanel : BasePanel
    {
        private const string ResetToDefaultPopupHeader = "Reset to default?";
        private const string ResetToDefaultPopupMessage = "Are You sure You want reset all settings to default?";
        
        [SerializeField] private Button resetButton;
        [SerializeField] private Button resetToDefaultButton;
        [SerializeField] private Button saveButton;

        [SerializeField] private TilesPanel tilesPanel;
        [SerializeField] private AlgorithmsPanel algorithmsPanel;
        
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
            InitButtonsNavigation();
            LoadInputValues();
        }

        protected override void SelectDefaultButton() => backButton.Select();
        protected override void ResetPanel() => LoadInputValues();

        private void BuildLookups()
        {
            tilesPanel.BuildLookup();
            algorithmsPanel.BuildLookup();
        }

        private void InitUIs()
        {
            tilesPanel.InitUI();
            algorithmsPanel.InitUI();
        }

        private void InitButtonsNavigation()
        {
            backButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesPanel.FirstGroup.FirstSetting.UISetting.Button);
            resetButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesPanel.FirstGroup.FirstSetting.UISetting.Button);
            resetToDefaultButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesPanel.FirstGroup.FirstSetting.UISetting.Button);
            saveButton.SetNavigation(Enums.ButtonsNaviDirection.Down, tilesPanel.FirstGroup.FirstSetting.UISetting.Button);
            
            tilesPanel.InitButtonsNavigation(null, algorithmsPanel);
            algorithmsPanel.InitButtonsNavigation(tilesPanel, null);
            
            // Todo: When You will delete upper buttons, delete also this line.
            tilesPanel.FirstGroup.FirstSetting.UISetting.Button.SetNavigation(Enums.ButtonsNaviDirection.Up, saveButton);
        }

        private void LoadInputValues()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            tilesPanel.SizeSetting = gameSettings.Size;
            tilesPanel.TileDimensionsSetting = gameSettings.TileDimensions;
            tilesPanel.TileColorsSetting = gameSettings.TileColors;
            tilesPanel.MarkerColorsSetting = gameSettings.MarkerColors;
            algorithmsPanel.StagesDelaySetting = gameSettings.AlgorithmStagesDelay;
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
                tilesPanel.SizeSetting,
                tilesPanel.TileDimensionsSetting,
                tilesPanel.TileColorsSetting,
                tilesPanel.MarkerColorsSetting,
                algorithmsPanel.StagesDelaySetting,
                permittedDirections
            );

            Enums.SettingsReloadingParam reloadingParam = Enums.SettingsReloadingParam.None;
            List<TilesPanel.SettingGroupName> settingGroupChangedValues = tilesPanel.ChangedValues;
            
            if (settingGroupChangedValues.Contains(TilesPanel.SettingGroupName.Size) ||
                settingGroupChangedValues.Contains(TilesPanel.SettingGroupName.TileDimensions))
            {
                reloadingParam = Enums.SettingsReloadingParam.Maze;
            }
            else if (settingGroupChangedValues.Contains(TilesPanel.SettingGroupName.Size))
            {
                reloadingParam = Enums.SettingsReloadingParam.TileColors;
            }
            
            onSave.Invoke(gameSettings, reloadingParam);
        }
    }
}