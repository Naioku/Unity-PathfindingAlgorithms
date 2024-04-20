using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class FloatPanel : InputPanel<float>
    {
        [SerializeField] private TMP_InputField inputField;

        public override GameObject SelectableOnOpen => inputField.gameObject;
        
        protected override void SetInitialValue(float initialValue) => inputField.text = initialValue.ToString(CultureInfo.CurrentCulture);
        protected override void Confirm()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                inputField.text = default(float).ToString(CultureInfo.CurrentCulture);
            }
            
            onConfirm.Invoke(float.Parse(inputField.text));
        }
    }
}