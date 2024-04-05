using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Unity <see cref="UnityEngine.UI.Scrollbar"/> without Selectable class and other IMHO unnecessary stuff.
    /// </summary>
    [AddComponentMenu("Naioku/UI/Naioku Scrollbar")]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class Scrollbar :
        UIBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IInitializePotentialDragHandler,
        ICanvasElement
    {
        [SerializeField] private RectTransform handleRect;
        [SerializeField] private Direction direction = Direction.LeftToRight;
        [SerializeField, Range(0f, 1f)] private float value;
        [SerializeField, Range(0f, 1f)] private float size = 0.2f;
        [SerializeField, Range(0, 11)] private int numberOfSteps;
        
        private RectTransform containerRect;
        private Vector2 offset = Vector2.zero; // The offset from handle position to mouse down position
        private bool delayedUpdateVisuals; // This "delayed" mechanism is required for case 1037681.
        
        public event Action<float> OnValueChanged;
        
        // Size of each step.
        public float StepSize => numberOfSteps > 1 ? 1f / (numberOfSteps - 1) : 0.1f;
        private AxisEnum Axis => direction == Direction.LeftToRight || direction == Direction.RightToLeft ? AxisEnum.Horizontal : AxisEnum.Vertical;
        private bool ReverseValue => direction == Direction.RightToLeft || direction == Direction.TopToBottom;


        /// <summary>
        /// The current value of the scrollbar, between 0 and 1.
        /// </summary>
        public float Value
        {
            get
            {
                float val = value;
                if (numberOfSteps > 1)
                {
                    val = Mathf.Round(val * (numberOfSteps - 1)) / (numberOfSteps - 1);
                }

                return val;
            }
            set => Set(value);
        }

        public float Size
        {
            set => size = value;
        }

        /// <summary>
        /// Set the value of the scrollbar without invoking onValueChanged callback.
        /// </summary>
        /// <param name="input">The new value for the scrollbar.</param>
        public void SetValueWithoutNotify(float input)
        {
            Set(input, false);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            size = Mathf.Clamp01(size);

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (IsActive())
            {
                UpdateCachedReferences();
                Set(value, false);
                // Update rects (in next update) since other things might affect them even if value didn't change.
                delayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }
#endif
        
        protected override void OnEnable()
        {
            UpdateCachedReferences();
            Set(value, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        // Todo: Change to my custom updater. Remember that this is called in editor too.
        /// <summary>
        /// Update the rect based on the delayed update visuals.
        /// Got around issue of calling sendMessage from onValidate.
        /// </summary>
        protected void Update()
        {
            if (delayedUpdateVisuals)
            {
                delayedUpdateVisuals = false;
                UpdateVisuals();
            }
        }

        private void UpdateCachedReferences()
        {
            if (handleRect && handleRect.parent != null)
                containerRect = handleRect.parent.GetComponent<RectTransform>();
            else
                containerRect = null;
        }

        private void Set(float input, bool sendCallback = true)
        {
            float currentValue = value;

            // bugfix (case 802330) clamp01 input in callee before calling this function, this allows inertia from dragging content to go past extremities without being clamped
            value = input;

            // If the stepped value doesn't match the last one, it's time to update
            if (Mathf.Approximately(currentValue, Value)) return;

            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Scrollbar.value", this);
                OnValueChanged?.Invoke(Value);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive()) return;

            UpdateVisuals();
        }

        // Force-update the scroll bar. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UpdateCachedReferences();
            }
#endif

            if (containerRect != null)
            {
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                float movement = Mathf.Clamp01(Value) * (1 - size);
                if (ReverseValue)
                {
                    anchorMin[(int)Axis] = 1 - movement - size;
                    anchorMax[(int)Axis] = 1 - movement;
                }
                else
                {
                    anchorMin[(int)Axis] = movement;
                    anchorMax[(int)Axis] = movement + size;
                }

                handleRect.anchorMin = anchorMin;
                handleRect.anchorMax = anchorMax;
            }
        }
        
#region ICanvasElement
        public void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                OnValueChanged?.Invoke(Value);
#endif
        }

        public void LayoutComplete() {}
        public void GraphicUpdateComplete() {}
#endregion
        
#region IHandlers
        /// <summary>
        /// Handling for when the scrollbar value is begin being dragged.
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            if (containerRect == null)
                return;

            offset = Vector2.zero;
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    handleRect,
                    eventData.pointerPressRaycast.screenPosition,
                    eventData.enterEventCamera)) return;
            
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    handleRect,
                    eventData.pointerPressRaycast.screenPosition,
                    eventData.pressEventCamera,
                    out Vector2 localMousePos))
            {
                offset = localMousePos - handleRect.rect.center;
            }
        }

        /// <summary>
        /// Handling for when the scrollbar value is dragged.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData)) return;

            if (containerRect != null)
            {
                UpdateDrag(eventData);
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData) => eventData.useDragThreshold = false;
#endregion
        
        private bool MayDrag(PointerEventData eventData) => IsActive() && eventData.button == PointerEventData.InputButton.Left;
        
        // Update the scroll bar's position based on the mouse.
        private void UpdateDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (containerRect == null) return;
            
            // Doc states: Returns true except when the drag operation is not on the same display as it originated.
            // I don't want to figure it out now and don't know if it won't break some co-op functionality,
            // so I leave it here for the future me :p.
            //
            // Vector2 position = Vector2.zero;
            // if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
            //     return;

            Vector2 position = eventData.position;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    containerRect,
                    position,
                    eventData.pressEventCamera,
                    out Vector2 localCursor)) return;

            Vector2 handleCenterRelativeToContainerCorner = localCursor - offset - containerRect.rect.position;
            Vector2 handleCorner = handleCenterRelativeToContainerCorner - (handleRect.rect.size - handleRect.sizeDelta) * 0.5f;

            float parentSize = Axis == 0 ? containerRect.rect.width : containerRect.rect.height;
            float remainingSize = parentSize * (1 - size);
            if (remainingSize <= 0) return;

            DoUpdateDrag(handleCorner, remainingSize);
        }

        private void DoUpdateDrag(Vector2 handleCorner, float remainingSize)
        {
            switch (direction)
            {
                case Direction.LeftToRight:
                    Set(Mathf.Clamp01(handleCorner.x / remainingSize));
                    break;
                case Direction.RightToLeft:
                    Set(Mathf.Clamp01(1f - handleCorner.x / remainingSize));
                    break;
                case Direction.BottomToTop:
                    Set(Mathf.Clamp01(handleCorner.y / remainingSize));
                    break;
                case Direction.TopToBottom:
                    Set(Mathf.Clamp01(1f - handleCorner.y / remainingSize));
                    break;
            }
        }
        
        private enum AxisEnum
        {
            Horizontal = 0,
            Vertical = 1
        }
        
        /// <summary>
        /// Setting that indicates one of four directions the scrollbar will travel.
        /// </summary>
        private enum Direction
        {
            /// <summary>
            /// Starting position is the Left.
            /// </summary>
            LeftToRight,

            /// <summary>
            /// Starting position is the Right
            /// </summary>
            RightToLeft,

            /// <summary>
            /// Starting position is the Bottom.
            /// </summary>
            BottomToTop,

            /// <summary>
            /// Starting position is the Top.
            /// </summary>
            TopToBottom,
        }
    }
}