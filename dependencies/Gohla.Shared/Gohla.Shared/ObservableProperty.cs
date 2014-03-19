using System;
using System.Reactive.Subjects;

namespace Gohla.Shared
{
    public class ObservableProperty<T> : IDisposable, IObservable<T>
    {
        private T _value;
        private Subject<T> _subject;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if(Equals(_value, value))
                    return;

                _value = value;
                _subject.OnNext(value);
            }
        }

        public ObservableProperty(T value)
        {
            _value = value;
            _subject = new Subject<T>();
        }

        public void Dispose()
        {
            if(_subject == null)
                return;

            _subject.OnCompleted();
            _subject.Dispose();
            _subject = null;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        public static implicit operator T(ObservableProperty<T> property)
        {
            return property.Value;
        }

        public static implicit operator ObservableProperty<T>(T value)
        {
            return new ObservableProperty<T>(value);
        }
    }
}
