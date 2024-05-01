using TMPro;
using UnityEngine;

namespace UI.Buttons
{
    public class ButtonSimple : Button
    {
        [SerializeField] private TextMeshProUGUI label;
        
        public string Label
        {
            set => label.text = value;
        }

        protected override Color LabelColor
        {
            set => label.color = value;
        }
    }
}