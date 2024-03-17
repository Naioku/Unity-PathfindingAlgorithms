using System;
using UnityEngine;

namespace UI.Buttons
{
    public class TaggedButton<T> : Button where T : Enum
    {
        [SerializeField] private T buttonTag;
        
        public T Tag => buttonTag;
    }
}