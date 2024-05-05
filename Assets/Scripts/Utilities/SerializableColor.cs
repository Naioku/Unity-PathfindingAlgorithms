using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public struct SerializableColor : ISerializableValue<Color>
    {
        private float r;
        private float g;
        private float b;
        private float a;

        public SerializableColor(Color color)
        {
            r = color.r;
            b = color.b;
            g = color.g;
            a = color.a;
        }

        public Color GetValue() => new(r, g, b, a);
    }
}