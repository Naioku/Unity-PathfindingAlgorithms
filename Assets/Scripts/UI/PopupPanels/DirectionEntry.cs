using System;
using UI.Buttons;
using UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PopupPanels
{
    public class DirectionEntry : MonoBehaviour
    {
        [SerializeField] private ButtonSimple button;
        [SerializeField] private Color selectedColor;
        
        private Image background;
        private Action<DirectionEntry> onPressAction;
        private Color normalColor;
        private bool selected;
        
        public Enums.PermittedDirection Direction { get; private set; }

        public bool Selected
        {
            set
            {
                selected = value;
                background.color = selected ? selectedColor : normalColor;
            }
        }

        private void Awake() => background = GetComponent<Image>();

        public void Initialize(Enums.PermittedDirection direction, Action<DirectionEntry> onPressAction)
        {
            LocalizedTextManager localizedTextManager = AllManagers.Instance.LocalizedTextManager;
            
            Direction = direction;
            this.onPressAction = onPressAction;
            button.Label = localizedTextManager.GetLocalizedString(Direction).GetLocalizedString();
            button.OnPressAction += HandleOnPress;
            normalColor = background.color;

        }

        private void HandleOnPress() => onPressAction.Invoke(this);
    }
}