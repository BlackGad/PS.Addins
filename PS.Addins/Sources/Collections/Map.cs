using System;
using System.Collections.Generic;

namespace PS.Addins.Collections
{
    public class Map<TKey, TValue>
    {
        protected readonly object Locker;
        protected readonly Dictionary<TValue, TKey> ReverseStorage;
        protected readonly Dictionary<TKey, TValue> Storage;

        #region Constructors

        public Map()
        {
            Storage = new Dictionary<TKey, TValue>();
            ReverseStorage = new Dictionary<TValue, TKey>();
            Locker = new object();
        }

        #endregion

        #region Members

        public TValue Query(TKey key)
        {
            lock (Locker)
            {
                if (Storage.ContainsKey(key))
                {
                    return Storage[key];
                }

                return default(TValue);
            }
        }

        public TKey Query(TValue value)
        {
            lock (Locker)
            {
                if (ReverseStorage.ContainsKey(value))
                {
                    return ReverseStorage[value];
                }

                return default(TKey);
            }
        }

        public void Register(TKey key, TValue value)
        {
            lock (Locker)
            {
                if (Storage.ContainsKey(key) || ReverseStorage.ContainsKey(value)) throw new ArgumentException("Pair already exist");
                Storage.Add(key, value);
                ReverseStorage.Add(value, key);
            }
        }

        #endregion
    }
}