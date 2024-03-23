using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class FloatPanel : InputPanel<float>
    {
        [SerializeField] private TMP_InputField inputField;

        protected override void SetInitialValue(float initialValue) => inputField.text = initialValue.ToString(CultureInfo.CurrentCulture);
        protected override void OnConfirm() => onConfirm.Invoke(float.Parse(inputField.text));
    }
}