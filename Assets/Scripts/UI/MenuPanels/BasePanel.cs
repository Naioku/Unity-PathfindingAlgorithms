using System;
using UI.Buttons;
using UnityEngine;

namespace UI.MenuPanels
{
    public abstract class BasePanel : UIStaticPanel
    {
        [SerializeField] protected Button backButton;

        private Action onBack;
        
        protected void Initialize(Action onBack)
        {
            this.onBack = onBack;
            backButton.OnPressAction += OnBack;
        }

        protected virtual void ResetPanel() {}

        private void OnBack()
        {
            ResetPanel();
            onBack.Invoke();
        }
    }
}