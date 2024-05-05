using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public static class Utility
    {
#region UI

        public static void RefreshLayoutGroupsImmediate(RectTransform root)
        {
            foreach (LayoutGroup layoutGroup in root.GetComponentsInChildren<LayoutGroup>(true))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        }
    
        public static string ColorToHexString(Color value, bool startWithHash)
        {
            int red = Convert.ToInt32(value.r * 255);
            int green = Convert.ToInt32(value.g * 255);
            int blue = Convert.ToInt32(value.b * 255);

            string result = "";
            if (startWithHash)
            {
                result += "#";
            }
            result += $"{red:X2}{green:X2}{blue:X2}";
        
            return result;
        }
    
        public static T CalculateNextSelectableElement<T>(int beginIndex, Enums.Direction direction, int elementsCount, List<T> elements) where T : class, ISelectableElement
        {
            T result = null;
            int intDirection = (int)direction;
            int nextIndex = beginIndex;
            int j = 0;
            do
            {
                nextIndex = CorrectIndex(nextIndex + intDirection, elementsCount);
                
                if (elements[nextIndex].IsInteractable())
                {
                    result = elements[nextIndex];
                    break;
                }
                
                j++;
            } while (j < elementsCount);

            return result;
        }

        private static int CorrectIndex(int index, int buttonsCount)
        {
            index = (index + buttonsCount) % buttonsCount;
            return index;
        }

#endregion

#region Settings

        public static object ConvertToSerializableValue<T>(T value)
        {
            switch (value)
            {
                case Color color:
                    return new SerializableColor(color);
                default:
                    Debug.LogError($"Type: {typeof(T)} is not supported for conversion.");
                    return null;
            }
        }
        
        public static T ConvertFromSerializableValue<T>(object value)
        {
            switch (value)
            {
                case ISerializableValue<T> serializableValue:
                    return serializableValue.GetValue();
                default:
                    Debug.LogError($"Type: {typeof(T)} is not supported for conversion.");
                    return default;
            }
        }

#endregion
        
        public static float EaseOutCubic(float start, float end, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
        
            if (start > end)
            {
                ratio = 1 - ratio;
                return CubicBendDownFunc(end, start, ratio);
            }
            else
            {
                ratio--;
                return CubicBendUpFunc(start, end, ratio);
            }
        }
    
        public static float EaseInCubic(float start, float end, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
        
            if (start > end)
            {
                ratio = 1 - ratio;
                ratio--;
                return CubicBendUpFunc(end, start, ratio);
            }
            else
            {
                return CubicBendDownFunc(start, end, ratio);
            }
        }

        private static float CubicBendUpFunc(float min, float max, float arg)
        {
            max -= min;
            return max * (arg * arg * arg + 1) + min;
        }
    
        private static float CubicBendDownFunc(float min, float max, float arg)
        {
            max -= min;
            return max * (arg * arg * arg) + min;
        }
    }
}