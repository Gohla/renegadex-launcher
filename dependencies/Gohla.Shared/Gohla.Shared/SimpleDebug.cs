using System;

namespace Gohla.Shared
{
    public static class SimpleDebug
    {
        public static T Debug<T>(this T t)
        {
            Console.WriteLine(t);
            return t;
        }

        public static T Debug<T>(this T t, String message)
        {
            Console.WriteLine(message + t);
            return t;
        }
    }
}
