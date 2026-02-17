using System.Collections.Generic;

namespace UI.Core.Extensions;

/// <summary>
/// Extensions for Dictionary class.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// This is a convenience method which is basically TryGetValue without the 'out' param.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="obj">The underlying dictionary object.</param>
    /// <param name="key">The key.</param>
    /// <returns>The value if found or default value otherwise.</returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> obj, TKey key) => obj.TryGetValue(key, out TValue val) ? val : default(TValue);
}