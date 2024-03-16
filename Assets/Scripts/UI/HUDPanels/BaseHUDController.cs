using System;
using System.Collections.Generic;
using UI.Buttons;
using UnityEngine;

namespace UI.HUDPanels
{
    public abstract class BaseHUDController<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private Button backButton;
        [SerializeField] private List<TaggedButton<T>> buttons;
        
        private Dictionary<T, TaggedButton<T>> buttonsLookup;

        private void Awake() => BuildButtonsLookup();

        public void Initialize(ButtonData onBack, Dictionary<T, ButtonData> buttonsData)
        {
            if (buttonsData.Count != buttonsLookup.Count)
            {
                Debug.LogError($"{name}: Actions count does not match the buttons count.");
                return;
            }

            InitButtonsData(onBack, buttonsData);
            InitButtonsNavigation();
        }

        public void Deinitialize()
        {
            foreach (TaggedButton<T> button in buttons)
            {
                button.ResetObj();
            }
            backButton.ResetObj();
        }
        
        private void Start() => Hide();

        public virtual void Show()
        {
            gameObject.SetActive(true);
            SelectButton(0);
        }

        public void Hide() => gameObject.SetActive(false);

        public void SelectButton(T type) => buttonsLookup[type].Select();

        private void InitButtonsData(ButtonData onBack, Dictionary<T, ButtonData> buttonsData)
        {
            backButton.OnPressAction += onBack.Action;
            backButton.Label = onBack.Label;

            foreach (var data in buttonsData)
            {
                TaggedButton<T> button = buttonsLookup[data.Key];

                button.OnPressAction += buttonsData[button.Tag].Action;
                button.Label = buttonsData[button.Tag].Label;
            }
        }

        private void InitButtonsNavigation()
        {
            List<Button> buttons = new List<Button>();
            foreach (TaggedButton<T> button in this.buttons)
            {
                buttons.Add(button);
            }

            buttons.Add(backButton);

            int buttonsCount = buttons.Count;
            for (int i = 0; i < buttonsCount; i++)
            {
                buttons[i].SetNavigation
                (
                    onUp: Utility.CalculateButtonForNavigation(i, Enums.Direction.Backward, buttonsCount, buttons),
                    onDown: Utility.CalculateButtonForNavigation(i, Enums.Direction.Forward, buttonsCount, buttons)
                );
            }
        }
        
        private void BuildButtonsLookup()
        {
            buttonsLookup = new Dictionary<T, TaggedButton<T>>();
            foreach (TaggedButton<T> button in buttons)
            {
                buttonsLookup.Add(button.Tag, button);
            }
        }

        private void SelectButton(int index) => buttons[index].Select();
    }
}