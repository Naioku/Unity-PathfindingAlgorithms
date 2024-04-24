using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class FloatPanel : InputPanelLimited<float>
    {
        [SerializeField] private TMP_InputField inputField;

        public override GameObject SelectableOnOpen => inputField.gameObject;
        
        protected override void SetInitialValue(float initialValue) => inputField.text = initialValue.ToString(CultureInfo.CurrentCulture);
        protected override void Confirm()
        {
            float result = string.IsNullOrEmpty(inputField.text) ? default : float.Parse(inputField.text);
            result = Mathf.Clamp(result, minValue, maxValue);
            onConfirm.Invoke(result);
        }
    }
}