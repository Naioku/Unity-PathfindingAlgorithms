using System;
using TMPro;
using UI.Localization;
using UnityEngine;

namespace UI.PopupPanels
{
    public class ConfirmationPanel : PopupPanel
    {
        [SerializeField] private TextMeshProUGUI messageLabel;
        
        private Action onConfirm;
        
        public void Initialize(
            string header,
            Action onClose,
            LocalizedContentCache localizedContentCache,
            string message,
            Action onConfirm)
        {
            base.Initialize
            (
                header,
                onClose,
                localizedContentCache.GetValue(Enums.PopupText.ConfirmationButtonYes),
                localizedContentCache.GetValue(Enums.PopupText.ConfirmationButtonNo)
            );
            messageLabel.text = message;
            this.onConfirm = onConfirm;
        }
        
        protected override void Confirm() => onConfirm.Invoke();
    }
}