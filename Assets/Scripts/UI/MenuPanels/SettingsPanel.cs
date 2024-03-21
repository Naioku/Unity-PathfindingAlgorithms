using System;
using System.Globalization;
using Settings;
using TMPro;
using UnityEngine;
using Button = UI.Buttons.Button;

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
        private Enums.PermittedDirection[] permittedDirections = new []
        {
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
            size.Setting = gameSettings.Size;
            tileDimensions.Setting = gameSettings.TileDimensions;
            tileColors.Setting = gameSettings.TileColors;
            markerColors.Setting = gameSettings.MarkerColors;
            stagesDelay.Setting = gameSettings.AlgorithmStagesDelay;
        }

        private void Save()
        {
            Vector2Int size = this.size.Setting;

            gameSettings = new GameSettings(
                size,
                tileDimensions.Setting,
                tileColors.Setting,
                markerColors.Setting,
                stagesDelay.Setting,
                permittedDirections);
            
            onSave.Invoke(gameSettings);
        }
        
        public override void SelectDefaultButton() => backButton.Select();

        private static string ColorToHexString(Color value)
        {
            int red = Convert.ToInt32(value.r * 255);
            int green = Convert.ToInt32(value.g * 255);
            int blue = Convert.ToInt32(value.b * 255);
            
            return $"#{red:X}{green:X}{blue:X}";
        }
        
        private static Color HexStringToColor(string input)
        {
            if (input.StartsWith("#"))
            {
                input = input.Substring(1);
            }

            float red = (float)int.Parse(input.Substring(0, 2), NumberStyles.HexNumber) / 255;
            float green = (float)int.Parse(input.Substring(2, 2), NumberStyles.HexNumber) / 255;
            float blue = (float)int.Parse(input.Substring(4, 2), NumberStyles.HexNumber) / 255;
            return new Color(red, green, blue);
        }

        [Serializable]
        private struct Size
        {
            [SerializeField] private TMP_InputField widthField;
            [SerializeField] private TMP_InputField lengthField;
            
            public Vector2Int Setting
            {
                get => new Vector2Int(int.Parse(widthField.text), int.Parse(lengthField.text));
                set
                {
                    widthField.text = value.x.ToString();
                    lengthField.text = value.y.ToString();
                }
            }
        }

        [Serializable]
        private struct TileDimensionsPanel
        {
            [SerializeField] private TMP_InputField lengthField;
            [SerializeField] private TMP_InputField heightField;
            
            public TileDimensions Setting
            {
                get =>
                    new TileDimensions
                    {
                        Length = float.Parse(lengthField.text),
                        Height = float.Parse(heightField.text)
                    };
                set
                {
                    lengthField.text = value.Length.ToString(CultureInfo.CurrentCulture);
                    heightField.text = value.Height.ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        [Serializable]
        private class TileColorsPanel : SettingChange<Enums.TileType, TMP_InputField>
        {
            [SerializeField] private TMP_InputField highlightField;
            
            public TileColors Setting
            {
                get
                {
                    TileColors setting = new TileColors();
                    foreach (Entry entry in entries)
                    {
                        setting.SetValue(entry.Key, HexStringToColor(entry.Value.text));
                    }
                    setting.HighlightValue = float.Parse(highlightField.text);

                    return setting;
                }
                set
                {
                    foreach (Entry entry in entries)
                    {
                       entry.Value.text = ColorToHexString(value.GetValue(entry.Key));
                    }
                    highlightField.text = value.HighlightValue.ToString(CultureInfo.CurrentCulture);

                }
            }
        }
        
        [Serializable]
        private class MarkerColorsPanel : SettingChange<Enums.MarkerType, TMP_InputField>
        {
            [SerializeField] private TMP_InputField alpha;
            
            public MarkerColors Setting
            {
                get
                {
                    MarkerColors setting = new MarkerColors();
                    foreach (Entry entry in entries)
                    {
                        setting.SetValue(entry.Key, HexStringToColor(entry.Value.text));
                    }
                    setting.Alpha = float.Parse(alpha.text);

                    return setting;
                }
                set
                {
                    foreach (Entry entry in entries)
                    {
                        entry.Value.text = ColorToHexString(value.GetValue(entry.Key));
                    }
                    alpha.text = value.Alpha.ToString(CultureInfo.CurrentCulture);
                }
            }
        }
        
        [Serializable]
        private class AlgorithmStagesDelayPanel : SettingChange<Enums.AlgorithmStageDelay, TMP_InputField>
        {
            public AlgorithmStagesDelay Setting
            {
                get
                {
                    AlgorithmStagesDelay setting = new AlgorithmStagesDelay();
                    foreach (Entry entry in entries)
                    {
                        setting.SetValue(entry.Key, float.Parse(entry.Value.text));
                    }

                    return setting;
                }
                set
                {
                    foreach (Entry entry in entries)
                    {
                        entry.Value.text = value.GetValue(entry.Key).ToString(CultureInfo.CurrentCulture);
                    }
                }
            }
        }
    }
}