using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class ButtonIcon : Button
    {
        [SerializeField] private Image icon;

        public Enums.Icon IconName
        {
            set => icon.sprite = AllManagers.Instance.UIManager.GetIcon(value);
        }

        protected override Color LabelColor
        {
            set => icon.color = value;
        }
    }
}