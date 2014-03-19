using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Gohla.Shared
{
    public static class ReactiveCollectionChanged
    {
        public static IObservable<T> AddedItems<T>(this INotifyCollectionChanged obj)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(obj, "CollectionChanged")
                .Where(args => args.EventArgs.Action == NotifyCollectionChangedAction.Add)
                .SelectMany(args => args.EventArgs.NewItems.Cast<T>());
        }

        public static IObservable<T> RemovedItems<T>(this INotifyCollectionChanged obj)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(obj, "CollectionChanged")
                .Where(args => args.EventArgs.Action == NotifyCollectionChangedAction.Remove)
                .SelectMany(args => args.EventArgs.OldItems.Cast<T>());
        }

        public static IObservable<Unit> Cleared(this INotifyCollectionChanged obj)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(obj, "CollectionChanged")
                .Where(args => args.EventArgs.Action == NotifyCollectionChangedAction.Reset)
                .Select(_ => Unit.Default);
        }

        public static IObservable<Unit> Changed(this INotifyCollectionChanged obj)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(obj, "CollectionChanged")
                .Select(_ => Unit.Default);
        }
    }
}
