using System.Collections.Generic;
using System.Collections.Specialized;

namespace Gohla.Shared
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>,
        IObservableCollection<KeyValuePair<TKey, TValue>>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnCollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }

        public new bool Remove(TKey key)
        {
            TValue value = base[key];
            bool removed = base.Remove(key);
            if(removed)
                OnCollectionChanged(this, 
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));

            return removed;
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(CollectionChanged != null)
            {
                CollectionChanged(sender, args);
            }
        }
    }
}