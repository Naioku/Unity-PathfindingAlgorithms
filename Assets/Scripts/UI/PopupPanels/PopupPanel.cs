using TMPro;
using UnityEngine;

namespace UI.PopupPanels
{
    public class PopupPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI headerLabel;

        protected void Initialize(string header) => headerLabel.text = header;
    }
}