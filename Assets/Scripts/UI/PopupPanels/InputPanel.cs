using System;
using UI.Buttons;
using UnityEngine;

namespace UI.PopupPanels
{
    public abstract class InputPanel<TReturn> : PopupPanel
    {
        [SerializeField] private Button confirmationButton;
        
        protected Action<TReturn> onConfirm;
        
        public void Initialize(string header, TReturn initialValue, Action<TReturn> onConfirm)
        {
            base.Initialize(header);
            SetInitialValue(initialValue);
            this.onConfirm = onConfirm;
            confirmationButton.OnPressAction += OnConfirm;
            SelectDefaultButton();
        }

        protected abstract void SetInitialValue(TReturn initialValue);

        protected abstract void OnConfirm();

        /// <summary>
        /// Selects specified button on initialisation. By default it is the confirmation button.
        /// If You want to change the behaviour, just override this method without calling the base.
        /// </summary>
        protected virtual void SelectDefaultButton() => confirmationButton.Select();
    }
}