using System;
using System.Collections.Generic;
using System.Globalization;
using Settings;
using UI.Buttons;
using UnityEngine;

namespace UI.MenuPanels
{
    public class SettingsPanel : BasePanel
    {
        private const string ResetToDefaultPopupHeader = "Reset to default?";
        private const string ResetToDefaultPopupMessage = "Are You sure You want reset all settings to default?";
        
        [SerializeField] private Button resetButton;
        [SerializeField] private Button resetToDefaultButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Transform uiSettingGroupEntriesTiles;
        [SerializeField] private Transform uiSettingGroupEntriesAlgorithms;
        
        private readonly Size size = new Size();
        private readonly TileDimensionsPanel tileDimensions = new TileDimensionsPanel();
        [SerializeField] private TileColorsPanel tileColors;
        [SerializeField] private MarkerColorsPanel markerColors;
        [SerializeField] private AlgorithmStagesDelayPanel stagesDelay;
        
        // Todo: Temporary.
        private Enums.PermittedDirection[] permittedDirections = {
            Enums.PermittedDirection.UpRight,
            Enums.PermittedDirection.Up,
            Enums.PermittedDirection.UpLeft,
            Enums.PermittedDirection.Left,
            Enums.PermittedDirection.DownLeft,
            Enums.PermittedDirection.Down,
            Enums.PermittedDirection.DownRight,
            Enums.PermittedDirection.Right,
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
            InitInputValues();
        }
        
        protected override void SelectDefaultButton() => backButton.Select();
        protected override void ResetPanel() => LoadInputValues();

        private void InitInputValues()
        {
            size.Initialize().SetParent(uiSettingGroupEntriesTiles);
            tileDimensions.Initialize().SetParent(uiSettingGroupEntriesTiles);
            tileColors.Initialize().SetParent(uiSettingGroupEntriesTiles);
            markerColors.Initialize().SetParent(uiSettingGroupEntriesTiles);
            stagesDelay.Initialize().SetParent(uiSettingGroupEntriesAlgorithms);
            
            LoadInputValues();
        }

        private void LoadInputValues()
        {
            GameSettings gameSettings = AllManagers.Instance.GameManager.GameSettings;

            size.Setting = gameSettings.Size;
            tileDimensions.Setting = gameSettings.TileDimensions;
            tileColors.Setting = gameSettings.TileColors;
            markerColors.Setting = gameSettings.MarkerColors;
            stagesDelay.Setting = gameSettings.AlgorithmStagesDelay;
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
                size.Setting,
                tileDimensions.Setting,
                tileColors.Setting,
                markerColors.Setting,
                stagesDelay.Setting,
                permittedDirections
            );

            Enums.SettingsReloadingParam reloadingParam = Enums.SettingsReloadingParam.None;
            if (size.AnyValueChanged() || tileDimensions.AnyValueChanged())
            {
                reloadingParam = Enums.SettingsReloadingParam.Maze;
            }
            else if (tileColors.AnyColorChanged())
            {
                reloadingParam = Enums.SettingsReloadingParam.TileColors;
            }
            
            onSave.Invoke(gameSettings, reloadingParam);
        }
        
        private class Size
        {
            private const string SettingGroupName = "Board size";
            private const string SettingNameWidth = "Width";
            private const string SettingNameLength = "Length";
            
            private readonly SettingEntry<int> width = new SettingEntry<int>();
            private readonly SettingEntry<int> length = new SettingEntry<int>();

            public UISettingGroup Initialize()
            {
                List<UISetting> uiSettings = new List<UISetting>
                {
                    width.Initialize(SettingGroupName, SettingNameWidth),
                    length.Initialize(SettingGroupName, SettingNameLength)
                };
                
                UISettingGroup uiSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISettingGroup>(Enums.UISpawned.SettingGroupEntry);
                uiSettingGroup.Initialize(SettingGroupName, uiSettings);

                return uiSettingGroup;
            }

            public Vector2Int Setting
            {
                get => new Vector2Int(width.Value, length.Value);
                set
                {
                    width.Value = value.x;
                    length.Value = value.y;
                }
            }

            public bool AnyValueChanged() => width.ChangedThroughPopup || length.ChangedThroughPopup;
        }

        private class TileDimensionsPanel
        {
            private const string SettingGroupName = "Tile dimensions";
            private const string SettingNameLength = "Tile length";
            private const string SettingNameHeight = "Tile height";
            
            private readonly SettingEntry<float> length = new SettingEntry<float>();
            private readonly SettingEntry<float> height = new SettingEntry<float>();

            public UISettingGroup Initialize()
            {
                List<UISetting> uiSettings = new List<UISetting>
                {
                    length.Initialize(SettingGroupName, SettingNameLength),
                    height.Initialize(SettingGroupName, SettingNameHeight)
                };
                
                UISettingGroup uiSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISettingGroup>(Enums.UISpawned.SettingGroupEntry);
                uiSettingGroup.Initialize(SettingGroupName, uiSettings);
                
                return uiSettingGroup;
            }

            public TileDimensions Setting
            {
                get =>
                    new TileDimensions
                    {
                        Length = length.Value,
                        Height = height.Value
                    };
                set
                {
                    length.Value = value.Length;
                    height.Value = value.Height;
                }
            }

            public bool AnyValueChanged() => length.ChangedThroughPopup || height.ChangedThroughPopup;
        }

        [Serializable]
        private class TileColorsPanel : SettingGroupChangeInGame<Enums.TileType, SettingEntry<Color>>
        {
            private const string SettingGroupName = "Tile colors";
            private const string SettingNameHighlight = "Cursor highlight";
            
            private readonly SettingEntry<float> highlight = new SettingEntry<float>();
            private readonly Dictionary<Enums.TileType, string> headersLookup = new Dictionary<Enums.TileType, string>
            {
                { Enums.TileType.Default, "Default" },
                { Enums.TileType.Start, "Start" },
                { Enums.TileType.Destination, "Destination" },
                { Enums.TileType.Blocked, "Blocked" }
            };
            
            public new UISettingGroup Initialize()
            {
                base.Initialize();
                
                List<UISetting> uiSettings = new List<UISetting>();
                foreach (Enums.TileType key in keysOrder)
                {
                    uiSettings.Add(valuesLookup[key].Initialize(SettingGroupName, headersLookup[key]));
                }
                uiSettings.Add(highlight.Initialize(SettingGroupName, SettingNameHighlight));
                
                UISettingGroup uiSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISettingGroup>(Enums.UISpawned.SettingGroupEntry);
                uiSettingGroup.Initialize(SettingGroupName, uiSettings);
                
                return uiSettingGroup;
            }
            
            public TileColors Setting
            {
                get
                {
                    TileColors setting = new TileColors();
                    foreach (var entry in valuesLookup)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }
                    setting.HighlightValue = highlight.Value;

                    return setting;
                }
                set
                {
                    foreach (var entry in valuesLookup)
                    {
                       entry.Value.Value = value.GetValue(entry.Key);
                    }
                    highlight.Value = value.HighlightValue;

                }
            }

            public bool AnyColorChanged()
            {
                foreach (var entry in valuesLookup)
                {
                    if (entry.Value.ChangedThroughPopup)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
        [Serializable]
        private class MarkerColorsPanel : SettingGroupChangeInGame<Enums.MarkerType, SettingEntry<Color>>
        {
            private const string SettingGroupName = "Marker colors";
            private const string SettingNameAlpha = "Marker alpha";
            
            private readonly SettingEntry<float> alpha = new SettingEntry<float>();
            private readonly Dictionary<Enums.MarkerType, string> headersLookup = new Dictionary<Enums.MarkerType, string>
            {
                { Enums.MarkerType.None, "None color" },
                { Enums.MarkerType.ReadyToCheck, "Ready to check color" },
                { Enums.MarkerType.Checked, "Checked color" },
                { Enums.MarkerType.Path, "Path color" }
            };
            
            public new UISettingGroup Initialize()
            {
                base.Initialize();
                
                List<UISetting> uiSettings = new List<UISetting>();
                foreach (Enums.MarkerType key in keysOrder)
                {
                    uiSettings.Add(valuesLookup[key].Initialize(SettingGroupName, headersLookup[key]));
                }
                uiSettings.Add(alpha.Initialize(SettingGroupName, SettingNameAlpha));
                
                UISettingGroup uiSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISettingGroup>(Enums.UISpawned.SettingGroupEntry);
                uiSettingGroup.Initialize(SettingGroupName, uiSettings);
                
                return uiSettingGroup;
            }
            
            public MarkerColors Setting
            {
                get
                {
                    MarkerColors setting = new MarkerColors();
                    foreach (var entry in valuesLookup)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }
                    setting.Alpha = alpha.Value;

                    return setting;
                }
                set
                {
                    foreach (var entry in valuesLookup)
                    {
                        entry.Value.Value = value.GetValue(entry.Key);
                    }
                    alpha.Value = value.Alpha;

                }
            }
        }
        
        [Serializable]
        private class AlgorithmStagesDelayPanel : SettingGroupChangeInGame<Enums.AlgorithmStageDelay, SettingEntry<float>>
        {
            private const string SettingGroupName = "Algorithm stages delay";

            private readonly Dictionary<Enums.AlgorithmStageDelay, string> headersLookup = new Dictionary<Enums.AlgorithmStageDelay, string>
            {
                { Enums.AlgorithmStageDelay.AfterNewNodeEnqueuing, "After new node enqueuing" },
                { Enums.AlgorithmStageDelay.AfterNodeChecking, "After node checkin" },
                { Enums.AlgorithmStageDelay.AfterCursorPositionChange, "After cursor position change" },
                { Enums.AlgorithmStageDelay.AfterPathNodeSetting, "After path node setting" }
            };
            
            public new UISettingGroup Initialize()
            {
                base.Initialize();
                
                List<UISetting> uiSettings = new List<UISetting>();
                foreach (Enums.AlgorithmStageDelay key in keysOrder)
                {
                    uiSettings.Add(valuesLookup[key].Initialize(SettingGroupName, headersLookup[key]));
                }
                
                UISettingGroup uiSettingGroup = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISettingGroup>(Enums.UISpawned.SettingGroupEntry);
                uiSettingGroup.Initialize(SettingGroupName, uiSettings);
                
                return uiSettingGroup;
            }
            
            public AlgorithmStagesDelay Setting
            {
                get
                {
                    AlgorithmStagesDelay setting = new AlgorithmStagesDelay();
                    foreach (var entry in valuesLookup)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }

                    return setting;
                }
                set
                {
                    foreach (var entry in valuesLookup)
                    {
                        entry.Value.Value = value.GetValue(entry.Key);
                    }
                }
            }
        }
        
        [Serializable]
        private class SettingEntry<T>
        {
            private UISetting uiSetting;
            private T value;
            private string popupHeader;

            public bool ChangedThroughPopup { get; private set; }
            
            public T Value
            {
                get => value;
                set
                {
                    ChangedThroughPopup = false;
                    SetValueInternal(value);
                }
            }

            public UISetting Initialize(string groupName, string name)
            {
                popupHeader = $"{groupName}: {name}";
                uiSetting = AllManagers.Instance.UIManager.UISpawner.CreateObject<UISetting>(Enums.UISpawned.SettingEntry);
                uiSetting.Initialize(name, ButtonAction);
                
                return uiSetting;
            }

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
                        uiSetting.Button.Label = floatValue.ToString(CultureInfo.CurrentCulture);
                        break;

                    case int intValue:
                        uiSetting.Button.Label = intValue.ToString();
                        break;

                    case Color colorValue:
                        uiSetting.Button.Color = colorValue;
                        uiSetting.Button.Label = Utility.ColorToHexString(colorValue, true);
                        break;
                }
            }
        }
    }
}