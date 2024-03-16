using System;
using UI.Buttons;
using UnityEngine;

namespace UI.MenuPanels
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] protected Button backButton;

        protected void Initialize(Action onBack) => backButton.OnPressAction += onBack;

        public virtual void Show() => gameObject.SetActive(true);
        public void Close() => gameObject.SetActive(false);
    }
}