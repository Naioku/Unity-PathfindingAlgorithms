using System;
using UI.Buttons;
using UnityEngine;

namespace UI.MenuPanels
{
    public abstract class BasePanel : UIStaticPanel
    {
        [SerializeField] protected ButtonTextLocalized backButton;

        private Action onBack;
        
        protected void Initialize(Action onBack, Enums.GeneralText onBackText = Enums.GeneralText.ButtonBack)
        {
            this.onBack = onBack;
            backButton.OnPressAction += OnBack;
            backButton.Initialize(onBackText);
        }

        protected virtual void ResetPanel() {}

        private void OnBack()
        {
            ResetPanel();
            onBack.Invoke();
        }
    }
}