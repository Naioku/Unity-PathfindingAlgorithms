using System;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "DefaultSettings", menuName = "Default settings/Create new", order = 0)]
    public class DefaultSettingsSO : ScriptableObject
    {
        [Header("Tile")]
        [SerializeField] private Vector2Int size;
        [SerializeField] private TileDimensionsDefault tileDimensions;
        [SerializeField] private TileColorsDefault tileColors;
        [SerializeField] private MarkerColorsDefault markerColors;
    
        [Header("Miscellaneous")]
        [SerializeField] private AlgorithmStagesDelayDefault algorithmStagesDelay;
        [SerializeField] private Enums.PermittedDirection[] permittedDirections;

        public GameSettings Settings =>
            new GameSettings
            (
                size,
                tileDimensions.Setting,
                tileColors.Setting,
                markerColors.Setting,
                algorithmStagesDelay.Setting,
                permittedDirections
            );

        public void Initialize()
        {
            tileColors.Initialize();
            markerColors.Initialize();
            algorithmStagesDelay.Initialize();
        }

        private void OnValidate()
        {
            algorithmStagesDelay.ManageForcingGeneralValue();
        }
    }

    [Serializable]
    public class TileDimensionsDefault
    {
        [SerializeField] private float Length = 1;
        [SerializeField] private float Height = 0.5f;

        public TileDimensions Setting => new TileDimensions
        {
            Length = Length,
            Height = Height
        };
    }

    // Todo: Change all these structs and made serialized dictionaries from them.
    [Serializable]
    public class TileColorsDefault : SettingChangeDefault<Enums.TileType, Color>
    {
        public float HighlightValue = 0.2f;
        public TileColors Setting
        {
            get
            {
                TileColors setting = new TileColors();
                SetLookupValues(setting);
                setting.HighlightValue = HighlightValue;

                return setting;
            }
        }
    }

    [Serializable]
    public class MarkerColorsDefault : SettingChangeDefault<Enums.MarkerType, Color>
    {
        public float Alpha;
        public MarkerColors Setting
        {
            get
            {
                MarkerColors setting = new MarkerColors();
                SetLookupValues(setting);
                setting.Alpha = Alpha;

                return setting;
            }
        }
    }

    [Serializable]
    public class AlgorithmStagesDelayDefault : SettingChangeDefault<Enums.AlgorithmStageDelay, float>
    {
        [Header("General value")]
        [SerializeField] private bool forceGeneralValue;
        [SerializeField] private float value;
    
        public AlgorithmStagesDelay Setting
        {
            get
            {
                AlgorithmStagesDelay setting = new AlgorithmStagesDelay();
                SetLookupValues(setting);

                return setting;
            }
        }
    
        public void ManageForcingGeneralValue()
        {
            if (!forceGeneralValue) return;
        
            foreach (Entry entry in entries)
            {
                entry.Value = value;
            }
        }
    }
}