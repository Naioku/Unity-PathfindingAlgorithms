using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace UI.Buttons
{
    public class TaggedButtonLocalized<T> : ButtonLocalized where T : Enum
    {
        [SerializeField] private T buttonTag;
        
        public T Tag => buttonTag;

        public void Initialize(params object[] arguments) => base.Initialize(buttonTag, arguments);
        public void Initialize(params KeyValuePair<string, IVariable>[] persistentArguments) => base.Initialize(buttonTag, persistentArguments);
    }
}