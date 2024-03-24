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
        [SerializeField] private Button saveButton;
        [SerializeField] private Size size;
        [SerializeField] private TileDimensionsPanel tileDimensions;
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

        private Action<GameSettings> onSave;
        private GameSettings gameSettings;
        
        public void Initialize(Action onBack, Action<GameSettings> onSave)
        {
            base.Initialize(onBack);
            this.onSave = onSave;
            gameSettings = AllManagers.Instance.GameManager.GameSettings;
            saveButton.OnPressAction += Save;
            InitInputValues();
        }

        private void InitInputValues()
        {
            size.Initialize();
            tileDimensions.Initialize();
            tileColors.Initialize();
            markerColors.Initialize();
            stagesDelay.Initialize();
            
            size.Setting = gameSettings.Size;
            tileDimensions.Setting = gameSettings.TileDimensions;
            tileColors.Setting = gameSettings.TileColors;
            markerColors.Setting = gameSettings.MarkerColors;
            stagesDelay.Setting = gameSettings.AlgorithmStagesDelay;
        }

        private void Save()
        {
            gameSettings = new GameSettings(
                size.Setting,
                tileDimensions.Setting,
                tileColors.Setting,
                markerColors.Setting,
                stagesDelay.Setting,
                permittedDirections);

            onSave.Invoke(gameSettings);
        }
        
        public override void SelectDefaultButton() => backButton.Select();

        [Serializable]
        private class Size
        {
            private const string PopupHeaderWidth = "Board width";
            private const string PopupHeaderLength = "Board length";
            
            [SerializeField] private SettingEntry<int> width;
            [SerializeField] private SettingEntry<int> length;

            public void Initialize()
            {
                width.Initialize(PopupHeaderWidth);
                length.Initialize(PopupHeaderLength);
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
        }

        [Serializable]
        private class TileDimensionsPanel
        {
            private const string PopupHeaderLength = "Tile length";
            private const string PopupHeaderHeight = "Tile height";
            
            [SerializeField] private SettingEntry<float> length;
            [SerializeField] private SettingEntry<float> height;

            public void Initialize()
            {
                length.Initialize(PopupHeaderLength);
                height.Initialize(PopupHeaderHeight);
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
        }

        [Serializable]
        private class TileColorsPanel : SettingChange<Enums.TileType, SettingEntry<Color>>
        {
            private const string PopupHeaderHighlight = "Cursor highlight";
            
            [SerializeField] private SettingEntry<float> highlight;

            private readonly Dictionary<Enums.TileType, string> headersLookup = new Dictionary<Enums.TileType, string>
            {
                { Enums.TileType.Default, "Default color" },
                { Enums.TileType.Start, "Start color" },
                { Enums.TileType.Destination, "Destination color" },
                { Enums.TileType.Blocked, "Blocked color" }
            };
            
            public override void Initialize()
            {
                base.Initialize();
                foreach (Entry entry in entries)
                {
                    entry.Value.Initialize(headersLookup[entry.Key]);
                }
                highlight.Initialize(PopupHeaderHighlight);
            }
            
            public TileColors Setting
            {
                get
                {
                    TileColors setting = new TileColors();
                    foreach (Entry entry in entries)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }
                    setting.HighlightValue = highlight.Value;

                    return setting;
                }
                set
                {
                    foreach (Entry entry in entries)
                    {
                       entry.Value.Value = value.GetValue(entry.Key);
                    }
                    highlight.Value = value.HighlightValue;

                }
            }
        }
        
        [Serializable]
        private class MarkerColorsPanel : SettingChange<Enums.MarkerType, SettingEntry<Color>>
        {
            private const string PopupHeaderAlpha = "Marker alpha";
            
            [SerializeField] private SettingEntry<float> alpha;

            private readonly Dictionary<Enums.MarkerType, string> headersLookup = new Dictionary<Enums.MarkerType, string>
            {
                { Enums.MarkerType.None, "None color" },
                { Enums.MarkerType.ReadyToCheck, "Ready to check color" },
                { Enums.MarkerType.Checked, "Checked color" },
                { Enums.MarkerType.Path, "Path color" }
            };
            
            public override void Initialize()
            {
                base.Initialize();
                foreach (Entry entry in entries)
                {
                    entry.Value.Initialize(headersLookup[entry.Key]);
                }
                alpha.Initialize(PopupHeaderAlpha);
            }
            
            public MarkerColors Setting
            {
                get
                {
                    MarkerColors setting = new MarkerColors();
                    foreach (Entry entry in entries)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }
                    setting.Alpha = alpha.Value;

                    return setting;
                }
                set
                {
                    foreach (Entry entry in entries)
                    {
                        entry.Value.Value = value.GetValue(entry.Key);
                    }
                    alpha.Value = value.Alpha;

                }
            }
        }
        
        [Serializable]
        private class AlgorithmStagesDelayPanel : SettingChange<Enums.AlgorithmStageDelay, SettingEntry<float>>
        {
            private readonly Dictionary<Enums.AlgorithmStageDelay, string> headersLookup = new Dictionary<Enums.AlgorithmStageDelay, string>
            {
                { Enums.AlgorithmStageDelay.AfterNewNodeEnqueuing, "Delay after new node enqueuing" },
                { Enums.AlgorithmStageDelay.AfterNodeChecking, "Delay after node checkin" },
                { Enums.AlgorithmStageDelay.AfterCursorPositionChange, "Delay after cursor position change" },
                { Enums.AlgorithmStageDelay.AfterPathNodeSetting, "Delay after path node setting" }
            };
            
            public override void Initialize()
            {
                base.Initialize();
                foreach (Entry entry in entries)
                {
                    entry.Value.Initialize(headersLookup[entry.Key]);
                }
            }
            
            public AlgorithmStagesDelay Setting
            {
                get
                {
                    AlgorithmStagesDelay setting = new AlgorithmStagesDelay();
                    foreach (Entry entry in entries)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }

                    return setting;
                }
                set
                {
                    foreach (Entry entry in entries)
                    {
                        entry.Value.Value = value.GetValue(entry.Key);
                    }

                }
            }
        }
        
        [Serializable]
        private class SettingEntry<T>
        {
            [SerializeField] private Button button;
            
            private T value;
            private string popupHeader;

            public T Value
            {
                get => value;
                set
                {
                    this.value = value;
                    switch (value)
                    {
                        case float floatValue:
                            button.Label = floatValue.ToString(CultureInfo.CurrentCulture);
                            break;
                        
                        case int intValue:
                            button.Label = intValue.ToString();
                            break;
                        
                        case Color colorValue:
                            button.Color = colorValue;
                            button.Label = Utility.ColorToHexString(colorValue, true);
                            break;
                    }
                }
            }
            
            public void Initialize(string popupHeader)
            {
                this.popupHeader = popupHeader;
                button.OnPressAction += ButtonAction;
            }

            private void ButtonAction() => AllManagers.Instance.UIManager.OpenPopupInput
            (
                popupHeader,
                value,
                OnClosePanel
            );
            
            private void OnClosePanel(T value) => Value = value;
        }
    }
}