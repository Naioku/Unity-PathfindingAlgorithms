using System;
using System.Collections.Generic;
using UI.Buttons;
using UI.Localization;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using Utilities;

namespace UI.HUDPanels
{
    public abstract class BaseHUDController<T> : UIStaticPanel where T : Enum
    {
        [SerializeField] protected LocalizedTextMeshPro staticLabel;
        [SerializeField] protected LocalizedTextMeshPro dynamicLabel;

        [SerializeField] private ButtonTextLocalized backButton;
        [SerializeField] private List<TaggedButtonLocalized<T>> buttons;
        
        private Dictionary<T, TaggedButtonLocalized<T>> buttonsLookup;

        private void Awake() => BuildButtonsLookup();
        private void Start() => Hide();
        
        public void Initialize(ButtonData onBack, ButtonDataTagged<T>[] buttonsData)
        {
            if (buttonsData.Length != buttonsLookup.Count)
            {
                Debug.LogError($"{name}: Actions count does not match the buttons count.");
                return;
            }

            InitButtonsData(onBack, buttonsData);
            InitButtonsNavigation();
            Initialize();
        }

        public void SelectButton(T type) => buttonsLookup[type].Select();

        protected abstract void Initialize();
        protected override void SelectDefaultButton() => SelectButton(0);

        private void InitButtonsData(ButtonData onBack, ButtonDataTagged<T>[] buttonsData)
        {
            backButton.OnPressAction += onBack.Action;
            backButton.Initialize(Enums.GeneralText.ButtonBack, new KeyValuePair<string, IVariable>("0", onBack.Binding));

            foreach (ButtonDataTagged<T> data in buttonsData)
            {
                TaggedButtonLocalized<T> button = buttonsLookup[data.Tag];

                button.OnPressAction += data.Action;
                button.Initialize(new KeyValuePair<string, IVariable>("0", data.Binding));
            }
        }

        private void InitButtonsNavigation()
        {
            List<ButtonTextLocalized> buttons = new List<ButtonTextLocalized>();
            foreach (TaggedButtonLocalized<T> button in this.buttons)
            {
                buttons.Add(button);
            }

            buttons.Add(backButton);

            int buttonsCount = buttons.Count;
            for (int i = 0; i < buttonsCount; i++)
            {
                buttons[i].SetNavigation(new SelectableNavigation
                {
                    OnUp = Utility.CalculateNextSelectableElement(i, Enums.Direction.Backward, buttonsCount, buttons),
                    OnDown = Utility.CalculateNextSelectableElement(i, Enums.Direction.Forward, buttonsCount, buttons)
                });
            }
        }
        
        private void BuildButtonsLookup()
        {
            buttonsLookup = new Dictionary<T, TaggedButtonLocalized<T>>();
            foreach (TaggedButtonLocalized<T> button in buttons)
            {
                buttonsLookup.Add(button.Tag, button);
            }
        }

        private void SelectButton(int index) => buttons[index].Select();
    }
}