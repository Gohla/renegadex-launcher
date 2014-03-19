using System;
using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, out TValue value,
        Func<TValue> create)
    {
        if(dict.TryGetValue(key, out value))
        {
            return true;
        }
        else
        {
            value = create();
            dict.Add(key, value);
            return false;
        }
    }

    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> create)
    {
        TValue value;
        dict.TryGetValue(key, out value, create);
        return value;
    }
}