using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI.Localization
{
    public class LocalizedTextManager
    {
        private readonly Dictionary<Languages, string> localeIdsLookup = new()
        {
            { Languages.English, "en" },
            { Languages.Polish, "pl" }
        };
        
        public Languages CurrentLanguage
        {
            set
            {
                LocalizationSettings localizationSettings = LocalizationSettings.Instance;
                localizationSettings.SetSelectedLocale(localizationSettings.GetAvailableLocales().GetLocale(localeIdsLookup[value]));
            }
        }

        public LocalizedString GetLocalizedString<T>(T name) where T : Enum =>
            new(name.GetType().Name, name.ToString());
        
        public void ChangeReference<T>(LocalizedString localizedString, T localizedTextKey) where T : Enum =>
            localizedString.SetReference(localizedTextKey.GetType().Name, localizedTextKey.ToString());

        public enum Languages
        {
            English,
            Polish
        }
    }
}