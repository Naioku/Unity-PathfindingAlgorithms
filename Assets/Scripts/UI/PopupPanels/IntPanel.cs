using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class IntPanel : InputPanel<int>
    {
        [SerializeField] private TMP_InputField inputField;

        public override GameObject SelectableOnOpen => inputField.gameObject;
        
        protected override void SetInitialValue(int initialValue) => inputField.text = initialValue.ToString();
        protected override void Confirm()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                inputField.text = default(int).ToString();
            }
            
            onConfirm.Invoke(int.Parse(inputField.text));
        }
    }
}