using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public abstract class SettingChange<TKey, TValue> where TKey : Enum
    {
        [SerializeField] protected Entry[] entries;
        protected Dictionary<TKey, TValue> valuesLookup = new Dictionary<TKey, TValue>();

        public void Initialize() => BuildLookup();
    
        private void BuildLookup()
        {
            valuesLookup = new Dictionary<TKey, TValue>();
            foreach (Entry entry in entries)
            {
                valuesLookup.Add(entry.Key, entry.Value);
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