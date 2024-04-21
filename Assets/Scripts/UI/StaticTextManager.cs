using System;
using System.Collections.Generic;

namespace UI
{
    public class StaticTextManager
    {
        private readonly Dictionary<Enums.SettingName, Dictionary<Languages, string>> settingNameEntries = new Dictionary<Enums.SettingName, Dictionary<Languages, string>>();
        private readonly Dictionary<Enums.SettingGroupStaticKey, Dictionary<Languages, string>> settingGroupNameEntries = new Dictionary<Enums.SettingGroupStaticKey, Dictionary<Languages, string>>();
        private readonly Dictionary<Enums.SettingGroupPanelStaticKey, Dictionary<Languages, string>> settingGroupPanelNameEntries = new Dictionary<Enums.SettingGroupPanelStaticKey, Dictionary<Languages, string>>();

        private Languages currentLanguage = Languages.English;
        
        public void Awake()
        {
            // Todo: It's temporary solution. Change it to the plugin or simply share in the inspector.
            foreach (Enums.SettingName name in Enum.GetValues(typeof(Enums.SettingName)))
            {
                settingNameEntries.Add(name, new Dictionary<Languages, string> {{ Languages.English, name.ToString() }});
            }
            
            // Todo: It's temporary solution. Change it to the plugin or simply share in the inspector.
            foreach (Enums.SettingGroupStaticKey name in Enum.GetValues(typeof(Enums.SettingGroupStaticKey)))
            {
                settingGroupNameEntries.Add(name, new Dictionary<Languages, string> {{ Languages.English, name.ToString() }});
            }
            
            // Todo: It's temporary solution. Change it to the plugin or simply share in the inspector.
            foreach (Enums.SettingGroupPanelStaticKey name in Enum.GetValues(typeof(Enums.SettingGroupPanelStaticKey)))
            {
                settingGroupPanelNameEntries.Add(name, new Dictionary<Languages, string> {{ Languages.English, name.ToString() }});
            }
        }

        public string GetValue(Enums.SettingName key) => settingNameEntries[key][currentLanguage];
        public string GetValue(Enums.SettingGroupStaticKey key) => settingGroupNameEntries[key][currentLanguage];
        public string GetValue(Enums.SettingGroupPanelStaticKey key) => settingGroupPanelNameEntries[key][currentLanguage];

        // private class TextEntry<T> where T : Enum
        // {
        //     public T Name;
        //     public string ;
        // }
        
        private enum Languages
        {
            English,
            Polish
        }
    }
}