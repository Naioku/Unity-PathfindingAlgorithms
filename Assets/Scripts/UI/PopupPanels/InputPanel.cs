using System;
using UnityEngine;

namespace UI.PopupPanels
{
    public abstract class InputPanel<TReturn> : PopupPanel
    {
        protected Action<TReturn> onConfirm;

        public abstract override GameObject SelectableOnOpen { get; }

        public void Initialize(string header, Action onClose, TReturn initialValue, Action<TReturn> onConfirm)
        {
            base.Initialize(header, onClose);
            SetInitialValue(initialValue);
            this.onConfirm = onConfirm;
        }

        protected abstract void SetInitialValue(TReturn initialValue);
    }
}