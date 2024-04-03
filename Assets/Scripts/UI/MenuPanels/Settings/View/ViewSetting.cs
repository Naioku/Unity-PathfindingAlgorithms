using System;
using TMPro;
using UI.Buttons;
using UnityEngine;

namespace UI.MenuPanels.Settings.View
{
    public class ViewSetting : MonoBehaviour
    {
        [Header("Programmer")]
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        
        private RectTransform rectTransform;

        public Button Button => button;
        
        private void Awake() => rectTransform = transform.GetComponent<RectTransform>();
        
        public void Initialize(string inputText, Action buttonAction)
        {
            name = inputText;
            label.text = inputText;
            button.OnPressAction += buttonAction;
        }
        
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

        public void SetNavigation(ViewSettingNavigation navigation)
        {
            SelectableNavigation selectableNavigation = new SelectableNavigation();

            if (navigation.OnUp != null)
            {
                selectableNavigation.OnUp = navigation.OnUp.button;
            }
            
            if (navigation.OnDown != null)
            {
                selectableNavigation.OnDown = navigation.OnDown.button;
            }
            
            button.SetNavigation(selectableNavigation);
        }

        public bool IsInteractable() => button.IsInteractable();
    }
}