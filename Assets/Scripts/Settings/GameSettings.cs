using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class GameSettings
    {
        [SerializeField] private DefaultSettingsSO defaultSettingsSO;

        private Dictionary<Enums.SettingName, ISetting> settings;
    
        private readonly Dictionary<Enums.PermittedDirection, Vector2Int> permittedDirectionsLookup = new()
        {
            { Enums.PermittedDirection.Up, Vector2Int.up }, 
            { Enums.PermittedDirection.Down, Vector2Int.down }, 
            { Enums.PermittedDirection.Left, Vector2Int.left }, 
            { Enums.PermittedDirection.Right, Vector2Int.right }, 
            { Enums.PermittedDirection.UpRight, new Vector2Int(1, 1) }, 
            { Enums.PermittedDirection.DownRight, new Vector2Int(1, -1) }, 
            { Enums.PermittedDirection.DownLeft, new Vector2Int(-1, -1) }, 
            { Enums.PermittedDirection.UpLeft, new Vector2Int(-1, 1) }
        };
        
        public ISetting GetSetting(Enums.SettingName name)
        {
            if (settings == null)
            {
                LoadDefault();
            }

            return settings![name];
        }
        
        public int BoardWidth => GetSetting<int>(Enums.SettingName.BoardWidth);
        public int BoardLength => GetSetting<int>(Enums.SettingName.BoardLength);
        public float TileLength => GetSetting<float>(Enums.SettingName.TileDimensionLength);
        public float TileHeight => GetSetting<float>(Enums.SettingName.TileDimensionHeight);
        public float TileHighlight => GetSetting<float>(Enums.SettingName.TileColorHighlightValue);
        public float MarkerAlpha => GetSetting<float>(Enums.SettingName.MarkerColorAlpha);
        public Enums.Language Language => GetSetting<Language>(Enums.SettingName.Language).Enum;

        public Color GetTileColor(Enums.TileType tileType)
        {
            Enums.SettingName settingName = tileType switch
            {
                Enums.TileType.Default => Enums.SettingName.TileColorDefault,
                Enums.TileType.Start => Enums.SettingName.TileColorStart,
                Enums.TileType.Destination => Enums.SettingName.TileColorDestination,
                Enums.TileType.Blocked => Enums.SettingName.TileColorBlocked,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null)
            };
            
            return GetSetting<Color>(settingName);
        }
        
        public Color GetMarkerColor(Enums.MarkerType markerType)
        {
            Enums.SettingName settingName = markerType switch
            {
                Enums.MarkerType.None => Enums.SettingName.MarkerColorNone,
                Enums.MarkerType.Closed => Enums.SettingName.MarkerColorClosed,
                Enums.MarkerType.Opened => Enums.SettingName.MarkerColorOpened,
                Enums.MarkerType.Path => Enums.SettingName.MarkerColorPath,
                _ => throw new ArgumentOutOfRangeException(nameof(markerType), markerType, null)
            };
            
            return GetSetting<Color>(settingName);
        }
        
        public float GetDelay(Enums.AlgorithmStageDelay delay)
        {
            Enums.SettingName settingName = delay switch
            {
                Enums.AlgorithmStageDelay.AfterNewNodeEnqueuing => Enums.SettingName.AlgorithmStageDelayAfterNewNodeEnqueuing,
                Enums.AlgorithmStageDelay.AfterNodeChecking => Enums.SettingName.AlgorithmStageDelayAfterNodeChecking,
                Enums.AlgorithmStageDelay.AfterCursorPositionChange => Enums.SettingName.AlgorithmStageDelayAfterCursorPositionChange,
                Enums.AlgorithmStageDelay.AfterPathNodeSetting => Enums.SettingName.AlgorithmStageDelayAfterPathNodeSetting,
                _ => throw new ArgumentOutOfRangeException(nameof(delay), delay, null)
            };
            
            return GetSetting<float>(settingName);
        }

        public Vector2Int[] GetPermittedDirections()
        {
            PermittedDirection[] permittedDirections = GetSetting<PermittedDirection[]>(Enums.SettingName.PermittedDirections);
            int length = permittedDirections.Length;
            var result = new Vector2Int[length];
            int j = 0;
            for (int i = 0; i < length; i++)
            {
                PermittedDirection direction = permittedDirections[i];
                
                if (direction.Enabled)
                {
                    result[j] = permittedDirectionsLookup[direction.direction];
                    j++;
                }
            }
    
            return result;
        }
        
        public void LoadDefault() => settings = defaultSettingsSO.Settings;
        
        public object CaptureState()
        {
            Dictionary<Enums.SettingName, object> result = new Dictionary<Enums.SettingName, object>();
            foreach (KeyValuePair<Enums.SettingName, ISetting> entry in settings)
            {
                result.Add(entry.Key, entry.Value.SerializableValue);
            }
            
            return result;
        }

        public void RestoreState(object state)
        {
            LoadDefault();
            Dictionary<Enums.SettingName, object> data = (Dictionary<Enums.SettingName, object>)state;

            foreach (KeyValuePair<Enums.SettingName, ISetting> entry in settings)
            {
                entry.Value.SerializableValue = data[entry.Key];
            }
        }

        private T GetSetting<T>(Enums.SettingName name)
        {
            try
            {
                return ((Setting<T>)settings[name]).Value;
            }
            catch (InvalidCastException)
            {
                Debug.LogError($"Setting: {name} is not type of {typeof(T)}.");
                throw;
            }
        }
    }
}