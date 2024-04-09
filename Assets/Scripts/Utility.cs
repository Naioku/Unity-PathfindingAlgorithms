using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

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
}