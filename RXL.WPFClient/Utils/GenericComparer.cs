using System;
using System.Collections;
using System.Collections.Generic;

namespace RXL.WPFClient.Utils
{
    public class GenericComparer<T, S> : IComparer<T>, IComparer
        where T : class
    {
        private Func<T, S> _getMember;
        private bool _invert;

        public bool Invert { get { return _invert; } set { _invert = value; } }

        public GenericComparer(Func<T, S> getMember, bool invert = false)
        {
            _getMember = getMember;
            _invert = invert;
        }

        public int Compare(T x, T y)
        {
            return Comparer<S>.Default.Compare(_getMember(x), _getMember(y)) * (_invert ? -1 : 1);
        }

        public int Compare(Object x, Object y)
        {
            return Compare(x as T, y as T);
        }
    }
}
