using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Settings
{
    // Todo: Ideally it would be, if inspector would be filled width settings dependently from enum SettingName. If enum entry would be added, then it would be shown in the inspector of this class.
    [CreateAssetMenu(fileName = "DefaultSettings", menuName = "Default settings/Create new", order = 0)]
    public class DefaultSettingsSO : ScriptableObject
    {
        [Header("Board")]
        [SerializeField, SettingRange(1, 100)] private SettingDefaultNumeric<int> boardWidth;
        [SerializeField, SettingRange(1, 100)] private SettingDefaultNumeric<int> boardLength;
        
        [Header("Tile")]
        [SerializeField, SettingRange(0.1f, 10)] private SettingDefaultNumeric<float> tileLength;
        [SerializeField, SettingRange(0.1f, 10)] private SettingDefaultNumeric<float> tileHeight;
        [SerializeField] private SettingDefault<Color> tileColorDefault;
        [SerializeField] private SettingDefault<Color> tileColorStart;
        [SerializeField] private SettingDefault<Color> tileColorDestination;
        [SerializeField] private SettingDefault<Color> tileColorBlocked;
        [SerializeField, SettingRange(0, 1)] private SettingDefaultNumeric<float> tileColorHighlightValue;
        
        [Header("Marker")]
        [SerializeField] private SettingDefault<Color> markerColorNone;
        [SerializeField] private SettingDefault<Color> markerColorReadyToCheck;
        [SerializeField] private SettingDefault<Color> markerColorChecked;
        [SerializeField] private SettingDefault<Color> markerColorPath;
        [SerializeField, SettingRange(0, 1)] private SettingDefaultNumeric<float> tileColorAlpha;
        
        [Header("Algorithms")]
        [SerializeField, SettingRange(0, 10)] private SettingDefaultNumeric<float> algorithmStageDelayAfterNewNodeEnqueuing;
        [SerializeField, SettingRange(0, 10)] private SettingDefaultNumeric<float> algorithmStageDelayAfterNodeChecking;
        [SerializeField, SettingRange(0, 10)] private SettingDefaultNumeric<float> algorithmStageDelayAfterCursorPositionChange;
        [SerializeField, SettingRange(0, 10)] private SettingDefaultNumeric<float> algorithmStageDelayAfterPathNodeSetting;
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
                InitSetting(algorithmStageDelayAfterPathNodeSetting, Enums.SettingName.AlgorithmStageDelayAfterPathNodeSetting);
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
            [SerializeField] protected T value;
            
            public virtual ISetting Setting => new Setting<T>(value);
        }

        [Serializable]
        private class SettingDefaultNumeric<T> : SettingDefault<T> where T : IComparable
        {
            [SerializeField] private T maxValue;
            [SerializeField] private T minValue;
            
            public override ISetting Setting => new SettingNumeric<T>(value, minValue, maxValue);
        }

        private interface ISettingDefault
        {
            ISetting Setting { get; }
        }

#if UNITY_EDITOR
        [AttributeUsage(AttributeTargets.Field)]
        private class SettingRangeAttribute : PropertyAttribute
        {
            public readonly float min;
            public readonly float max;

            public SettingRangeAttribute(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }
        
        [CustomPropertyDrawer(typeof (SettingRangeAttribute))]
        private class SettingRangeDrawer : PropertyDrawer
        {
            private SerializedProperty maxValue;
            private SerializedProperty minValue;
            private SerializedProperty value;
            
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                maxValue = property.FindPropertyRelative("maxValue");
                minValue = property.FindPropertyRelative("minValue");
                value = property.FindPropertyRelative("value");

                return EditorGUI.GetPropertyHeight(property);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                SettingRangeAttribute attribute = (SettingRangeAttribute) this.attribute;
                if (property.type == typeof(SettingDefaultNumeric<>).Name)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(position, property, label, true);
                    if (!EditorGUI.EndChangeCheck())
                        return;

                    // Todo: It is not a pretty solution, but for now I haven't got better one.
                    if (value.propertyType == SerializedPropertyType.Float)
                    {
                        maxValue.floatValue = Mathf.Max(minValue.floatValue, maxValue.floatValue);
                        minValue.floatValue = Mathf.Min(minValue.floatValue, maxValue.floatValue);
                    
                        maxValue.floatValue = Mathf.Min(maxValue.floatValue, attribute.max);
                        minValue.floatValue = Mathf.Max(minValue.floatValue, attribute.min);
                    
                        maxValue.floatValue = Mathf.Clamp(maxValue.floatValue, attribute.min, attribute.max);
                        minValue.floatValue = Mathf.Clamp(minValue.floatValue, attribute.min, attribute.max);

                        value.floatValue = Mathf.Clamp(value.floatValue, minValue.floatValue, maxValue.floatValue);
                    }
                    else if (value.propertyType == SerializedPropertyType.Integer)
                    {
                        maxValue.intValue = Mathf.Max(minValue.intValue, maxValue.intValue);
                        minValue.intValue = Mathf.Min(minValue.intValue, maxValue.intValue);
                    
                        maxValue.intValue = Mathf.Min(maxValue.intValue, (int)attribute.max);
                        minValue.intValue = Mathf.Max(minValue.intValue, (int)attribute.min);
                    
                        maxValue.intValue = Mathf.Clamp(maxValue.intValue, (int)attribute.min, (int)attribute.max);
                        minValue.intValue = Mathf.Clamp(minValue.intValue, (int)attribute.min, (int)attribute.max);

                        value.intValue = Mathf.Clamp(value.intValue, minValue.intValue, maxValue.intValue);
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Use Range with SettingDefaultNumeric.");
                }
            }
        }
#endif
    }
}