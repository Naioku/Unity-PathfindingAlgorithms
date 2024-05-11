using System;
using UI.Buttons;
using UI.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PopupPanels.EntryPanels
{
    public class Entry<T1, T2> : MonoBehaviour where T1 : IEntrySetting<T2> where T2 : Enum
    {
        [SerializeField] private ButtonTextSimple selectingButton;
        
        private Color selectedColor;
        private Image background;
        private Action<Entry<T1, T2>> onPressAction;
        private Color normalColor;
        private bool selected;
        
        protected T1 value;


        public T1 Value
        {
            get => value;
            private set => this.value = value;
        }

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
            selectedColor = selectingButton.ColorBlock.selectedColor;
        }

        public virtual void Initialize(T1 value, Action<Entry<T1, T2>> onPressAction)
        {
            LocalizedTextManager localizedTextManager = AllManagers.Instance.LocalizedTextManager;
            
            Value = value;
            this.onPressAction = onPressAction;
            selectingButton.Label = localizedTextManager.GetLocalizedString(Value.Enum).GetLocalizedString();
            selectingButton.OnPressAction += HandleOnPress;
            normalColor = background.color;
        }
    
        private void HandleOnPress() => onPressAction.Invoke(this);
    }
}