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

        public void Initialize(Transform parent, string labelText)
        {
            label.text = labelText;
            transform.SetParent(parent);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

        public void AddChild(UISetting child) => child.SetParent(inputEntries);
    }
}