using System;
using System.Collections.Generic;
using UI.Buttons;
using UI.Localization;
using UnityEngine;

namespace UI.MenuPanels
{
    [Serializable]
    public class MainPanel : BasePanel
    {
        [SerializeField] private LocalizedTextMeshPro header;
        [SerializeField] private List<MainMenuPanelButton> buttons = new();
        
        private Dictionary<Enums.MainMenuPanelButtonTag, MainMenuPanelButton> buttonsLookup = new();
        
        public void Initialize(Action onBack, Dictionary<Enums.MainMenuPanelButtonTag, Action> actionsData)
        {
            base.Initialize(onBack, Enums.GeneralText.MainMenuButtonQuit);
            header.Initialize(Enums.GeneralText.MainMenuHeader);
            BuildButtonsLookup();
            SetupButtonsNavigation();
            InitButtonActions(actionsData);
        }

        private void OnDestroy() => header.Destroy();

        protected override void SelectDefaultButton() => buttons[0].Select();

        private void BuildButtonsLookup()
        {
            foreach (MainMenuPanelButton button in buttons)
            {
                buttonsLookup.Add(button.Tag, button);
                button.Initialize(button.Tag);
            }
        }

        private void SetupButtonsNavigation()
        {
            List<ButtonLocalized> buttons = new List<ButtonLocalized>();
            foreach (MainMenuPanelButton menuButton in this.buttons)
            {
                buttons.Add(menuButton);
            }
            buttons.Add(backButton);

            int buttonsCount = buttons.Count;
            for (int i = 0; i < buttonsCount; i++)
            {
                buttons[i].SetNavigation(new SelectableNavigation
                {
                    OnUp = Utility.Utility.CalculateNextSelectableElement(i, Enums.Direction.Backward, buttonsCount, buttons),
                    OnDown = Utility.Utility.CalculateNextSelectableElement(i, Enums.Direction.Forward, buttonsCount, buttons)
                });
            }
        }
        
        private void InitButtonActions(Dictionary<Enums.MainMenuPanelButtonTag, Action> actionsData)
        {
            if (actionsData.Count != buttonsLookup.Count)
            {
                Debug.LogError($"{name}: Actions count does not match the buttons count.");
                return;
            }
            
            foreach (var data in actionsData)
            {
                buttonsLookup[data.Key].OnPressAction += data.Value;
            }
        }
    }
}