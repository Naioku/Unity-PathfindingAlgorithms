using UnityEngine;

namespace UI.HUDPanels
{
    public abstract class BaseHUDController : MonoBehaviour
    {
        private void Start() => Hide();

        public virtual void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}