using System;
using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class ConfirmationPanel : PopupPanel
    {
        private const string ConfirmationButtonText = "Yes";
        private const string ClosingButtonText = "No";
        
        [SerializeField] private TextMeshProUGUI messageLabel;
        
        private Action onConfirm;
        
        public void Initialize(string header, Action onClose, string message, Action onConfirm)
        {
            base.Initialize(header, onClose, ConfirmationButtonText, ClosingButtonText);
            messageLabel.text = message;
            this.onConfirm = onConfirm;
        }
        
        protected override void Confirm() => onConfirm.Invoke();
    }
}