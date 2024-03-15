using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.HUDPanels
{
    public abstract class BaseHUDController : MonoBehaviour
    {
        [SerializeField] private List<Button> buttons;

        public void Initialize(
            List<ButtonData> buttonDataList)
        {
            if (buttonDataList.Count != buttons.Count)
            {
                Debug.LogError($"{name}: Action count does not match the button count.");
                return;
            }

            for (var i = 0; i < buttons.Count; i++)
            {
                var buttonData = buttonDataList[i];
                Button button = buttons[i];
                
                button.OnPressAction += buttonData.Action;
                button.Label = buttonData.Label;

                int nextIndex = i + 1;
                int previousIndex = i - 1;

                if (nextIndex > buttons.Count - 1)
                {
                    nextIndex = 0;
                }

                if (previousIndex < 0)
                {
                    previousIndex = buttons.Count - 1;
                }
                
                button.SetNavigation(onUp: buttons[previousIndex], onDown: buttons[nextIndex]);
            }
        }

        public void Deinitialize()
        {
            foreach (Button button in buttons)
            {
                button.ResetObj();
            }
        }
        
        private void Start() => Hide();

        public virtual void Show()
        {
            gameObject.SetActive(true);
            SelectButton(0);
        }

        public void Hide() => gameObject.SetActive(false);

        public void SelectButton(int buttonIndex) => buttons[buttonIndex].Select();

        public struct ButtonData
        {
            public Action Action;
            public string Label;
        }
    }
}