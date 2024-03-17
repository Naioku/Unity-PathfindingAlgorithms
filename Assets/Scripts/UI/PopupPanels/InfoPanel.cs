using System;
using TMPro;
using UI.Buttons;
using UnityEngine;

namespace UI.PopupPanels
{
    public class InfoPanel : PopupPanel
    {
        [SerializeField] private TextMeshProUGUI infoLabel;
        [SerializeField] private Button confirmationButton;

        private Action onConfirm;
        
        public void Initialize(string header, string info, Action onConfirm)
        {
            base.Initialize(header);
            infoLabel.text = info;
            this.onConfirm = onConfirm;
            confirmationButton.OnPressAction += Confirm;
            confirmationButton.Select();
        }

        private void Confirm() => onConfirm.Invoke();
    }
}