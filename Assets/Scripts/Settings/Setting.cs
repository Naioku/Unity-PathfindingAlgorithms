using UI.MenuPanels.Settings.Logic;

namespace Settings
{
    public class Setting<T> : ISetting
    {
        private T value;

        public T Value => value;
        public object SerializableValue
        {
            get => typeof(T).IsSerializable ? value : Utility.Utility.ConvertToSerializableValue(value);
            set => this.value = typeof(T).IsSerializable ? (T)value : Utility.Utility.ConvertFromSerializableValue<T>(value);
        }
        
        public IUILogicSetting UILogicSetting => new UILogicSetting<T>();
        
        public void SetValue(IUILogicSetting uiLogicSetting) => value = ((UILogicSetting<T>)uiLogicSetting).Value;

        public Setting(Enums.SettingName name, T value) => this.value = value;
    }
}