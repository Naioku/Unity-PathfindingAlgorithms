namespace UI.MenuPanels.Settings
{
    public interface ISettingEntry
    {
        public bool ChangedThroughPopup { get; }
        UISetting UISetting { get; }
        public UISetting InitializeUI();
        public void SetNavigation(SettingNavigation navigation);
    }
}