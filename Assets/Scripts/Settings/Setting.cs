﻿using UI.MenuPanels.Settings.Logic;
using Utilities;

namespace Settings
{
    public class Setting<T> : ISetting
    {
        private T value;

        public T Value => value;
        
        public object SerializableValue
        {
            get => typeof(T).IsSerializable ? value : Utility.ConvertToSerializableValue(value);
            set => this.value = typeof(T).IsSerializable ? (T)value : Utility.ConvertFromSerializableValue<T>(value);
        }
        
        public virtual IUILogicSetting UILogicSetting => new UILogicSetting<T>();
        
        public void SetValue(IUILogicSetting uiLogicSetting) => value = ((UILogicSetting<T>)uiLogicSetting).Value;

        public Setting(T value) => this.value = value;
    }
}