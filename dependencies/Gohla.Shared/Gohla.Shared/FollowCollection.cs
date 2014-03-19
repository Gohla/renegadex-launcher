using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Disposables;

namespace Gohla.Shared
{
    public static class FollowCollection
    {
        public static IDisposable CopyAndFollow<T, C>(this ICollection<T> collection, C followCollection)
            where C : INotifyCollectionChanged, IEnumerable<T>
        {
            foreach(T t in followCollection)
                collection.Add(t);
            return collection.Follow(followCollection);
        }

        public static IDisposable Follow<T>(this ICollection<T> collection, INotifyCollectionChanged followCollection)
        {
            CompositeDisposable disposable = new CompositeDisposable();
            disposable.Add(followCollection.AddedItems<T>().Subscribe(i => collection.Add(i)));
            disposable.Add(followCollection.RemovedItems<T>().Subscribe(i => collection.Remove(i)));
            disposable.Add(followCollection.Cleared().Subscribe(i => collection.Clear()));
            return disposable;
        }
    }
}
