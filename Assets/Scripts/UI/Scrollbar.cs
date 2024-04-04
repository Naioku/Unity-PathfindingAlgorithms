using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class Scrollbar : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        [SerializeField] private RectTransform m_HandleRect;
        [SerializeField] private Direction m_Direction = Direction.LeftToRight;
        [SerializeField, Range(0f, 1f)] private float m_Value;
        [SerializeField, Range(0f, 1f)] private float m_Size = 0.2f;
        [SerializeField, Range(0, 11)] private int m_NumberOfSteps = 0;
        
        private RectTransform m_ContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;
        
        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649
        private Coroutine m_PointerDownRepeat;
        private bool isPointerDownAndNotDragging = false;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;

        
        public event Action<float> OnValueChanged; 

        // Size of each step.
        public float StepSize => m_NumberOfSteps > 1 ? 1f / (m_NumberOfSteps - 1) : 0.1f;


        /// <summary>
        /// The current value of the scrollbar, between 0 and 1.
        /// </summary>
        public float Value
        {
            get
            {
                float val = m_Value;
                if (m_NumberOfSteps > 1)
                    val = Mathf.Round(val * (m_NumberOfSteps - 1)) / (m_NumberOfSteps - 1);
                return val;
            }
            set => Set(value);
        }

        public float Size
        {
            get => m_Size;
            set => m_Size = value;
        }

        /// <summary>
        /// Set the value of the scrollbar without invoking onValueChanged callback.
        /// </summary>
        /// <param name="input">The new value for the scrollbar.</param>
        public virtual void SetValueWithoutNotify(float input)
        {
            Set(input, false);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            m_Size = Mathf.Clamp01(m_Size);

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (IsActive())
            {
                UpdateCachedReferences();
                Set(m_Value, false);
                // Update rects (in next update) since other things might affect them even if value didn't change.
                m_DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }
#endif

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                OnValueChanged?.Invoke(Value);
#endif
        }

        /// <summary>
        /// See ICanvasElement.LayoutComplete.
        /// </summary>
        public virtual void LayoutComplete()
        {}

        /// <summary>
        /// See ICanvasElement.GraphicUpdateComplete.
        /// </summary>
        public virtual void GraphicUpdateComplete()
        {}

        protected override void OnEnable()
        {
            UpdateCachedReferences();
            Set(m_Value, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
        }

        /// <summary>
        /// Update the rect based on the delayed update visuals.
        /// Got around issue of calling sendMessage from onValidate.
        /// </summary>
        protected virtual void Update()
        {
            if (m_DelayedUpdateVisuals)
            {
                m_DelayedUpdateVisuals = false;
                UpdateVisuals();
            }
        }

        void UpdateCachedReferences()
        {
            if (m_HandleRect && m_HandleRect.parent != null)
                m_ContainerRect = m_HandleRect.parent.GetComponent<RectTransform>();
            else
                m_ContainerRect = null;
        }

        void Set(float input, bool sendCallback = true)
        {
            float currentValue = m_Value;

            // bugfix (case 802330) clamp01 input in callee before calling this function, this allows inertia from dragging content to go past extremities without being clamped
            m_Value = input;

            // If the stepped value doesn't match the last one, it's time to update
            if (currentValue == Value)
                return;

            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Scrollbar.value", this);
                OnValueChanged?.Invoke(Value);
            }
        }

        protected void OnRectTransformDimensionsChange()
        {
            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        Axis axis => m_Direction == Direction.LeftToRight || m_Direction == Direction.RightToLeft ? Axis.Horizontal : Axis.Vertical;
        bool reverseValue => m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom;

        // Force-update the scroll bar. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif
            m_Tracker.Clear();

            if (m_ContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                float movement = Mathf.Clamp01(Value) * (1 - m_Size);
                if (reverseValue)
                {
                    anchorMin[(int)axis] = 1 - movement - m_Size;
                    anchorMax[(int)axis] = 1 - movement;
                }
                else
                {
                    anchorMin[(int)axis] = movement;
                    anchorMax[(int)axis] = movement + m_Size;
                }

                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the scroll bar's position based on the mouse.
        void UpdateDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (m_ContainerRect == null)
                return;
            
            // Doc states: Returns true except when the drag operation is not on the same display as it originated.
            // I don't want to figure it out now and don't know if it won't break some co-op functionality,
            // so I leave it here for the future me :p.
            //
            // Vector2 position = Vector2.zero;
            // if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
            //     return;

            Vector2 position = eventData.position;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ContainerRect, position, eventData.pressEventCamera, out var localCursor))
                return;

            Vector2 handleCenterRelativeToContainerCorner = localCursor - m_Offset - m_ContainerRect.rect.position;
            Vector2 handleCorner = handleCenterRelativeToContainerCorner - (m_HandleRect.rect.size - m_HandleRect.sizeDelta) * 0.5f;

            float parentSize = axis == 0 ? m_ContainerRect.rect.width : m_ContainerRect.rect.height;
            float remainingSize = parentSize * (1 - m_Size);
            if (remainingSize <= 0)
                return;

            DoUpdateDrag(handleCorner, remainingSize);
        }

        //this function is testable, it is found using reflection in ScrollbarClamp test
        private void DoUpdateDrag(Vector2 handleCorner, float remainingSize)
        {
            switch (m_Direction)
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

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && eventData.button == PointerEventData.InputButton.Left;
        }

        /// <summary>
        /// Handling for when the scrollbar value is begin being dragged.
        /// </summary>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            isPointerDownAndNotDragging = false;

            if (!MayDrag(eventData))
                return;

            if (m_ContainerRect == null)
                return;

            m_Offset = Vector2.zero;
            if (RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos - m_HandleRect.rect.center;
            }
        }

        /// <summary>
        /// Handling for when the scrollbar value is dragged.
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            if (m_ContainerRect != null)
                UpdateDrag(eventData);
        }

        /// <summary>
        /// Event triggered when pointer is pressed down on the scrollbar.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            isPointerDownAndNotDragging = true;
            m_PointerDownRepeat = StartCoroutine(ClickRepeat(eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera));
        }

        /// <summary>
        /// Coroutine function for handling continual press during Scrollbar.OnPointerDown.
        /// </summary>
        private IEnumerator ClickRepeat(Vector2 screenPosition, Camera camera)
        {
            while (isPointerDownAndNotDragging)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, screenPosition, camera))
                {
                    Vector2 localMousePos;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, screenPosition, camera, out localMousePos))
                    {
                        var axisCoordinate = axis == 0 ? localMousePos.x : localMousePos.y;

                        // modifying value depending on direction, fixes (case 925824)

                        float change = axisCoordinate < 0 ? m_Size : -m_Size;
                        Value += reverseValue ? change : -change;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            
            StopCoroutine(m_PointerDownRepeat);
        }
        
        /// <summary>
        /// Event triggered when pointer is released after pressing on the scrollbar.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDownAndNotDragging = false;
        }

        /// <summary>
        /// See: IInitializePotentialDragHandler.OnInitializePotentialDrag
        /// </summary>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
        
        
        
        /// <summary>
        /// Setting that indicates one of four directions the scrollbar will travel.
        /// </summary>
        public enum Direction
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