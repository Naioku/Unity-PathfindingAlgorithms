using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class Button : Selectable, IPointerClickHandler, ISubmitHandler
    {
        private TextMeshProUGUI textLabel;
        
        public event Action OnSelectAction;
        public event Action OnClickAction;
        public event Action OnSubmitAction;
        public event Action OnPressAction;
        
        public string Label
        {
            set => textLabel.text = value;
        }

        public Color Color
        {
            set
            {
                ColorBlock newBlock = colors;
                newBlock.normalColor = value;
                colors = newBlock;
            }
        }

        public void SetNavigation(
            Selectable onUp = null,
            Selectable onDown = null,
            Selectable onLeft = null,
            Selectable onRight = null)
        {
            
            navigation = new Navigation
            {
                selectOnUp = onUp,
                selectOnDown = onDown,
                selectOnLeft = onLeft,
                selectOnRight = onRight,
                mode = Navigation.Mode.Explicit
            };
        }

        protected override void Awake()
        {
            base.Awake();
            textLabel = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void ResetObj()
        {
            OnSelectAction = null;
            OnClickAction = null;
            OnSubmitAction = null;
            OnPressAction = null;
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            Press();
            OnClickAction?.Invoke();
        }
        
        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // Unity comment: if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
            
            OnSubmitAction?.Invoke();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnSelectAction?.Invoke();
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            OnPressAction?.Invoke();
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}