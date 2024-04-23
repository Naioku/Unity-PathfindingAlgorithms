using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class IntPanel : InputPanelLimited<int>
    {
        [SerializeField] private TMP_InputField inputField;

        public override GameObject SelectableOnOpen => inputField.gameObject;
        
        protected override void SetInitialValue(int initialValue) => inputField.text = initialValue.ToString();
        protected override void Confirm()
        {
            int result = string.IsNullOrEmpty(inputField.text) ? default : int.Parse(inputField.text);
            result = Mathf.Clamp(result, minValue, maxValue);
            onConfirm.Invoke(result);
        }
    }
}