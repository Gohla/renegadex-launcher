using System;
using System.Collections.Specialized;
using System.Threading;

namespace Gohla.Shared
{
    public class SynchronizedKeyedCollection<TKey, TItem> :
        System.Collections.ObjectModel.KeyedCollection<TKey, TItem>,
        IDisposable,
        IObservableCollection<TItem>
        where TItem : class, IKeyedObject<TKey>
    {
        private SynchronizationContext _context;

        public System.Collections.Generic.IEnumerable<TKey> Keys
        {
            get
            {
                return base.Dictionary.Keys;
            }
        }

        public System.Collections.Generic.IEnumerable<TItem> Values
        {
            get
            {
                return base.Dictionary.Values;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SynchronizedKeyedCollection(SynchronizationContext context)
            : base()
        {
            _context = context;
        }

        public void Dispose()
        {
            if(CollectionChanged == null)
                return;

            CollectionChanged = null;
        }

        public new void ChangeItemKey(TItem item, TKey newKey)
        {
            base.ChangeItemKey(item, newKey);
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return item.Key;
        }

        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                item, index));
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                item, index));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item,
                index));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if(_context != null)
            {
                _context.Send(SendOnCollectionChanged, args);
            }
            else
            {
                if(CollectionChanged != null)
                    CollectionChanged(this, args);
            }
        }

        private void SendOnCollectionChanged(object state)
        {
            NotifyCollectionChangedEventArgs args = state as NotifyCollectionChangedEventArgs;
            if(CollectionChanged != null)
                CollectionChanged(this, args);
        }
    }
}
