using System;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace UI.HUDPanels
{
    public struct ButtonDataTagged<T>
    {
        public T Tag;
        public Action Action;
        public StringVariable Binding;
    }
}