using System;
using System.Collections;
using System.Collections.Generic;

namespace RXL.WPFClient.Utils
{
    public class BaseComparer
    {
        public bool Invert { get; set; }
    }

    public class GenericComparer<T, S> : BaseComparer, IComparer<T>, IComparer
        where T : class
    {
        private Func<T, S> _getMember;

        public GenericComparer(Func<T, S> getMember, bool invert = false)
        {
            _getMember = getMember;
            Invert = invert;
        }

        public int Compare(T x, T y)
        {
            return Comparer<S>.Default.Compare(_getMember(x), _getMember(y)) * (Invert ? -1 : 1);
        }

        public int Compare(Object x, Object y)
        {
            return Compare(x as T, y as T);
        }
    }
}
