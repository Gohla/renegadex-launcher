using System;

public static class ArrayConcat
{
    public static T[] Concat<T>(this T[] x, params T[] y)
    {
        int oldLen = x.Length;
        Array.Resize<T>(ref x, x.Length + y.Length);
        Array.Copy(y, 0, x, oldLen, y.Length);
        return x;
    }
}