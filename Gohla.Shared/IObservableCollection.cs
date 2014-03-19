using System.Collections.Generic;
using System.Collections.Specialized;

namespace Gohla.Shared
{
    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {

    }
}
