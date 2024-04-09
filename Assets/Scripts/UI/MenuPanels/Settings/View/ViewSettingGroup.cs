using TMPro;
using UnityEngine;

namespace UI.MenuPanels.Settings.View
{
    public class ViewSettingGroup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private RectTransform inputEntries;

        public RectTransform UIParent => inputEntries;
        
        public void Initialize(string labelText)
        {
            label.text = labelText;
        }
    }
}