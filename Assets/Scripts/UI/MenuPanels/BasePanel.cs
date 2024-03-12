using UnityEngine;

namespace UI.MenuPanels
{
    public class BasePanel : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        protected UIMenuController.InitData initData;

        protected virtual void Awake() {}

        public void Initialize(UIMenuController.InitData initData)
        {
            this.initData = initData;
            backButton.OnPressAction += initData.onBack;
        }
        
        public void Show() => gameObject.SetActive(true);
        public void Close() => gameObject.SetActive(false);
    }
}