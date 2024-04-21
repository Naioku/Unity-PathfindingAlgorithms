using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UpdateSystem.CoroutineSystem;

namespace UI
{
    /// <summary>
    /// Manages visualisation of interactable elements, which should not be selectable.
    /// Custom <see cref="Selectable"/> class.
    /// </summary>
    [AddComponentMenu("Naioku/UI/UI Interactable")]
    public class UIInteractable :
        UIBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Color baseColor = Color.white;
        [SerializeField] private Color hoveringColor = Color.white;
        [SerializeField] private Color pressingColor = Color.white;
        
        /// <summary>
        ///  Naming corresponds to the <see cref="Selectable"/>.
        /// </summary>
        [SerializeField] private float fadeDuration = 0.1f;

        private State state;
        private CoroutineManager.CoroutineCaller coroutineCaller;
        private Guid colorTransitionCoroutine;

        private State StateValue
        {
            set
            {
                switch (value)
                {
                    case State.Base:
                        ChangeColor(baseColor);
                        break;

                    case State.Hovered:
                        ChangeColor(hoveringColor);
                        break;
                    
                    case State.Pressed:
                        ChangeColor(pressingColor);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                state = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Application.isPlaying)
            {
                StopColorTransition();
            }
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (background == null) return;
            
            background.color = baseColor;
        }
#endif

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (Mouse.current.leftButton.isPressed) return;
            
            StateValue = State.Hovered;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (state == State.Pressed) return;
            
            StateValue = State.Base;
        }

        public virtual void OnPointerDown(PointerEventData eventData) => StateValue = State.Pressed;
        public virtual void OnPointerUp(PointerEventData eventData) =>
            StateValue = eventData.hovered.Contains(gameObject) ? State.Hovered : State.Base;

        private void ChangeColor(Color targetColor)
        {
            if (Application.isPlaying)
            {
                StartColorTransition(targetColor);
            }
            else
            {
                background.color = targetColor;
            }
        }
        
        private void StartColorTransition(Color targetColor)
        {
            if (colorTransitionCoroutine != Guid.Empty)
            {
                StopColorTransition();
            }
            
            colorTransitionCoroutine = coroutineCaller.StartCoroutine(DoColorTransition(targetColor));
        }

        private void StopColorTransition() => coroutineCaller.StopCoroutine(ref colorTransitionCoroutine);

        private IEnumerator DoColorTransition(Color targetColor)
        {
            Color startingColor = background.color;
            float deltaTime = 0;
            while (deltaTime < fadeDuration)
            {
                float timeNormalized = deltaTime / fadeDuration;
                background.color = Color.Lerp(startingColor, targetColor, timeNormalized);
                deltaTime += Time.deltaTime;
                yield return null;
            }
            
            coroutineCaller.StopCoroutine(ref colorTransitionCoroutine);
        }

        private enum State
        {
            Base, Hovered, Pressed
        }
    }
}