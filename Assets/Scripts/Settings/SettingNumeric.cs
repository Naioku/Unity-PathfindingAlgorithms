using System;
using UI.MenuPanels.Settings.Logic;

namespace Settings
{
    public class SettingNumeric<T> : Setting<T> where T : IComparable
    {
        public T MinValue { get; }
        public T MaxValue { get; }
        
        public override IUILogicSetting UILogicSetting => new UILogicSettingNumeric<T>();

        public SettingNumeric(T value, T minValue, T maxValue) : base(value)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}