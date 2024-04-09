using System;
using TMPro;
using UI.MenuPanels.Settings.SettingEntries;
using UnityEngine;
using Button = UI.Buttons.Button;

namespace UI.MenuPanels.Settings.View
{
    public class ViewSetting : MonoBehaviour
    {
        [Header("Programmer:")]
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        
        private RectTransform rectTransform;
        private Action<EntryPosition> onButtonSelect;
        private EntryPosition entryPosition;

        public Button Button => button;
        
        private void Awake() => rectTransform = transform.GetComponent<RectTransform>();
        
        public void Initialize(string inputText, Action onButtonPress, Action<EntryPosition> onButtonSelect)
        {
            name = inputText;
            label.text = inputText;
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
            // Todo: You need to find a way to get to know where, in normalized vertical position, is the specific entry regarding the Content object.
            onButtonSelect.Invoke(entryPosition);
        }
    }
}