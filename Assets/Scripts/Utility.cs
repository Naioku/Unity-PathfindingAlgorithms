using System.Collections.Generic;
using UI.Buttons;

public static class Utility
{
    #region UI

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