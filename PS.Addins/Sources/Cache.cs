using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PS.Addins
{
    class Cache<TKey, TValue>
    {
        readonly ConcurrentDictionary<TKey, Lazy<TValue>> _storage;

        #region Constructors

        public Cache()
        {
            _storage = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        #endregion

        #region Members

        public TValue Query(TKey key)
        {
            return _storage.TryGetValue(key, out var result) ? result.Value : default(TValue);
        }

        public TValue Query(TKey key, Func<TKey, TValue> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return _storage.GetOrAdd(key, t => new Lazy<TValue>(() => factory(key), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
        }

        public TValue Remove(TKey key)
        {
            return _storage.TryGetValue(key, out var removed)
                ? removed.Value
                : default(TValue);
        }

        #endregion
    }
}