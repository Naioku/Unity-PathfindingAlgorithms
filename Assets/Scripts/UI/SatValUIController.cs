using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SatValUIController : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        private const string PickerPath = "PICKER";
        
        private Image pickerImage;
        private RectTransform rectTransform;
        private RectTransform pickerTransform;
    
        public event Action<Vector2> OnValueChange;
    
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            
            Transform pickerTransform = transform.Find(PickerPath);
            pickerImage = pickerTransform.GetComponent<Image>();
            this.pickerTransform = pickerTransform.GetComponent<RectTransform>();
            
            InitializePickerPosition();
        }

        public void UpdatePicker(Vector2 positionNormalized)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 halfSize = size * 0.5f;
            Vector2 position = new Vector2(positionNormalized.x * size.x - halfSize.x, positionNormalized.y * size.y - halfSize.y);
            UpdatePicker(position, positionNormalized);
        }
    
        public void OnDrag(PointerEventData eventData) => UpdateColor(eventData);
        public void OnPointerDown(PointerEventData eventData) => UpdateColor(eventData);
    
        private void InitializePickerPosition()
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 halfSize = size * 0.5f;

            pickerTransform.localPosition = new Vector2(-halfSize.x, -halfSize.y);
        }
    
        private void UpdateColor(PointerEventData eventData)
        {
            Vector2 position = rectTransform.InverseTransformPoint(eventData.position);
            Vector2 size = rectTransform.rect.size;
            Vector2 halfSize = size * 0.5f;
    
            position.x = Mathf.Clamp(position.x, -halfSize.x, halfSize.x);
            position.y = Mathf.Clamp(position.y, -halfSize.y, halfSize.y);
    
            Vector2 positionNormalized = new Vector2((position.x + halfSize.x) / size.x, (position.y + halfSize.y) / size.y);
            UpdatePicker(position, positionNormalized);
            OnValueChange?.Invoke(positionNormalized);
        }
    
        private void UpdatePicker(Vector2 position, Vector2 positionNormalized)
        {
            pickerTransform.localPosition = position;
            pickerImage.color = Color.HSVToRGB(0, 0, 1 - positionNormalized.y);
        }
    }
}