using System;

namespace Utility
{
    public static class Extensions
    {
        public static T GetValue<T>(this Enum enumeration) => (T)Convert.ChangeType(enumeration, typeof(T));
    }
}