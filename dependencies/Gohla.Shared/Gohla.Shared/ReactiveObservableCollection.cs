using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;

namespace Gohla.Shared
{
    public class ReactiveObservableCollection<T> : ObservableCollection<T>, IDisposable
    {
        private CompositeDisposable _disposable = new CompositeDisposable();

        public ReactiveObservableCollection(IEnumerable<T> initial, IObservable<T> addObservable, 
            IObservable<T> removeObservable, IObservable<Unit> clearedObservable) :
            base(initial)
        {
            // TODO: Efficient removal
            _disposable.Add(addObservable.Subscribe(t => Add(t)));
            _disposable.Add(removeObservable.Subscribe(t => Remove(t)));
            _disposable.Add(clearedObservable.Subscribe(t => Clear()));
        }

        ~ReactiveObservableCollection()
        {
            _disposable.Dispose();
        }

        public void Dispose()
        {
            _disposable.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
