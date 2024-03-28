using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public abstract class SettingGroupChangeDefault<TKey, TValue> where TKey : Enum
    {
        [SerializeField] protected Entry[] entries;
        protected Dictionary<TKey, TValue> valuesLookup = new Dictionary<TKey, TValue>();

        public virtual void Initialize() => BuildLookup();
    
        private void BuildLookup()
        {
            valuesLookup = new Dictionary<TKey, TValue>();
            foreach (Entry entry in entries)
            {
                valuesLookup.Add(entry.Key, entry.Value);
            }
        }

        protected void SetLookupValues(Setting<TKey, TValue> setting)
        {
            foreach (var entry in valuesLookup)
            {
                setting.SetValue(entry.Key, entry.Value);
            }
        }

        [Serializable]
        protected class Entry
        {
            public TKey Key;
            public TValue Value;
        }
    }
}