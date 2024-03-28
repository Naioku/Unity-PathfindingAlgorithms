using System;
using TMPro;
using UI.Buttons;
using UnityEngine;

namespace UI
{
    public class UISetting : MonoBehaviour
    {
        [Header("Programmer")]
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        
        private RectTransform rectTransform;

        public Button Button => button;
        
        private void Awake() => rectTransform = transform.GetComponent<RectTransform>();
        
        public void Initialize(string inputText, Action buttonAction)
        {
            label.text = inputText;
            button.OnPressAction += buttonAction;
        }
        
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
    }
}