using System;
using TMPro;
using UnityEngine;
using Button = UI.Buttons.Button;

namespace UI.MenuPanels.Settings.View
{
    public class ViewSetting : MonoBehaviour
    {
        [Header("Programmer:")]
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;

        private StaticTextManager staticTextManager;
        private RectTransform rectTransform;
        private Enums.SettingName displayedNameStaticKey;
        private Action<EntryPosition> onButtonSelect;
        private EntryPosition entryPosition;

        public Button Button => button;
        
        private string DisplayedName => staticTextManager.GetValue(displayedNameStaticKey);
        
        private void Awake()
        {
            staticTextManager = AllManagers.Instance.StaticTextManager;
            rectTransform = transform.GetComponent<RectTransform>();
        }

        public void Initialize(Enums.SettingName displayedName, Action onButtonPress, Action<EntryPosition> onButtonSelect)
        {
            displayedNameStaticKey = displayedName;
            name = DisplayedName;
            label.text = DisplayedName;
            button.OnPressAction += onButtonPress;
            this.onButtonSelect = onButtonSelect;
            button.OnSelectAction += OnButtonSelect;
        }

        public void CalcEntryPosRelatedTo(RectTransform contentRoot)
        {
            RectTransform currentRectTrans = rectTransform;
            float yPositionRelatedToRoot = 0;
            while (currentRectTrans != contentRoot)
            {
                yPositionRelatedToRoot += currentRectTrans.anchoredPosition.y;
                currentRectTrans = (RectTransform)currentRectTrans.parent;
            }

            entryPosition = new EntryPosition
            {
                Min = new Vector2(0, yPositionRelatedToRoot - rectTransform.sizeDelta.y),
                Max = new Vector2(0, yPositionRelatedToRoot)
            };
        }

        public void SetNavigation(ViewSettingNavigation navigation)
        {
            SelectableNavigation selectableNavigation = new SelectableNavigation();

            if (navigation.OnUp != null)
            {
                selectableNavigation.OnUp = navigation.OnUp.button;
            }
            
            if (navigation.OnDown != null)
            {
                selectableNavigation.OnDown = navigation.OnDown.button;
            }
            
            button.SetNavigation(selectableNavigation);
        }

        private void OnButtonSelect()
        {
            onButtonSelect.Invoke(entryPosition);
        }
    }
}