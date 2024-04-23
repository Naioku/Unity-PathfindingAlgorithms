using UI.MenuPanels.Settings.Logic;

namespace Settings
{
    public interface ISetting
    {
        object SerializableValue { get; set; }
        IUILogicSetting UILogicSetting { get; }
        void SetValue(IUILogicSetting uiLogicSetting);
    }
}