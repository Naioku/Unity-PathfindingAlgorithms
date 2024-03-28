using System;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public abstract class SettingGroupChangeInGame<TKey, TValue> where TKey : Enum where TValue : new()
    {
        [SerializeField] protected TKey[] keysOrder;
        protected Dictionary<TKey, TValue> valuesLookup = new Dictionary<TKey, TValue>();

        public virtual void Initialize() => BuildLookup();
    
        private void BuildLookup()
        {
            valuesLookup = new Dictionary<TKey, TValue>();
            foreach (TKey key in keysOrder)
            {
                valuesLookup.Add(key, new TValue());
            }
        }
    }
}