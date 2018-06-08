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
            Lazy<TValue> result;
            return _storage.TryGetValue(key, out result) ? result.Value : default(TValue);
        }

        public TValue Query(TKey key, Func<TKey, TValue> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return _storage.GetOrAdd(key, t => new Lazy<TValue>(() => factory(key), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
        }

        public TValue Remove(TKey key)
        {
            Lazy<TValue> removed;
            return _storage.TryGetValue(key, out removed)
                ? removed.Value
                : default(TValue);
        }

        #endregion
    }
}