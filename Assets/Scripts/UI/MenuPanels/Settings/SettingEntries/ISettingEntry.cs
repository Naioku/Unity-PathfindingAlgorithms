using UI.MenuPanels.Settings.View;

namespace UI.MenuPanels.Settings.SettingEntries
{
    public interface ISettingEntry
    {
        public bool ChangedThroughPopup { get; }
        ViewSetting ViewSetting { get; }
        public ViewSetting InitializeUI();
        public void SetNavigation(SettingNavigation navigation);
    }
}