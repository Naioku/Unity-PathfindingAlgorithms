using UI.Buttons;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class InputEntry : MonoBehaviour
    {
        [FormerlySerializedAs("inputEntryType")] [SerializeField] private Enums.UIPopupType uiPopupType;
        [SerializeField] private Button button;
        
        private void OpenEntryPanel()
        {
            // AllManagers.Instance.UIManager.OpenInputPanel(inputEntryType);
        }
    }
}