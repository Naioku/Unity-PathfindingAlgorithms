using UnityEngine;

namespace UI
{
    public abstract class UIStaticPanel : MonoBehaviour
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
            SelectDefaultButton();
        }

        public void Hide() => gameObject.SetActive(false);
        protected abstract void SelectDefaultButton();
    }
}