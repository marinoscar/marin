using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common
{
    /// <summary>
    /// Caches objects in memory
    /// </summary>
    /// <typeparam name="TKey">Key data type</typeparam>
    /// <typeparam name="TValue">Value data type</typeparam>
    public class ObjectCache<TKey, TValue>
    {
        private IDictionary<TKey, TValue> _data = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Gets the value by the key
        /// </summary>
        /// <param name="key">Key to retrieve the value</param>
        /// <param name="getVal">Function to retrieve a new value</param>
        /// <returns></returns>
        public TValue Get(TKey key, Func<TValue> getVal)
        {
            if (getVal == null) throw new ArgumentException("getVal");
            if (_data.ContainsKey(key)) return _data[key];
            _data.Add(key, getVal());
            return _data[key];
        }
    }
}
