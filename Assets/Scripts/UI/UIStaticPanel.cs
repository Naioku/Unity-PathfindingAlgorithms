using UnityEngine;

namespace UI
{
    public abstract class UIStaticPanel : MonoBehaviour
    {
        public abstract void SelectDefaultButton();

        public virtual void Show()
        {
            AllManagers.Instance.UIManager.CurrentStaticPanel = this;
            gameObject.SetActive(true);
        }

        public void Hide(bool callbackToUIManager = true)
        {
            gameObject.SetActive(false);
            if (callbackToUIManager)
            {
                AllManagers.Instance.UIManager.CurrentStaticPanel = null;
            }
        }
    }
}