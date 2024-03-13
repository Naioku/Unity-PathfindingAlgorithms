using UnityEngine;

namespace UI.MenuPanels
{
    public class BasePanel : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        protected MenuController.InitData initData;

        protected virtual void Awake() {}

        public void Initialize(MenuController.InitData initData)
        {
            this.initData = initData;
            backButton.OnPressAction += initData.onBack;
        }
        
        public virtual void Show() => gameObject.SetActive(true);
        public void Close() => gameObject.SetActive(false);
    }
}