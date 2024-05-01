using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Utility;

namespace UI.Localization
{
    public class LocalizedContentCache
    {
        private readonly Dictionary<Type, Dictionary<int, LocalizedString>> content = new();

        public string GetValue(Enum name) => content[name.GetType()][name.GetValue<int>()].GetLocalizedString();

        public LocalizedContentCache(params Enum[] names)
        {
            foreach (Enum name in names)
            {
                Type nameType = name.GetType();
                if (content.TryGetValue(nameType, out Dictionary<int, LocalizedString> value))
                {
                    value.Add(name.GetValue<int>(), AllManagers.Instance.LocalizedTextManager.GetLocalizedString(name));
                }
                else
                {
                    content.Add(nameType, new Dictionary<int, LocalizedString>
                    {
                        { name.GetValue<int>(), AllManagers.Instance.LocalizedTextManager.GetLocalizedString(name) }
                    });
                }
            }
        }
    }
}