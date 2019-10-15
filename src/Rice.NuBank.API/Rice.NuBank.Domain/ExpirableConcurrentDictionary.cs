using System;
using System.Collections.Concurrent;

namespace Rice.NuBank.Domain
{
    public class ExpirableConcurrentDictionary<TKey, TValue>
    {
        private object _mutex;

        private readonly ConcurrentDictionary<TKey, ExpirableValue<TValue>> _internalDictionary;
        private readonly TimeSpan _timeToLive;

        public ExpirableConcurrentDictionary(TimeSpan timeToLive)
        {
            _mutex = new Object();
            _timeToLive = timeToLive;
            _internalDictionary = new ConcurrentDictionary<TKey, ExpirableValue<TValue>>();
        }

        public bool TryGetValue(TKey key, out TValue outValue)
        {
            lock (_mutex)
            {
                var success = _internalDictionary.TryGetValue(key, out var value);

                if (value != null && value.Expiration > DateTime.Now)
                {
                    outValue = value.Value;
                }
                else
                {
                    outValue = default;
                }

                return success;
            }
        }

        public bool TryAddValue(TKey key, TValue value)
        {
            lock (_mutex)
            {
                if (_internalDictionary.TryAdd(key, new ExpirableValue<TValue>(value, _timeToLive)))
                {
                    return true;
                }

                if (_internalDictionary.TryGetValue(key, out var oldValue))
                {
                    return false;
                }
                
                _internalDictionary.TryRemove(key, out _);
                return _internalDictionary.TryAdd(key, new ExpirableValue<TValue>(value, _timeToLive));
            }
        }
    }

    public class ExpirableValue<T>
    {
        public T Value { get; set; }
        public DateTime Expiration { get; set; }

        public ExpirableValue(T value, TimeSpan timeToLive)
        {
            Value = value;
            Expiration = DateTime.Now.Add(timeToLive);
        }
    }
}