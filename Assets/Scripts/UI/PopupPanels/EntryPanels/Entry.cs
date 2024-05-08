using System;
using UI.Buttons;
using UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PopupPanels.EntryPanels
{
    public class Entry<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private ButtonSimple button;
        
        private Color selectedColor;
        private Image background;
        private Action<Entry<T>> onPressAction;
        private Color normalColor;
        private bool selected;
        
        public T Value { get; private set; }
    
        public bool Selected
        {
            set
            {
                selected = value;
                background.color = selected ? selectedColor : normalColor;
            }
        }
    
        private void Awake()
        {
            background = GetComponent<Image>();
            selectedColor = button.ColorBlock.selectedColor;
        }

        public void Initialize(T direction, Action<Entry<T>> onPressAction)
        {
            LocalizedTextManager localizedTextManager = AllManagers.Instance.LocalizedTextManager;
            
            Value = direction;
            this.onPressAction = onPressAction;
            button.Label = localizedTextManager.GetLocalizedString(Value).GetLocalizedString();
            button.OnPressAction += HandleOnPress;
            normalColor = background.color;
        }
    
        private void HandleOnPress() => onPressAction.Invoke(this);
    }
}