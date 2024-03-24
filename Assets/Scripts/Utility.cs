using System;
using System.Collections.Generic;
using UI.Buttons;
using UnityEngine;

public static class Utility
{
    #region UI

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
    
    public static Button CalculateButtonForNavigation<T>(int beginIndex, Enums.Direction direction, int buttonsCount, List<T> buttons) where T : Button
    {
        Button result = null;
        int intDirection = (int)direction;
        int nextIndex = beginIndex;
        int j = 0;
        do
        {
            nextIndex = CorrectIndex(nextIndex + intDirection, buttonsCount);
                
            if (buttons[nextIndex].IsInteractable())
            {
                result = buttons[nextIndex];
                break;
            }
                
            j++;
        } while (j < buttonsCount);

        return result;
    }

    private static int CorrectIndex(int index, int buttonsCount)
    {
        index = (index + buttonsCount) % buttonsCount;
        return index;
    }

    #endregion
}