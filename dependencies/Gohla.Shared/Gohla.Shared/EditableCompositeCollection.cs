using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Gohla.Shared
{
    public class EditableCompositeCollection<T, C> : CompositeCollection<T, C>, IList<T>, IList
        where C : IList<T>, INotifyCollectionChanged, new()
    {
        private C _editCollection = new C();

        public EditableCompositeCollection(params C[] collections):
            base(collections)
        {
            AddCollection(_editCollection);
        }

        public EditableCompositeCollection(IEnumerable<C> collections):
            base(collections)
        {
            AddCollection(_editCollection);
        }

        #region IList<T>
        public int IndexOf(T item)
        {
            return _collections
                .Select(c => c.IndexOf(item))
                .Where(i => i != -1)
                .DefaultIfEmpty(-1)
                .First()
                ;
        }

        public void Insert(int index, T item)
        {
            // TODO: Right semantics?

            int newIndex;
            C collection = CollectionAt(index, out newIndex);
            if(collection != null)
                collection.Insert(newIndex, item);
        }

        public void RemoveAt(int index)
        {
            int newIndex;
            C collection = CollectionAt(index, out newIndex);
            if(collection != null)
                collection.RemoveAt(newIndex);
        }

        public T this[int index]
        {
            get
            {
                int newIndex;
                C collection = CollectionAt(index, out newIndex);
                if(collection != null)
                    return collection[newIndex];
                return default(T);
            }
            set
            {
                int newIndex;
                C collection = CollectionAt(index, out newIndex);
                if(collection != null)
                    collection[newIndex] = value;
            }
        }

        public void Add(T item)
        {
            _editCollection.Add(item);
        }

        public void Clear()
        {
            _collections.Do(c => c.Clear());
        }

        public bool Contains(T item)
        {
            return _collections
                .Where(c => c.Contains(item))
                .Count() 
                > 0
                ;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach(C collection in _collections)
            {
                collection.CopyTo(array, arrayIndex);
                arrayIndex += collection.Count;
            }
        }

        public int Count
        {
            get { return _collections.Sum(c => c.Count); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            bool removed = false;
            foreach(C collection in _collections)
            {
                removed = removed || collection.Remove(item);
            }
            return removed;
        }
        #endregion

        #region IList
        public int Add(object value)
        {
            return ((IList)_editCollection).Add(value);
        }

        public bool Contains(object value)
        {
            return _collections
                .Cast<IList>()
                .Where(c => c.Contains(value))
                .Count()
                > 0
                ;
        }

        public int IndexOf(object value)
        {
            return _collections
                .Cast<IList>()
                .Select(c => c.IndexOf(value))
                .Where(i => i != -1)
                .DefaultIfEmpty(-1)
                .First()
                ;
        }

        public void Insert(int index, object value)
        {
            // TODO: Right semantics?

            int newIndex;
            C collection = CollectionAt(index, out newIndex);
            if(collection != null)
                ((IList)collection).Insert(newIndex, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            _collections
                .Cast<IList>()
                .Do(c => c.Remove(value));
        }

        object IList.this[int index]
        {
            get
            {
                int newIndex;
                C collection = CollectionAt(index, out newIndex);
                if(collection != null)
                    return collection[newIndex];
                return default(T);
            }
            set
            {
                int newIndex;
                C collection = CollectionAt(index, out newIndex);
                if(collection != null)
                    ((IList)collection)[newIndex] = value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            int i = index;
            foreach(C collection in _collections)
            {
                ((IList)collection).CopyTo(array, index);
                index += collection.Count;
            }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }
        #endregion
    }
}
