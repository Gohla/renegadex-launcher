using System.Text;
using System;

namespace Gohla.Shared
{
    /**
    TimeSpan extensions.
    **/
    public static class TimeSpanExtension
    {
        /**
        A TimeSpan extension method that converts a span to a readable string.
        
        @param  span    The span to act on.
        
        @return Readable string representation of the time span.
        **/
        public static String ToReadableString(this TimeSpan span, short numParts = short.MaxValue)
        {
            if(span.Seconds <= 0)
                return "now";

            if(span.Days > 365)
                return "∞";

            StringBuilder builder = new StringBuilder();
            if(span.Days > 0 && --numParts > 0)
                builder.Append(String.Format("{0:0}d, ", span.Days));
            if(span.Hours > 0 && --numParts > 0)
                builder.Append(String.Format("{0:0}h, ", span.Hours));
            if(span.Minutes > 0 && --numParts > 0)
                builder.Append(String.Format("{0:0}m, ", span.Minutes));
            if(span.Seconds > 0 && --numParts > 0)
                builder.Append(String.Format("{0:0}s", span.Seconds));

            String formatted = builder.ToString();
            if(formatted.EndsWith(", ")) 
                formatted = formatted.Substring(0, formatted.Length - 2);

            return formatted;
        }

    }
}
