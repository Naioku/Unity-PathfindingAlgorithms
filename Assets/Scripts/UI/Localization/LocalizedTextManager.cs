using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI.Localization
{
    public class LocalizedTextManager
    {
        private readonly Dictionary<Enums.Language, string> localeIdsLookup = new()
        {
            { Enums.Language.English, "en" },
            { Enums.Language.Polish, "pl" }
        };
        
        public Enums.Language CurrentLanguage
        {
            set
            {
                LocalizationSettings localizationSettings = LocalizationSettings.Instance;
                Locale locale = null;
                
                if (value == Enums.Language.Auto)
                {
                    foreach (var selector in localizationSettings.GetStartupLocaleSelectors())
                    {
                        locale = selector.GetStartupLocale(localizationSettings.GetAvailableLocales());
                        if (locale != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    locale = localizationSettings.GetAvailableLocales().GetLocale(localeIdsLookup[value]);
                }
                
                localizationSettings.SetSelectedLocale(locale);
            }
        }

        public void Awake()
        {
            Enums.Language language = AllManagers.Instance.GameManager.GameSettings.Language;
            if (language == Enums.Language.Auto) return;
            
            CurrentLanguage = language;
        }

        public LocalizedString GetLocalizedString<T>(T name) where T : Enum =>
            new(name.GetType().Name, name.ToString());
        
        public void ChangeReference<T>(LocalizedString localizedString, T localizedTextKey) where T : Enum =>
            localizedString.SetReference(localizedTextKey.GetType().Name, localizedTextKey.ToString());

        public void ReloadLanguage() => CurrentLanguage = AllManagers.Instance.GameManager.GameSettings.Language;
    }
}