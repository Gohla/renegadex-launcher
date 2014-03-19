using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gohla.Shared
{
    public class MultiValueDictionary<TKey, TValue> : IDictionary<TKey, ICollection<TValue>>
    {
        private Dictionary<TKey, ICollection<TValue>> _dictionary;

        public MultiValueDictionary()
        {
            _dictionary = new Dictionary<TKey, ICollection<TValue>>();
        }

        public MultiValueDictionary(Dictionary<TKey, ICollection<TValue>> dictionary)
        {
            _dictionary = new Dictionary<TKey, ICollection<TValue>>(dictionary);
        }

        public MultiValueDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, ICollection<TValue>>(comparer);
        }

        public MultiValueDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, ICollection<TValue>>(capacity);
        }

        public MultiValueDictionary(Dictionary<TKey, ICollection<TValue>> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, ICollection<TValue>>(dictionary, comparer);
        }

        public MultiValueDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, ICollection<TValue>>(capacity, comparer);
        }

        public int Count
        {
            get
            {
                return _dictionary.Values
                    .Sum(c => c.Count)
                    ;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return _dictionary.Keys;
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                return _dictionary.Values
                    .SelectMany(x => x)
                    ;
            }
        }

        ICollection<ICollection<TValue>> IDictionary<TKey, ICollection<TValue>>.Values
        {
            get
            {
                return _dictionary.Values;
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
        {
            get
            {
                foreach(KeyValuePair<TKey, ICollection<TValue>> pair in _dictionary)
                {
                    foreach(TValue val in pair.Value)
                    {
                        yield return new KeyValuePair<TKey, TValue>(pair.Key, val);
                    }
                }
            }
        }

        public ICollection<TValue> this[TKey key]
        {
            get
            {
                ICollection<TValue> collection;
                if(_dictionary.TryGetValue(key, out collection))
                    return collection;
                else
                    return Add(key);
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        public ICollection<TValue> Add(TKey key)
        {
            ICollection<TValue> collection;
            if(!_dictionary.TryGetValue(key, out collection))
            {
                collection = new List<TValue>();
                _dictionary[key] = collection;
            }
            return collection;
        }

        public void Add(TKey key, TValue value)
        {
            ICollection<TValue> collection;
            if(_dictionary.TryGetValue(key, out collection))
            {
                collection.Add(value);
            }
            else
            {
                collection = new List<TValue>();
                collection.Add(value);
                _dictionary[key] = collection;
            }
        }

        public void Add(TKey key, ICollection<TValue> values)
        {
            ICollection<TValue> collection;
            if(_dictionary.TryGetValue(key, out collection))
            {
                foreach(TValue value in values)
                    collection.Add(value);
            }
            else
            {
                collection = new List<TValue>(values);
                _dictionary[key] = collection;
            }
        }

        public void Add(TKey key, IEnumerable<TValue> values)
        {
            ICollection<TValue> collection;
            if(_dictionary.TryGetValue(key, out collection))
            {
                foreach(TValue value in values)
                    collection.Add(value);
            }
            else
            {
                collection = new List<TValue>(values);
                _dictionary[key] = collection;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void Add(KeyValuePair<TKey, ICollection<TValue>> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void CopyTo(KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, ICollection<TValue>>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public bool Contains(KeyValuePair<TKey, ICollection<TValue>> pair)
        {
            return ((ICollection<KeyValuePair<TKey, ICollection<TValue>>>)_dictionary).Contains(pair);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out ICollection<TValue> values)
        {
            ICollection<TValue> collection;
            bool found = _dictionary.TryGetValue(key, out collection);
            values = collection;
            return found;
        }

        public bool TryGetValue(TKey key, out IEnumerable<TValue> values)
        {
            ICollection<TValue> collection;
            bool found = _dictionary.TryGetValue(key, out collection);
            values = collection;
            return found;
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool Remove(TKey key, TValue value)
        {
            ICollection<TValue> collection;
            if(_dictionary.TryGetValue(key, out collection))
                return collection.Remove(value);
            return false;
        }

        public void Remove(KeyValuePair<TKey, TValue> pair)
        {
            Remove(pair.Key, pair.Value);
        }

        public bool Remove(KeyValuePair<TKey, ICollection<TValue>> pair)
        {
            ICollection<TValue> collection;
            if(_dictionary.TryGetValue(pair.Key, out collection))
            {
                if(Equals(collection, pair.Value))
                    return _dictionary.Remove(pair.Key);
            }
            return false;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}