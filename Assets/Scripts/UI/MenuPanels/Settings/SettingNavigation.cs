using UI.MenuPanels.Settings.SettingEntries;

namespace UI.MenuPanels.Settings
{
    public struct SettingNavigation
    {
        public ISettingEntry OnUp;
        public ISettingEntry OnDown;
        public ISettingEntry OnLeft;
        public ISettingEntry OnRight;
    }
}