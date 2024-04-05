using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Unity <see cref="UnityEngine.UI.ScrollRect"/> adjusted to the <see cref="Scrollbar"/> and without IMHO unnecessary stuff.
    /// </summary>
    [AddComponentMenu("Naioku/UI/Naioku Scroll Rect")]
    [SelectionBase]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ScrollRect :
        UIBehaviour,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IScrollHandler,
        ICanvasElement,
        ILayoutElement,
        ILayoutGroup
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private bool horizontal = true;
        [SerializeField] private bool vertical = true;
        [SerializeField] private UnityEngine.UI.ScrollRect.MovementType movementType = UnityEngine.UI.ScrollRect.MovementType.Elastic;
        [SerializeField] private float elasticity = 0.1f;
        [SerializeField] private bool inertia = true;
        [SerializeField] private float decelerationRate = 0.135f; // Only used when inertia is enabled
        [SerializeField] private float scrollSensitivity = 1.0f;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private Scrollbar horizontalScrollbar;
        [SerializeField] private Scrollbar verticalScrollbar;
        [SerializeField] private UnityEngine.UI.ScrollRect.ScrollbarVisibility verticalScrollbarVisibility;
        [SerializeField] private UnityEngine.UI.ScrollRect.ScrollbarVisibility horizontalScrollbarVisibility;
        [SerializeField] private float horizontalScrollbarSpacing;
        [SerializeField] private float verticalScrollbarSpacing;

        private readonly Vector3[] corners = new Vector3[4];

        // The offset from handle position to mouse down position
        private Vector2 pointerStartLocalCursor = Vector2.zero;
        private Vector2 contentStartPosition = Vector2.zero;
        private Bounds contentBounds;
        private Bounds viewBounds;
        private Vector2 velocity;
        private bool dragging;
        private bool scrolling;
        private Vector2 prevPosition = Vector2.zero;
        private Bounds prevContentBounds;
        private Bounds prevViewBounds;
        [NonSerialized] private bool hasRebuiltLayout;
        private bool hSliderExpand;
        private bool vSliderExpand;
        private float hSliderHeight;
        private float vSliderWidth;
        private RectTransform viewRect;
        [NonSerialized] private RectTransform rectTransform;
        private RectTransform horizontalScrollbarRect;
        private RectTransform verticalScrollbarRect;

        /// <summary>
        /// The scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.
        /// </summary>
        public Vector2 NormalizedPosition => new Vector2(HorizontalNormalizedPosition, VerticalNormalizedPosition);

        /// <summary>
        /// The horizontal scroll position as a value between 0 and 1, with 0 being at the left.
        /// </summary>
        public float HorizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if (contentBounds.size.x <= viewBounds.size.x || Mathf.Approximately(contentBounds.size.x, viewBounds.size.x))
                {
                    return viewBounds.min.x > contentBounds.min.x ? 1 : 0;
                }

                return (viewBounds.min.x - contentBounds.min.x) / (contentBounds.size.x - viewBounds.size.x);
            }
        }

        /// <summary>
        /// The vertical scroll position as a value between 0 and 1, with 0 being at the bottom.
        /// </summary>
        public float VerticalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if (contentBounds.size.y <= viewBounds.size.y || Mathf.Approximately(contentBounds.size.y, viewBounds.size.y))
                {
                    return viewBounds.min.y > contentBounds.min.y ? 1 : 0;
                }

                return (viewBounds.min.y - contentBounds.min.y) / (contentBounds.size.y - viewBounds.size.y);
            }
        }

        private bool HScrollingNeeded => !Application.isPlaying || contentBounds.size.x > viewBounds.size.x + 0.01f;
        private bool VScrollingNeeded => !Application.isPlaying || contentBounds.size.y > viewBounds.size.y + 0.01f;

        private RectTransform ViewRect
        {
            get
            {
                if (viewRect == null)
                {
                    viewRect = viewport;
                }

                if (viewRect == null)
                {
                    viewRect = (RectTransform)transform;
                }

                return viewRect;
            }
        }

        private RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        public event Action<Vector2> OnValueChanged;

        /// <summary>
        /// Sets the velocity to zero on both axes so the content stops moving.
        /// </summary>
        public virtual void StopMovement()
        {
            velocity = Vector2.zero;
        }

        /// <inheritdoc cref="UIBehaviour.IsActive()"/>
        public override bool IsActive() => base.IsActive() && content != null;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (horizontalScrollbar)
            {
                horizontalScrollbar.OnValueChanged += SetHorizontalNormalizedPosition;
            }

            if (verticalScrollbar)
            {
                verticalScrollbar.OnValueChanged += SetVerticalNormalizedPosition;
            }

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            SetDirty();
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            if (horizontalScrollbar)
            {
                horizontalScrollbar.OnValueChanged -= SetHorizontalNormalizedPosition;
            }

            if (verticalScrollbar)
            {
                verticalScrollbar.OnValueChanged -= SetVerticalNormalizedPosition;
            }

            dragging = false;
            scrolling = false;
            hasRebuiltLayout = false;
            velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange() => SetDirty();

#if UNITY_EDITOR
        protected override void OnValidate() => SetDirtyCaching();
#endif

        protected virtual void LateUpdate()
        {
            if (!content) return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();
            float deltaTime = Time.unscaledDeltaTime;
            Vector2 offset = CalculateOffset(Vector2.zero);
            if (!dragging && (offset != Vector2.zero || velocity != Vector2.zero))
            {
                Vector2 position = content.anchoredPosition;
                for (int axis = 0; axis < 2; axis++)
                {
                    // Apply spring physics if movement is elastic and content has an offset from the view.
                    if (movementType == UnityEngine.UI.ScrollRect.MovementType.Elastic && offset[axis] != 0)
                    {
                        float speed = velocity[axis];
                        float smoothTime = elasticity;
                        if (scrolling)
                            smoothTime *= 3.0f;
                        position[axis] = Mathf.SmoothDamp(content.anchoredPosition[axis], content.anchoredPosition[axis] + offset[axis], ref speed, smoothTime, Mathf.Infinity,
                            deltaTime);
                        if (Mathf.Abs(speed) < 1)
                            speed = 0;
                        velocity[axis] = speed;
                    }
                    // Else move content according to velocity with deceleration applied.
                    else if (inertia)
                    {
                        velocity[axis] *= Mathf.Pow(decelerationRate, deltaTime);
                        if (Mathf.Abs(velocity[axis]) < 1)
                            velocity[axis] = 0;
                        position[axis] += velocity[axis] * deltaTime;
                    }
                    // If we have neither elaticity or friction, there shouldn't be any velocity.
                    else
                    {
                        velocity[axis] = 0;
                    }
                }

                if (movementType == UnityEngine.UI.ScrollRect.MovementType.Clamped)
                {
                    offset = CalculateOffset(position - content.anchoredPosition);
                    position += offset;
                }

                SetContentAnchoredPosition(position);
            }

            if (dragging && inertia)
            {
                Vector3 newVelocity = (content.anchoredPosition - prevPosition) / deltaTime;
                velocity = Vector3.Lerp(velocity, newVelocity, deltaTime * 10);
            }

            if (viewBounds != prevViewBounds || contentBounds != prevContentBounds || content.anchoredPosition != prevPosition)
            {
                UpdateScrollbars(offset);
                UISystemProfilerApi.AddMarker("ScrollRect.value", this);
                OnValueChanged?.Invoke(NormalizedPosition);
                UpdatePrevData();
            }

            UpdateScrollbarVisibility();
            scrolling = false;
        }

        private void SetHorizontalNormalizedPosition(float value) => SetNormalizedPosition(value, 0);
        private void SetVerticalNormalizedPosition(float value) => SetNormalizedPosition(value, 1);

        /// <summary>
        /// Set the horizontal or vertical scroll position as a value between 0 and 1, with 0 being at the left or at the bottom.
        /// </summary>
        /// <param name="value">The position to set, between 0 and 1.</param>
        /// <param name="axis">The axis to set: 0 for horizontal, 1 for vertical.</param>
        private void SetNormalizedPosition(float value, int axis)
        {
            EnsureLayoutHasRebuilt();
            UpdateBounds();
            // How much the content is larger than the view.
            float hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
            // Where the position of the lower left corner of the content bounds should be, in the space of the view.
            float contentBoundsMinPosition = viewBounds.min[axis] - value * hiddenLength;
            // The new content localPosition, in the space of the view.
            float newAnchoredPosition = content.anchoredPosition[axis] + contentBoundsMinPosition - contentBounds.min[axis];

            Vector3 anchoredPosition = content.anchoredPosition;
            if (Mathf.Abs(anchoredPosition[axis] - newAnchoredPosition) > 0.01f)
            {
                anchoredPosition[axis] = newAnchoredPosition;
                content.anchoredPosition = anchoredPosition;
                velocity[axis] = 0;
                UpdateBounds();
            }
        }

        private void EnsureLayoutHasRebuilt()
        {
            if (hasRebuiltLayout || CanvasUpdateRegistry.IsRebuildingLayout()) return;

            Canvas.ForceUpdateCanvases();
        }

        /// <summary>
        /// Override to alter or add to the code that keeps the appearance of the scroll rect synced with its data.
        /// </summary>
        private void SetDirty()
        {
            if (!IsActive()) return;

            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }

        /// <summary>
        /// Override to alter or add to the code that caches data to avoid repeated heavy operations.
        /// </summary>
        private void SetDirtyCaching()
        {
            if (!IsActive()) return;

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);

            viewRect = null;
        }

        private void UpdateScrollbarVisibility()
        {
            UpdateOneScrollbarVisibility(VScrollingNeeded, vertical, verticalScrollbarVisibility, verticalScrollbar);
            UpdateOneScrollbarVisibility(HScrollingNeeded, horizontal, horizontalScrollbarVisibility, horizontalScrollbar);
        }

        private static void UpdateOneScrollbarVisibility(bool xScrollingNeeded, bool xAxisEnabled, UnityEngine.UI.ScrollRect.ScrollbarVisibility scrollbarVisibility,
            Scrollbar scrollbar)
        {
            if (scrollbar)
            {
                if (scrollbarVisibility == UnityEngine.UI.ScrollRect.ScrollbarVisibility.Permanent)
                {
                    if (scrollbar.gameObject.activeSelf != xAxisEnabled)
                        scrollbar.gameObject.SetActive(xAxisEnabled);
                }
                else
                {
                    if (scrollbar.gameObject.activeSelf != xScrollingNeeded)
                        scrollbar.gameObject.SetActive(xScrollingNeeded);
                }
            }
        }

        /// <summary>
        /// Sets the anchored position of the content.
        /// </summary>
        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            if (!horizontal)
            {
                position.x = content.anchoredPosition.x;
            }

            if (!vertical)
            {
                position.y = content.anchoredPosition.y;
            }

            if (position != content.anchoredPosition)
            {
                content.anchoredPosition = position;
                UpdateBounds();
            }
        }

        /// <summary>
        /// Helper function to update the previous data fields on a ScrollRect. Call this before you change data in the ScrollRect.
        /// </summary>
        private void UpdatePrevData()
        {
            prevPosition = content == null ? Vector2.zero : content.anchoredPosition;
            prevViewBounds = viewBounds;
            prevContentBounds = contentBounds;
        }

        private void UpdateScrollbars(Vector2 offset)
        {
            if (horizontalScrollbar)
            {
                if (contentBounds.size.x > 0)
                {
                    horizontalScrollbar.Size = Mathf.Clamp01((viewBounds.size.x - Mathf.Abs(offset.x)) / contentBounds.size.x);
                }
                else
                {
                    horizontalScrollbar.Size = 1;
                }

                horizontalScrollbar.Value = HorizontalNormalizedPosition;
            }

            if (verticalScrollbar)
            {
                if (contentBounds.size.y > 0)
                {
                    verticalScrollbar.Size = Mathf.Clamp01((viewBounds.size.y - Mathf.Abs(offset.y)) / contentBounds.size.y);
                }
                else
                {
                    verticalScrollbar.Size = 1;
                }

                verticalScrollbar.Value = VerticalNormalizedPosition;
            }
        }

#region IHandlers
        public virtual void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive. // Todo: You can change it in the future xd.
            delta.y *= -1;
            if (vertical && !horizontal)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    delta.y = delta.x;
                }

                delta.x = 0;
            }

            if (horizontal && !vertical)
            {
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                {
                    delta.x = delta.y;
                }

                delta.y = 0;
            }

            if (data.IsScrolling())
            {
                scrolling = true;
            }

            Vector2 position = content.anchoredPosition;
            position += delta * scrollSensitivity;
            if (movementType == UnityEngine.UI.ScrollRect.MovementType.Clamped)
            {
                position += CalculateOffset(position - content.anchoredPosition);
            }

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            velocity = Vector2.zero;
        }

        /// <summary>
        /// Handling for when the content dragging is started.
        /// </summary>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!IsActive()) return;

            UpdateBounds();

            pointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewRect, eventData.position, eventData.pressEventCamera, out pointerStartLocalCursor);
            contentStartPosition = content.anchoredPosition;
            dragging = true;
        }

        /// <summary>
        /// Handling for when the content has finished being dragged.
        /// </summary>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            dragging = false;
        }

        /// <summary>
        /// Handling for when the content is dragged.
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!IsActive()) return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    ViewRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localCursor)) return;

            UpdateBounds();

            var pointerDelta = localCursor - pointerStartLocalCursor;
            Vector2 position = contentStartPosition + pointerDelta;

            // Offset to get content into place in the view.
            Vector2 offset = CalculateOffset(position - content.anchoredPosition);
            position += offset;
            if (movementType == UnityEngine.UI.ScrollRect.MovementType.Elastic)
            {
                if (offset.x != 0)
                {
                    position.x -= RubberDelta(offset.x, viewBounds.size.x);
                }

                if (offset.y != 0)
                {
                    position.y -= RubberDelta(offset.y, viewBounds.size.y);
                }
            }

            SetContentAnchoredPosition(position);
        }

        private static float RubberDelta(float overStretching, float viewSize) =>
            (1 - 1 / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1)) * viewSize * Mathf.Sign(overStretching);
#endregion

#region ILayoutElement
        public virtual void CalculateLayoutInputHorizontal() {}
        public virtual void CalculateLayoutInputVertical() {}
        public virtual float minWidth => -1;
        public virtual float preferredWidth => -1;
        public virtual float flexibleWidth => -1;
        public virtual float minHeight => -1;
        public virtual float preferredHeight => -1;
        public virtual float flexibleHeight => -1;
        public virtual int layoutPriority => -1;
#endregion

#region ICanvasElement
        /// <summary>
        /// Rebuilds the scroll rect data after initialization.
        /// </summary>
        /// <param name="executing">The current step in the rendering CanvasUpdate cycle.</param>
        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                UpdateCachedData();
            }

            if (executing == CanvasUpdate.PostLayout)
            {
                UpdateBounds();
                UpdateScrollbars(Vector2.zero);
                UpdatePrevData();

                hasRebuiltLayout = true;
            }
        }

        public virtual void LayoutComplete() {}
        public virtual void GraphicUpdateComplete() {}
#endregion

#region ILayoutGroup
        public virtual void SetLayoutHorizontal()
        {
            UpdateCachedData();

            Rect rect = ViewRect.rect;
            if (hSliderExpand || vSliderExpand)
            {
                // Make view full size to see if content fits.
                ViewRect.anchorMin = Vector2.zero;
                ViewRect.anchorMax = Vector2.one;
                ViewRect.sizeDelta = Vector2.zero;
                ViewRect.anchoredPosition = Vector2.zero;

                // Recalculate content layout with this size to see if it fits when there are no scrollbars.
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                viewBounds = new Bounds(rect.center, rect.size);
                contentBounds = GetBounds();
            }

            // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
            if (vSliderExpand && VScrollingNeeded)
            {
                ViewRect.sizeDelta = new Vector2(-(vSliderWidth + verticalScrollbarSpacing), ViewRect.sizeDelta.y);

                // Recalculate content layout with this size to see if it fits vertically
                // when there is a vertical scrollbar (which may reflowed the content to make it taller).
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                viewBounds = new Bounds(rect.center, rect.size);
                contentBounds = GetBounds();
            }

            // If it doesn't fit horizontally, enable horizontal scrollbar and shrink view vertically to make room for it.
            if (hSliderExpand && HScrollingNeeded)
            {
                ViewRect.sizeDelta = new Vector2(ViewRect.sizeDelta.x, -(hSliderHeight + horizontalScrollbarSpacing));
                viewBounds = new Bounds(rect.center, rect.size);
                contentBounds = GetBounds();
            }

            // If the vertical slider didn't kick in the first time, and the horizontal one did,
            // we need to check again if the vertical slider now needs to kick in.
            // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
            if (vSliderExpand && VScrollingNeeded && ViewRect.sizeDelta.x == 0 && ViewRect.sizeDelta.y < 0)
            {
                ViewRect.sizeDelta = new Vector2(-(vSliderWidth + verticalScrollbarSpacing), ViewRect.sizeDelta.y);
            }
        }

        public virtual void SetLayoutVertical()
        {
            UpdateScrollbarLayout();
            viewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
            contentBounds = GetBounds();
        }
#endregion

        private void UpdateCachedData()
        {
            Transform transform = this.transform;
            horizontalScrollbarRect = horizontalScrollbar == null ? null : horizontalScrollbar.transform as RectTransform;
            verticalScrollbarRect = verticalScrollbar == null ? null : verticalScrollbar.transform as RectTransform;

            // These are true if either the elements are children, or they don't exist at all.
            bool viewIsChild = ViewRect.parent == transform;
            bool hScrollbarIsChild = !horizontalScrollbarRect || horizontalScrollbarRect.parent == transform;
            bool vScrollbarIsChild = !verticalScrollbarRect || verticalScrollbarRect.parent == transform;
            bool allAreChildren = viewIsChild && hScrollbarIsChild && vScrollbarIsChild;

            hSliderExpand = allAreChildren && horizontalScrollbarRect && horizontalScrollbarVisibility == UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            vSliderExpand = allAreChildren && verticalScrollbarRect && verticalScrollbarVisibility == UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            hSliderHeight = horizontalScrollbarRect == null ? 0 : horizontalScrollbarRect.rect.height;
            vSliderWidth = verticalScrollbarRect == null ? 0 : verticalScrollbarRect.rect.width;
        }

        private void UpdateScrollbarLayout()
        {
            if (vSliderExpand && horizontalScrollbar)
            {
                horizontalScrollbarRect.anchorMin = new Vector2(0, horizontalScrollbarRect.anchorMin.y);
                horizontalScrollbarRect.anchorMax = new Vector2(1, horizontalScrollbarRect.anchorMax.y);
                horizontalScrollbarRect.anchoredPosition = new Vector2(0, horizontalScrollbarRect.anchoredPosition.y);
                if (VScrollingNeeded)
                    horizontalScrollbarRect.sizeDelta = new Vector2(-(vSliderWidth + verticalScrollbarSpacing), horizontalScrollbarRect.sizeDelta.y);
                else
                    horizontalScrollbarRect.sizeDelta = new Vector2(0, horizontalScrollbarRect.sizeDelta.y);
            }

            if (hSliderExpand && verticalScrollbar)
            {
                verticalScrollbarRect.anchorMin = new Vector2(verticalScrollbarRect.anchorMin.x, 0);
                verticalScrollbarRect.anchorMax = new Vector2(verticalScrollbarRect.anchorMax.x, 1);
                verticalScrollbarRect.anchoredPosition = new Vector2(verticalScrollbarRect.anchoredPosition.x, 0);
                if (HScrollingNeeded)
                    verticalScrollbarRect.sizeDelta = new Vector2(verticalScrollbarRect.sizeDelta.x, -(hSliderHeight + horizontalScrollbarSpacing));
                else
                    verticalScrollbarRect.sizeDelta = new Vector2(verticalScrollbarRect.sizeDelta.x, 0);
            }
        }

        /// <summary>
        /// Calculate the bounds the ScrollRect should be using.
        /// </summary>
        private void UpdateBounds()
        {
            viewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
            contentBounds = GetBounds();

            if (content == null)
                return;

            Vector3 contentSize = contentBounds.size;
            Vector3 contentPos = contentBounds.center;
            var contentPivot = content.pivot;
            AdjustBounds(ref viewBounds, ref contentPivot, ref contentSize, ref contentPos);
            contentBounds.size = contentSize;
            contentBounds.center = contentPos;

            if (movementType == UnityEngine.UI.ScrollRect.MovementType.Clamped)
            {
                // Adjust content so that content bounds bottom (right side) is never higher (to the left) than the view bounds bottom (right side).
                // top (left side) is never lower (to the right) than the view bounds top (left side).
                // All this can happen if content has shrunk.
                // This works because content size is at least as big as view size (because of the call to InternalUpdateBounds above).
                Vector2 delta = Vector2.zero;
                if (viewBounds.max.x > contentBounds.max.x)
                {
                    delta.x = Math.Min(viewBounds.min.x - contentBounds.min.x, viewBounds.max.x - contentBounds.max.x);
                }
                else if (viewBounds.min.x < contentBounds.min.x)
                {
                    delta.x = Math.Max(viewBounds.min.x - contentBounds.min.x, viewBounds.max.x - contentBounds.max.x);
                }

                if (viewBounds.min.y < contentBounds.min.y)
                {
                    delta.y = Math.Max(viewBounds.min.y - contentBounds.min.y, viewBounds.max.y - contentBounds.max.y);
                }
                else if (viewBounds.max.y > contentBounds.max.y)
                {
                    delta.y = Math.Min(viewBounds.min.y - contentBounds.min.y, viewBounds.max.y - contentBounds.max.y);
                }

                if (delta.sqrMagnitude > float.Epsilon)
                {
                    contentPos = content.anchoredPosition + delta;
                    if (!horizontal)
                        contentPos.x = content.anchoredPosition.x;
                    if (!vertical)
                        contentPos.y = content.anchoredPosition.y;
                    AdjustBounds(ref viewBounds, ref contentPivot, ref contentSize, ref contentPos);
                }
            }
        }

        private static void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos)
        {
            // Make sure content bounds are at least as large as view by adding padding if not.
            // One might think at first that if the content is smaller than the view, scrolling should be allowed.
            // However, that's not how scroll views normally work.
            // Scrolling is *only* possible when content is *larger* than view.
            // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
            // E.g. if pivot is at top, bounds are expanded downwards.
            // This also works nicely when ContentSizeFitter is used on the content.
            Vector3 excess = viewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (contentPivot.x - 0.5f);
                contentSize.x = viewBounds.size.x;
            }

            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (contentPivot.y - 0.5f);
                contentSize.y = viewBounds.size.y;
            }
        }

        private Bounds GetBounds()
        {
            if (content == null)
            {
                return new Bounds();
            }

            content.GetWorldCorners(corners);
            Matrix4x4 viewWorldToLocalMatrix = ViewRect.worldToLocalMatrix;
            return InternalGetBounds(corners, ref viewWorldToLocalMatrix);
        }

        private static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            Bounds bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Vector2 CalculateOffset(Vector2 delta) =>
            InternalCalculateOffset
            (
                ref viewBounds,
                ref contentBounds,
                horizontal,
                vertical,
                movementType,
                ref delta
            );

        private static Vector2 InternalCalculateOffset(ref Bounds viewBounds, ref Bounds contentBounds, bool horizontal, bool vertical,
            UnityEngine.UI.ScrollRect.MovementType movementType, ref Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            if (movementType == UnityEngine.UI.ScrollRect.MovementType.Unrestricted)
            {
                return offset;
            }

            Vector2 min = contentBounds.min;
            Vector2 max = contentBounds.max;

            // min/max offset extracted to check if approximately 0 and avoid recalculating layout every frame (case 1010178)

            if (horizontal)
            {
                min.x += delta.x;
                max.x += delta.x;

                float maxOffset = viewBounds.max.x - max.x;
                float minOffset = viewBounds.min.x - min.x;

                if (minOffset < -0.001f)
                {
                    offset.x = minOffset;
                }
                else if (maxOffset > 0.001f)
                {
                    offset.x = maxOffset;
                }
            }

            if (vertical)
            {
                min.y += delta.y;
                max.y += delta.y;

                float maxOffset = viewBounds.max.y - max.y;
                float minOffset = viewBounds.min.y - min.y;

                if (maxOffset > 0.001f)
                {
                    offset.y = maxOffset;
                }
                else if (minOffset < -0.001f)
                {
                    offset.y = minOffset;
                }
            }

            return offset;
        }

        /// <summary>
        /// A setting for which behavior to use when content moves beyond the confines of its container.
        /// </summary>
        public enum MovementType
        {
            /// <summary>
            /// Unrestricted movement. The content can move forever.
            /// </summary>
            Unrestricted,

            /// <summary>
            /// Elastic movement. The content is allowed to temporarily move beyond the container, but is pulled back elastically.
            /// </summary>
            Elastic,

            /// <summary>
            /// Clamped movement. The content can not be moved beyond its container.
            /// </summary>
            Clamped,
        }

        /// <summary>
        /// Enum for which behavior to use for scrollbar visibility.
        /// </summary>
        public enum ScrollbarVisibility
        {
            /// <summary>
            /// Always show the scrollbar.
            /// </summary>
            Permanent,

            /// <summary>
            /// Automatically hide the scrollbar when no scrolling is needed on this axis. The viewport rect will not be changed.
            /// </summary>
            AutoHide,

            /// <summary>
            /// Automatically hide the scrollbar when no scrolling is needed on this axis, and expand the viewport rect accordingly.
            /// </summary>
            /// <remarks>
            /// When this setting is used, the scrollbar and the viewport rect become driven, meaning that values in the RectTransform are calculated automatically and can't be manually edited.
            /// </remarks>
            AutoHideAndExpandViewport,
        }
    }
}