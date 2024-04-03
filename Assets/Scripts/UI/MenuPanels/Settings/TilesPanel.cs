using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace UI.MenuPanels.Settings
{
    [Serializable]
    public class TilesPanel : SettingGroupPanel<TilesPanel.SettingGroupName>
    {
        [SerializeField] private Size size;
        [SerializeField] private TileDimensionsPanel tileDimensions;
        [SerializeField] private TileColorsPanel tileColors;
        [SerializeField] private MarkerColorsPanel markerColors;

        public Vector2Int SizeSetting
        {
            get => size.Setting;
            set => size.Setting = value;
        }

        public TileDimensions TileDimensionsSetting
        {
            get => tileDimensions.Setting;
            set => tileDimensions.Setting = value;
        }

        public TileColors TileColorsSetting
        {
            get => tileColors.Setting;
            set => tileColors.Setting = value;
        }

        public MarkerColors MarkerColorsSetting
        {
            get => markerColors.Setting;
            set => markerColors.Setting = value;
        }

        public override void BuildLookup()
        {
            settingGroupsLookup ??= new Dictionary<SettingGroupName, SettingGroupInGame>()
            {
                { size.Name, size },
                { tileDimensions.Name, tileDimensions },
                { tileColors.Name, tileColors },
                { markerColors.Name, markerColors }
            };
            
            base.BuildLookup();
        }
        
        [Serializable]
        private class Size : SettingGroupInGame<SettingNameSize, SettingGroupName>
        {
            private const string SettingGroupName = "Board size";

            private SettingEntry<int> width = new SettingEntry<int>(SettingGroupName, "Width");
            private SettingEntry<int> length = new SettingEntry<int>(SettingGroupName, "Length");

            public override SettingGroupName Name => TilesPanel.SettingGroupName.Size;

            public Vector2Int Setting
            {
                get => new Vector2Int(width.Value, length.Value);
                set
                {
                    width.Value = value.x;
                    length.Value = value.y;
                }
            }

            public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

            public override void BuildLookup()
            {
                settingsLookup = new Dictionary<SettingNameSize, ISettingEntry>
                {
                    { SettingNameSize.Width, width },
                    { SettingNameSize.Length, length }
                };
            }
        }
        
        private enum SettingNameSize
        {
            Width,
            Length
        }

        [Serializable]
        private class TileDimensionsPanel : SettingGroupInGame<SettingNameTileDimensions, SettingGroupName>
        {
            private const string SettingGroupName = "Tile dimensions";

            private SettingEntry<float> length = new SettingEntry<float>(SettingGroupName, "Length");
            private SettingEntry<float> height = new SettingEntry<float>(SettingGroupName, "Height");
            
            public override SettingGroupName Name => TilesPanel.SettingGroupName.TileDimensions;
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

            public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

            public override void BuildLookup()
            {
                settingsLookup = new Dictionary<SettingNameTileDimensions, ISettingEntry>
                {
                    { SettingNameTileDimensions.Length, length },
                    { SettingNameTileDimensions.Height, height }
                };
            }
        }
        
        private enum SettingNameTileDimensions
        {
            Length,
            Height
        }

        [Serializable]
        private class TileColorsPanel : SettingGroupInGame<SettingNameTileColors, SettingGroupName>
        {
            private const string SettingGroupName = "Tile colors";
            
            private Dictionary<Enums.TileType, SettingEntry<Color>> colors = new Dictionary<Enums.TileType, SettingEntry<Color>>
            {
                { Enums.TileType.Default, new SettingEntry<Color>(SettingGroupName, "Default") },
                { Enums.TileType.Start, new SettingEntry<Color>(SettingGroupName, "Start") },
                { Enums.TileType.Destination, new SettingEntry<Color>(SettingGroupName, "Destination") },
                { Enums.TileType.Blocked, new SettingEntry<Color>(SettingGroupName, "Blocked") }
            };
            private readonly SettingEntry<float> highlight = new SettingEntry<float>(SettingGroupName, "Cursor highlight");
            
            public override SettingGroupName Name => TilesPanel.SettingGroupName.TileColors;
            public TileColors Setting
            {
                get
                {
                    TileColors setting = new TileColors();
                    foreach (var entry in colors)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }
                    setting.HighlightValue = highlight.Value;
            
                    return setting;
                }
                set
                {
                    foreach (var entry in colors)
                    {
                        entry.Value.Value = value.GetValue(entry.Key);
                    }
                    highlight.Value = value.HighlightValue;
            
                }
            }
            
            public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

            public override void BuildLookup()
            {
                if (settingsLookup != null) return;

                settingsLookup = new Dictionary<SettingNameTileColors, ISettingEntry>();
                foreach (var color in colors)
                {
                    settingsLookup.Add((SettingNameTileColors)color.Key, color.Value);
                }
                settingsLookup.Add(SettingNameTileColors.HighlightValue, highlight);
            }
        }
        
        private enum SettingNameTileColors
        {
            Default = 0,
            Start = 1,
            Destination = 2,
            Blocked = 3,
            HighlightValue = 4
        }
        
        [Serializable]
        private class MarkerColorsPanel : SettingGroupInGame<SettingNameMarkerColors, SettingGroupName>
        {
            private const string SettingGroupName = "Marker colors";
            
            private Dictionary<Enums.MarkerType, SettingEntry<Color>> colors = new Dictionary<Enums.MarkerType, SettingEntry<Color>>
            {
                { Enums.MarkerType.None, new SettingEntry<Color>(SettingGroupName, "None") },
                { Enums.MarkerType.ReadyToCheck, new SettingEntry<Color>(SettingGroupName, "Ready to check") },
                { Enums.MarkerType.Checked, new SettingEntry<Color>(SettingGroupName, "Checked") },
                { Enums.MarkerType.Path, new SettingEntry<Color>(SettingGroupName, "Path") },
            };
            private readonly SettingEntry<float> alpha = new SettingEntry<float>(SettingGroupName, "Opacity");
            
            public override SettingGroupName Name => TilesPanel.SettingGroupName.MarkerColors;
            public MarkerColors Setting
            {
                get
                {
                    MarkerColors setting = new MarkerColors();
                    foreach (var entry in colors)
                    {
                        setting.SetValue(entry.Key, entry.Value.Value);
                    }
                    setting.Alpha = alpha.Value;

                    return setting;
                }
                set
                {
                    foreach (var entry in colors)
                    {
                        entry.Value.Value = value.GetValue(entry.Key);
                    }
                    alpha.Value = value.Alpha;

                }
            }

            public override void InitUI(Transform uiParent) => InitUI(uiParent, SettingGroupName);

            public override void BuildLookup()
            {
                if (settingsLookup != null) return;
                
                settingsLookup = new Dictionary<SettingNameMarkerColors, ISettingEntry>();
                foreach (var color in colors)
                {
                    settingsLookup.Add((SettingNameMarkerColors)color.Key, color.Value);
                }
                settingsLookup.Add(SettingNameMarkerColors.Alpha, alpha);
            }
        }
        
        private enum SettingNameMarkerColors
        {
            None = 0,
            ReadyToCheck = 1,
            Destination = 2,
            Blocked = 3,
            Alpha = 4
        }
        
        public enum SettingGroupName
        {
            Size,
            TileDimensions,
            TileColors,
            MarkerColors
        }
    }
}