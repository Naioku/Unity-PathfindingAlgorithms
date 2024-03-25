using System;
using CustomInputSystem;
using TMPro;
using UI.Buttons;
using UnityEngine;

namespace UI.PopupPanels
{
    public abstract class PopupPanel : MonoBehaviour
    {
        [SerializeField] protected Button confirmationButton;
        [SerializeField] protected Button closeButton;
        [SerializeField] private TextMeshProUGUI headerLabel;

        private Action onClose;

        protected virtual void Awake()
        {
            InputManager inputManager = AllManagers.Instance.InputManager;
            inputManager.PopupMap.OnConfirmData.Canceled += Confirm;
            inputManager.PopupMap.OnCloseData.Canceled += Close;
        }

        protected virtual void OnDestroy()
        {
            InputManager inputManager = AllManagers.Instance.InputManager;
            inputManager.PopupMap.OnConfirmData.Canceled -= Confirm;
            inputManager.PopupMap.OnCloseData.Canceled -= Close;
        }

        protected void Initialize(string header, Action onClose)
        {
            headerLabel.text = header;
            this.onClose = onClose;
            confirmationButton.OnPressAction += Confirm;
            closeButton.OnPressAction += Close;
        }

        protected abstract void Confirm();
        private void Close() => onClose.Invoke();
    }
}