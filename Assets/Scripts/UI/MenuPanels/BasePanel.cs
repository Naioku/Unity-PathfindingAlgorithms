using System;
using UI.Buttons;
using UnityEngine;

namespace UI.MenuPanels
{
    public abstract class BasePanel : UIStaticPanel
    {
        [SerializeField] protected Button backButton;

        protected void Initialize(Action onBack) => backButton.OnPressAction += onBack;

        public override void Show()
        {
            base.Show();
            SelectDefaultButton();
        }
    }
}