using System;
using System.Collections.Generic;

namespace Settings
{
    public class Setting<TKey, TValue> where TKey : Enum
    {
        private readonly Dictionary<TKey, TValue> valuesLookup = new Dictionary<TKey, TValue>();
    
        public TValue GetValue(TKey tileType) => valuesLookup[tileType];
        public void SetValue(TKey tileType, TValue color) => valuesLookup[tileType] = color;
    }
}