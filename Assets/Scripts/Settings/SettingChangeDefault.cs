using System;

namespace Settings
{
    public class SettingChangeDefault<TKey, TValue> : SettingChange<TKey, TValue> where TKey : Enum
    {
        protected void SetLookupValues(Setting<TKey, TValue> setting)
        {
            foreach (var entry in valuesLookup)
            {
                setting.SetValue(entry.Key, entry.Value);
            }
        }
    }
}