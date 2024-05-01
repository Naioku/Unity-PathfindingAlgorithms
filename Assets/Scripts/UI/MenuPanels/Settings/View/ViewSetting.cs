using System;
using UI.Buttons;
using UI.Localization;
using UnityEngine;

namespace UI.MenuPanels.Settings.View
{
    public class ViewSetting : MonoBehaviour
    {
        [Header("Programmer:")]
        [SerializeField] private LocalizedTextMeshPro label;
        [SerializeField] private ButtonSimple button;

        private RectTransform rectTransform;
        private Action<EntryPosition> onButtonSelect;
        private EntryPosition entryPosition;

        public ButtonSimple Button => button;
        
        
        private void Awake() => rectTransform = transform.GetComponent<RectTransform>();

        public void Initialize(Enums.SettingName displayedName, Action onButtonPress, Action<EntryPosition> onButtonSelect)
        {
            name = displayedName.ToString();
            label.Initialize(displayedName);
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