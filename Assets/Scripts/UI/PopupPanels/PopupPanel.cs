using System;
using CustomInputSystem;
using TMPro;
using UI.Buttons;
using UnityEngine;

namespace UI.PopupPanels
{
    public abstract class PopupPanel : MonoBehaviour
    {
        private const string ConfirmationButtonText = "OK";
        private const string ClosingButtonText = "X";
        
        [SerializeField] protected ButtonSimple confirmationButton;
        [SerializeField] protected ButtonSimple closeButton;
        [SerializeField] private TextMeshProUGUI headerLabel;

        private Action onClose;
        public virtual GameObject SelectableOnOpen => null;

        protected virtual void Awake() => AddInput();
        protected virtual void OnDestroy() => RemoveInput();

        protected void AddInput()
        {
            InputManager inputManager = AllManagers.Instance.InputManager;
            inputManager.PopupMap.OnConfirmData.Canceled += Confirm;
            inputManager.PopupMap.OnCloseData.Canceled += Close;
        }

        protected void RemoveInput()
        {
            InputManager inputManager = AllManagers.Instance.InputManager;
            inputManager.PopupMap.OnConfirmData.Canceled -= Confirm;
            inputManager.PopupMap.OnCloseData.Canceled -= Close;
        }

        protected void Initialize(
            string header,
            Action onClose,
            string confirmationButtonText = ConfirmationButtonText,
            string closingButtonText = ClosingButtonText)
        {
            headerLabel.text = header;
            this.onClose = onClose;
            confirmationButton.OnPressAction += Confirm;
            confirmationButton.Label = confirmationButtonText;
            closeButton.OnPressAction += Close;
            closeButton.Label = closingButtonText;
        }

        protected abstract void Confirm();
        private void Close() => onClose.Invoke();
    }
}