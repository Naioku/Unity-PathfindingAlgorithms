using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UISettingGroup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Transform inputEntries;
        
        private RectTransform rectTransform;

        private void Awake() => rectTransform = transform.GetComponent<RectTransform>();

        public void Initialize(string labelText, List<UISetting> entries)
        {
            label.text = labelText;
            foreach (UISetting entry in entries)
            {
                entry.SetParent(inputEntries);
            }
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }
    }
}