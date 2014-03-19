using System;
using System.Text.RegularExpressions;

namespace ReactiveIRC.Interface
{
    public enum ControlType
    {
        Unknown
      , Color
      , Bold
      , Underline
      , Italic
      , Reverse
      , Stop
    }

    public enum ColorCode
    {
        None        = -1
      , White       = 0
      , Black       = 1
      , Blue        = 2
      , Green       = 3
      , LightRed    = 4
      , Brown       = 5
      , Purple      = 6
      , Orange      = 7
      , Yellow      = 8
      , LightGreen  = 9
      , Cyan        = 10
      , LightCyan   = 11
      , LightBlue   = 12
      , Pink        = 13
      , Grey        = 14
      , LightGrey   = 15
    }

    public static class ControlCodes
    {
        public const char ColorBarrierCode = '\x03';
        public const char BoldCode = '\x02';
        public const char UnderlineCode = '\x1F';
        public const char ItalicCode = '\x1D';
        public const char ReverseCode = '\x16';
        public const char StopCode = '\x0F';
        public static readonly Regex Pattern = new Regex(@"^([\d]{1,2})(?:,([\d]{1,2}))?", 
            RegexOptions.Compiled | RegexOptions.Multiline);

        public static ControlType Type(char c)
        {
            switch(c)
            {
                case ColorBarrierCode: return ControlType.Color;
                case BoldCode: return ControlType.Bold;
                case UnderlineCode: return ControlType.Underline;
                case ItalicCode: return ControlType.Italic;
                case ReverseCode: return ControlType.Reverse;
                case StopCode: return ControlType.Stop;
            }

            return ControlType.Unknown;
        }

        public static int GetColors(String str, out int foreground, out int background, out bool reset)
        {
            int eaten = 1;
            foreground = -1;
            background = -1;
            reset = false;

            Match matchResults = Pattern.Match(str);
            if(!matchResults.Success)
            {
                reset = true;
                return eaten;
            }

            if(matchResults.Groups[1].Success)
            {
                String nums = matchResults.Groups[1].Value;
                eaten += nums.Length;
                foreground = Int32.Parse(nums);
            }

            if(matchResults.Groups[2].Success)
            {
                String nums = matchResults.Groups[2].Value;
                eaten += 1 + nums.Length;
                background = Int32.Parse(nums);
            }

            return eaten;
        }

        public static String Color(String str)
        {
            return String.Concat(ColorBarrierCode, str, ColorBarrierCode);
        }

        public static String Color(String str, ColorCode foreground)
        {
            if(foreground == ColorCode.None)
                return str;
            return String.Concat(ColorBarrierCode, (int)foreground, str, ColorBarrierCode);
        }

        public static String Color(String str, ColorCode foreground, ColorCode background)
        {
            if(foreground == ColorCode.None)
                return str;
            else if(foreground != ColorCode.None && background == ColorCode.None)
                return Color(str, foreground);
            else
                return String.Concat(ColorBarrierCode, (int)foreground, ',', (int)background, str, ColorBarrierCode);
        }

        public static bool ValidColor(int color)
        {
            return (int)color >= 0 && (int)color <= 15;
        }

        public static bool ValidColor(ColorCode color)
        {
            return ValidColor((int)color);
        }

        public static String Bold(String str)
        {
            return String.Concat(BoldCode, str, BoldCode);
        }

        public static String Underline(String str)
        {
            return String.Concat(UnderlineCode, str, UnderlineCode);
        }

        public static String Italic(String str)
        {
            return String.Concat(ItalicCode, str, ItalicCode);
        }

        public static String Reverse(String str)
        {
            return String.Concat(ReverseCode, str, ReverseCode);
        }
    }
}
