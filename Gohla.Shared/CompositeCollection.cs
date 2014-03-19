using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Disposables;

namespace Gohla.Shared
{
    public class CompositeCollection<T, C> : IEnumerable<T>, INotifyCollectionChanged, IDisposable
        where C : ICollection<T>, INotifyCollectionChanged
    {
        private CompositeDisposable _subscriptions = new CompositeDisposable();
        protected List<C> _collections;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CompositeCollection(params C[] collections)
        {
            _collections = new List<C>(collections);
            foreach(C collection in _collections)
                collection.CollectionChanged += CollectionChange;
        }

        public CompositeCollection(IEnumerable<C> collections)
        {
            _collections = new List<C>(collections);
            foreach(C collection in _collections)
                collection.CollectionChanged += CollectionChange;
        }

        public void Dispose()
        {
            if(_subscriptions == null)
                return;

            _subscriptions.Dispose();
            _subscriptions = null;
        }

        public IDisposable CopyAndFollow<R>(ObservableCollection<R> collection, Func<R, C> convert)
        {
            foreach(R r in collection)
                AddCollection(convert(r));
            return Follow<R>(collection, convert);
        }

        public IDisposable CopyAndFollow<R>(IEnumerable<R> enumerable, IObservable<R> addedObservable, 
            IObservable<R> removedObservable, Func<R, C> convert)
        {
            foreach(R r in enumerable)
                AddCollection(convert(r));
            return Follow<R>(addedObservable, removedObservable, convert);
        }

        public IDisposable Follow<R>(INotifyCollectionChanged collection, Func<R, C> convert)
        {
            return Follow<R>(collection.AddedItems<R>(), collection.RemovedItems<R>(), convert);
        }

        public IDisposable Follow<R>(IObservable<R> addedObservable, IObservable<R> removedObservable,
            Func<R, C> convert)
        {
            CompositeDisposable disposables = new CompositeDisposable();
            disposables.Add(addedObservable.Subscribe(nc => this.AddCollection(convert(nc))));
            disposables.Add(removedObservable.Subscribe(nc => this.RemoveCollection(convert(nc))));
            return disposables;
        }

        public void AddCollection(C collection)
        {
            _collections.Add(collection);
            collection.CollectionChanged += CollectionChange;

            if(CollectionChanged == null)
                return;

            int offset = IndexAt(collection);
            int i = 0;
            foreach(T t in collection)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, 
                    t, 
                    offset + i
                ));

                ++i;
            }
        }

        public void RemoveCollection(C collection)
        {
            int offset = IndexAt(collection);
            foreach(T t in collection)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    t,
                    offset
                ));
            }

            _collections.Remove(collection);
            collection.CollectionChanged -= CollectionChange;
        }

        protected C CollectionAt(int index, out int newIndex)
        {
            int c = 0;
            foreach(C collection in _collections)
            {
                if(c + collection.Count > index)
                {
                    newIndex = index - c;
                    return collection;
                }

                c += collection.Count;
            }

            newIndex = -1;
            return default(C);
        }

        protected int IndexAt(object collection)
        {
            int offset = 0;
            int i = 0;
            while(!ReferenceEquals(collection, _collections[i]) && i < _collections.Count)
                offset += _collections[i++].Count;

            return offset;
        }

        #region IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            foreach(C collection in _collections)
                foreach(T item in collection)
                    yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region INotifyCollectionChanged
        protected void CollectionChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(CollectionChanged == null)
                return;

            int offset = IndexAt(sender);

            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(e.Action, e.NewItems[0],
                        e.NewStartingIndex + offset));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(e.Action, e.OldItems,
                        e.OldStartingIndex + offset));
                    break;
#if !WINDOWS_PHONE
                case NotifyCollectionChangedAction.Move:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.OldItems,
                        e.OldStartingIndex + offset));
                    break;
#endif
                case NotifyCollectionChangedAction.Replace:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.OldItems,
                        e.NewStartingIndex + offset));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    break;
            }
        }
        #endregion
    }
}
