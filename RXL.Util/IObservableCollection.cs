using System.Collections.Generic;
using System.Collections.Specialized;

namespace RXL.Util
{
    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {

    }
}
