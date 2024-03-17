using System;
using System.Collections.Generic;
using UI.Buttons;
using UnityEngine;

namespace UI.MenuPanels
{
    [Serializable]
    public class MainPanel : BasePanel
    {
        [SerializeField] private List<MainMenuPanelButton> buttons = new List<MainMenuPanelButton>();
        
        private Dictionary<Enums.MainMenuPanelButtonTag, MainMenuPanelButton> buttonsLookup = new Dictionary<Enums.MainMenuPanelButtonTag, MainMenuPanelButton>();
        
        public void Initialize(Action onBack, Dictionary<Enums.MainMenuPanelButtonTag, Action> actionsData)
        {
            base.Initialize(onBack);
            BuildButtonsLookup();
            SetupButtonsNavigation();
            InitButtonActions(actionsData);
        }
        
        public override void SelectDefaultButton() => buttons[0].Select();

        private void BuildButtonsLookup()
        {
            foreach (MainMenuPanelButton button in buttons)
            {
                buttonsLookup.Add(button.Tag, button);
            }
        }

        private void SetupButtonsNavigation()
        {
            List<Button> buttons = new List<Button>();
            foreach (MainMenuPanelButton menuButton in this.buttons)
            {
                buttons.Add(menuButton);
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