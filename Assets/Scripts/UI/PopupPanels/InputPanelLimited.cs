using System;

namespace UI.PopupPanels
{
    public abstract class InputPanelLimited<TReturn> : InputPanel<TReturn>
    {
        protected TReturn minValue;
        protected TReturn maxValue;
        
        public void Initialize(
            string header,
            Action onClose,
            TReturn initialValue,
            TReturn minValue,
            TReturn maxValue,
            Action<TReturn> onConfirm)
        {
            base.Initialize(header, onClose, initialValue, onConfirm);
            SetInitialValue(initialValue);
            this.onConfirm = onConfirm;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}