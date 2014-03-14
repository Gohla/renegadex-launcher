using System;
using System.Collections;
using System.Collections.Generic;

namespace RXL.Util
{
    public class BaseComparer
    {
        public bool Invert { get; set; }
    }

    public class GenericComparer<T, TType> : BaseComparer, IComparer<T>, IComparer
        where T : class
    {
        private readonly Func<T, TType> _getMember;

        public GenericComparer(Func<T, TType> getMember, bool invert = false)
        {
            _getMember = getMember;
            Invert = invert;
        }

        public int Compare(T x, T y)
        {
            return Comparer<TType>.Default.Compare(_getMember(x), _getMember(y)) * (Invert ? -1 : 1);
        }

        public int Compare(Object x, Object y)
        {
            return Compare(x as T, y as T);
        }
    }
}
