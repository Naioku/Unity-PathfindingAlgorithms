using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class Button : Selectable, IPointerClickHandler, ISubmitHandler, ISelectableElement
    {
        private const string BackgroundPath = "BACKGROUND";
        private const string TextLabelPath = "TEXT";
        
        private Image background;
        private TextMeshProUGUI textLabel;
        private Color backgroundNormalColor;
        
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
                SetBackgroundColor();
                SetLabelColor();
                return;
                
                void SetBackgroundColor()
                {
                    background.color = value;
                    backgroundNormalColor = value;
                }
                
                void SetLabelColor()
                {
                    Color.RGBToHSV(value, out _, out _, out float v);
                    textLabel.color = Color.HSVToRGB(0, 0, v < 0.5 ? 1 : 0);
                }
            }
        }

        public void SetNavigation(Enums.ButtonsNaviDirection direction, Selectable selectable)
        {
            Navigation navigation = this.navigation;
            
            switch (direction)
            {
                case Enums.ButtonsNaviDirection.Up:
                    navigation.selectOnUp = selectable;
                    break;
                
                case Enums.ButtonsNaviDirection.Down:
                    navigation.selectOnDown = selectable;
                    break;
                
                case Enums.ButtonsNaviDirection.Left:
                    navigation.selectOnLeft = selectable;
                    break;
                
                case Enums.ButtonsNaviDirection.Right:
                    navigation.selectOnRight = selectable;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            
            this.navigation = navigation;
        }
        
        public void SetNavigation(SelectableNavigation navigation)
        {
            this.navigation = new Navigation
            {
                selectOnUp = navigation.OnUp,
                selectOnDown = navigation.OnDown,
                selectOnLeft = navigation.OnLeft,
                selectOnRight = navigation.OnRight,
                mode = Navigation.Mode.Explicit
            };
        }

        protected override void Awake()
        {
            base.Awake();
            background = transform.Find(BackgroundPath).GetComponent<Image>();
            textLabel = transform.Find(TextLabelPath).GetComponent<TextMeshProUGUI>();
            backgroundNormalColor = background.color;
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

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            // Color disabledColor = colors.disabledColor;
            // ColorBlock newBlock = colors;
            // newBlock.disabledColor = new Color();
            // colors = newBlock;
            base.DoStateTransition(state, instant);
            
            background.color = state == SelectionState.Disabled ? colors.disabledColor : backgroundNormalColor;
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