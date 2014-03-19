using System;

namespace Gohla.Shared
{
    /**
    Utility class for formatting byte sizes or speeds into readable strings.
    **/
    public static class SizeSpeedString
    {
        /**
        Format given size into a readable string.
        
        @param  size    The size in bytes.
        
        @return The formatted size.
        **/
        public static String FormatSize(ulong size)
        {
            return Format(size, false);
        }

        /**
        Format given speed into a readable string.
        
        @param  speed   The speed in bytes per second.
        
        @return The formatted speed.
        **/
        public static String FormatSpeed(ulong speed)
        {
            return Format(speed, true);
        }

        /**
        Formats given size or speed into a readable string.
        
        @param  bytes   The size in bytes or the speed in byes per second.
        @param  speed   True if given bytes represent a speed, false if represents size.
        
        @return The formatted size or speed.
        **/
        public static String Format(ulong bytes, bool speed)
        {
            String suffix = speed ? "B/s" : "B";
            String suffixSingular = speed ? "Bytes/s" : "Bytes";

            if(bytes >= 1125899906842624)
            {
                Decimal size = Decimal.Divide(bytes, 1125899906842624);
                return String.Format("{0:##.##} P{1}", size, suffix);
            }
            if(bytes >= 1099511627776)
            {
                Decimal size = Decimal.Divide(bytes, 1099511627776);
                return String.Format("{0:##.##} T{1}", size, suffix);
            }
            if(bytes >= 1073741824)
            {
                Decimal size = Decimal.Divide(bytes, 1073741824);
                return String.Format("{0:##.##} G{1}", size, suffix);
            }
            else if(bytes >= 1048576)
            {
                Decimal size = Decimal.Divide(bytes, 1048576);
                return String.Format("{0:##.##} M{1}", size, suffix);
            }
            else if(bytes >= 1024)
            {
                Decimal size = Decimal.Divide(bytes, 1024);
                return String.Format("{0:##.##} K{1}", size, suffix);
            }
            else if(bytes > 0 & bytes < 1024)
            {
                Decimal size = bytes;
                return String.Format("{0:##.##} {1}", size, suffixSingular);
            }
            else
            {
                return String.Format("0 {0}", suffixSingular);
            }
        }
    }
}
