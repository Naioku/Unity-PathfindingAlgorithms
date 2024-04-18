using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    // Todo: Ideally it would be, if inspector would be filled width settings dependently from enum SettingName. If enum entry would be added, then it would be shown in the inspector of this class.
    [CreateAssetMenu(fileName = "DefaultSettings", menuName = "Default settings/Create new", order = 0)]
    public class DefaultSettingsSO : ScriptableObject
    {
        [Header("Board")]
        [SerializeField] private SettingDefault<int> boardWidth;
        [SerializeField] private SettingDefault<int> boardLength;
        
        [Header("Tile")]
        [SerializeField] private SettingDefault<float> tileLength;
        [SerializeField] private SettingDefault<float> tileHeight;
        [SerializeField] private SettingDefault<Color> tileColorDefault;
        [SerializeField] private SettingDefault<Color> tileColorStart;
        [SerializeField] private SettingDefault<Color> tileColorDestination;
        [SerializeField] private SettingDefault<Color> tileColorBlocked;
        [SerializeField] private SettingDefault<float> tileColorHighlightValue;
        
        [Header("Marker")]
        [SerializeField] private SettingDefault<Color> markerColorNone;
        [SerializeField] private SettingDefault<Color> markerColorReadyToCheck;
        [SerializeField] private SettingDefault<Color> markerColorChecked;
        [SerializeField] private SettingDefault<Color> markerColorPath;
        [SerializeField] private SettingDefault<float> tileColorAlpha;
        
        [Header("Algorithms")]
        [SerializeField] private SettingDefault<float> algorithmStageDelayAfterNewNodeEnqueuing;
        [SerializeField] private SettingDefault<float> algorithmStageDelayAfterNodeChecking;
        [SerializeField] private SettingDefault<float> algorithmStageDelayAfterCursorPositionChange;
        [SerializeField] private SettingDefault<float> algorithmStageDelayAfterPathNodeSetting;
        [SerializeField] private SettingDefault<Enums.PermittedDirection[]> permittedDirectionsSetting;

        public Dictionary<Enums.SettingName, ISetting> Settings
        {
            get
            {
                Dictionary<Enums.SettingName, ISetting> result = new Dictionary<Enums.SettingName, ISetting>();
                
                InitSetting(boardWidth, Enums.SettingName.BoardWidth);
                InitSetting(boardLength, Enums.SettingName.BoardLength);
                InitSetting(tileLength, Enums.SettingName.TileDimensionLength);
                InitSetting(tileHeight, Enums.SettingName.TileDimensionHeight);
                InitSetting(tileColorDefault, Enums.SettingName.TileColorDefault);
                InitSetting(tileColorStart, Enums.SettingName.TileColorStart);
                InitSetting(tileColorDestination, Enums.SettingName.TileColorDestination);
                InitSetting(tileColorBlocked, Enums.SettingName.TileColorBlocked);
                InitSetting(tileColorHighlightValue, Enums.SettingName.TileColorHighlightValue);
                InitSetting(markerColorNone, Enums.SettingName.MarkerColorNone);
                InitSetting(markerColorReadyToCheck, Enums.SettingName.MarkerColorReadyToCheck);
                InitSetting(markerColorChecked, Enums.SettingName.MarkerColorChecked);
                InitSetting(markerColorPath, Enums.SettingName.MarkerColorPath);
                InitSetting(tileColorAlpha, Enums.SettingName.MarkerColorAlpha);
                InitSetting(algorithmStageDelayAfterNewNodeEnqueuing, Enums.SettingName.AlgorithmStageDelayAfterNewNodeEnqueuing);
                InitSetting(algorithmStageDelayAfterNodeChecking, Enums.SettingName.AlgorithmStageDelayAfterNodeChecking);
                InitSetting(algorithmStageDelayAfterCursorPositionChange, Enums.SettingName.AlgorithmStageDelayAfterCursorPositionChange);
                InitSetting(algorithmStageDelayAfterPathNodeSetting, Enums.SettingName.AlgorithmStageDelayAfterPathNode);
                InitSetting(permittedDirectionsSetting, Enums.SettingName.PermittedDirections);

                return result;

                void InitSetting(ISettingDefault setting, Enums.SettingName name)
                {
                    result.Add(name, setting.Setting);
                }
            }
        }

        [Serializable]
        private class SettingDefault<T> : ISettingDefault
        {
            [SerializeField] private T value;
            
            public Enums.SettingName Name { get; set; }
            public ISetting Setting => new Setting<T>(Name, value);

        }

        private interface ISettingDefault
        {
            Enums.SettingName Name { get; set; }
            ISetting Setting { get; }
        }
    }
}