using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PopupPanels
{
    public class ColorPanel : InputPanel<Color>
    {
        [SerializeField] private SatValUIController satValUIController;
        [SerializeField] private RawImage hueImage;
        [SerializeField] private RawImage satValImage;
        [SerializeField] private RawImage outputImage;
        [SerializeField] private Slider hueSlider;
        [SerializeField] private TMP_InputField hexInputField;

        private Texture2D hueTexture;
        private Texture2D satValTexture;
        private Texture2D outputTexture;
        
        private float currentHue;
        private float currentSat;
        private float currentVal;

        protected override void Awake()
        {
            base.Awake();
            CreateHueImage();
            CreateSatValImage();
            CreateOutputImage();
            satValUIController.OnValueChange += SetSatVal;
            hueSlider.onValueChanged.AddListener(UpdateHueFromSlider);
            hexInputField.onValueChanged.AddListener(UpdateColorFromHexInput);
        }
        
        protected override void SetInitialValue(Color initialValue) => UpdateTexturesAndControllers(initialValue);
        protected override void Confirm() => onConfirm.Invoke(Color.HSVToRGB(currentHue, currentSat, currentVal));

        private void CreateHueImage()
        {
            hueTexture = new Texture2D(1, 16)
            {
                wrapMode = TextureWrapMode.Clamp,
                name = "HueTexture"
            };

            for (int i = 0; i < hueTexture.height; i++)
            {
                hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 0.8f));
            }
            
            hueTexture.Apply();
            currentHue = 0;
            hueSlider.value = currentHue;

            hueImage.texture = hueTexture;
        }
        
        private void CreateSatValImage()
        {
            satValTexture = new Texture2D(16, 16)
            {
                wrapMode = TextureWrapMode.Clamp,
                name = "SatValTexture"
            };

            satValImage.texture = satValTexture;
            UpdateSatValTexture();
            
            currentSat = 0;
            currentVal = 0;
        }
        
        private void UpdateHueFromSlider(float hueValue)
        {
            currentHue = hueValue;

            UpdateSatValTexture();
            UpdateOutputTexture();
        }

        private void UpdateSatValTexture()
        {
            for (int y = 0; y < satValTexture.height; y++)
            {
                for (int x = 0; x < satValTexture.width; x++)
                {
                    satValTexture.SetPixel(x, y, Color.HSVToRGB
                    (
                        currentHue,
                        (float)x / satValTexture.width,
                        (float)y / satValTexture.height
                    ));
                }
            }
            
            satValTexture.Apply();
        }

        private void CreateOutputImage()
        {
            outputTexture = new Texture2D(1, 16)
            {
                wrapMode = TextureWrapMode.Clamp,
                name = "OutputTexture"
            };

            outputImage.texture = outputTexture;
            UpdateOutputTexture();
        }

        private void SetSatVal(Vector2 position)
        {
            currentSat = position.x;
            currentVal = position.y;
            
            UpdateOutputTexture();
        }
        
        private void UpdateColorFromHexInput(string hexColor)
        {
            if (hexColor.Length != 6) return;
            if (!HexStringToColor(hexColor, out Color color)) return;
            
            UpdateTexturesAndControllers(color);
        }

        private void UpdateTexturesAndControllers(Color color)
        {
            Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);
            UpdateSatValTexture();
            UpdateOutputTexture();
            satValUIController.UpdatePicker(new Vector2(currentSat, currentVal));
            hueSlider.value = currentHue;
        }

        private void UpdateOutputTexture()
        {
            Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);
            
            for (int i = 0; i < outputTexture.height; i++)
            {
                outputTexture.SetPixel(0, i, currentColor);
            }
            
            outputTexture.Apply();
            hexInputField.SetTextWithoutNotify(Utility.ColorToHexString(currentColor, false));
        }
        
        private static bool HexStringToColor(string input, out Color result)
        {
            result = default;
            
            if (input.StartsWith("#"))
            {
                input = input.Substring(1);
            }

            try
            {
                float red = (float)int.Parse(input.Substring(0, 2), NumberStyles.HexNumber) / 255;
                float green = (float)int.Parse(input.Substring(2, 2), NumberStyles.HexNumber) / 255;
                float blue = (float)int.Parse(input.Substring(4, 2), NumberStyles.HexNumber) / 255;
                result = new Color(red, green, blue);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }
    }
}