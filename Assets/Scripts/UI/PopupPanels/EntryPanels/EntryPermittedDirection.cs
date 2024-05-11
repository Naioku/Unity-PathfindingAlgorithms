using System;
using Settings;
using UI.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PopupPanels.EntryPanels
{
    public class EntryPermittedDirection : Entry<PermittedDirection, Enums.PermittedDirection>
    {
        [SerializeField] private ButtonIcon onOffButton;
        [SerializeField] private Image screen;
        [SerializeField, Range(0, 1)] private float disablingAlfa;


        private bool EntryEnabled
        {
            get => value.Enabled;
            set
            {
                this.value.Enabled = value;
                
                Color color = screen.color;
                color.a = value ? 0 : disablingAlfa;
                screen.color = color;

                onOffButton.IconName = value ? Enums.Icon.VisibilityOn : Enums.Icon.VisibilityOff;
            }
        }

        public override void Initialize(PermittedDirection direction, Action<Entry<PermittedDirection, Enums.PermittedDirection>> onPressAction)
        {
            base.Initialize(direction, onPressAction);
            onOffButton.OnPressAction += ToggleEntry;
            EntryEnabled = direction.Enabled;
        }

        private void ToggleEntry() => EntryEnabled = !EntryEnabled;
    }
}