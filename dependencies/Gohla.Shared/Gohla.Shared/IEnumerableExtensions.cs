using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    public static void Do<T>(this IEnumerable<T> source, Action<T> action)
    {
        if(source == null)
            throw new ArgumentNullException("source");

        foreach(T item in source)
            action(item);
    }

    public static void Evaluate<T>(this IEnumerable<T> source)
    {
        if(source == null)
            throw new ArgumentNullException("enumerable");

        foreach(T item in source);
    }

    public static IEnumerable<R> As<R>(this IEnumerable source)
        where R : class
    {
        if(source == null)
            throw new ArgumentNullException("source");

        foreach(object obj in source)
            yield return obj as R;
    }

    public static IEnumerable<T> AsEnumerable<T>(this T item)
    {
        if(item == null)
            throw new ArgumentNullException("item");

        yield return item;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        if(source == null)
            throw new ArgumentNullException("source");

        ICollection<T> genericCollection = source as ICollection<T>;
        if(genericCollection != null)
            return genericCollection.Count == 0;

        ICollection nonGenericCollection = source as ICollection;
        if(nonGenericCollection != null)
            return nonGenericCollection.Count == 0;

        return !source.Any();
    }

    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
    {
        if(source == null)
            throw new ArgumentNullException("source");

        using(IEnumerator<T> iterator = source.GetEnumerator())
        {
            if(!iterator.MoveNext())
            {
                yield break;
            }

            T previous = iterator.Current;

            while(iterator.MoveNext())
            {
                yield return previous;
                previous = iterator.Current;
            }
        }
    }
}
