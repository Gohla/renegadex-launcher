using System;

namespace ReactiveIRC.Interface
{
    [Flags]
    public enum ChannelUserModeType
    {
        None        = 0
      , Voice       = 1
      , HalfOp      = 2
      , Op          = 4
      , Protected   = 8
      , Owner       = 16
    }

    public static class ChannelUserMode
    {
        public const char VoiceMode         = 'v';
        public const char HalfOpMode        = 'h';
        public const char OpMode            = 'o';
        public const char ProtectedMode     = 'p';
        public const char OwnerMode         = 'q';

        public const char VoiceSymbol       = '+';
        public const char HalfOpSymbol      = '%';
        public const char OpSymbol          = '@';
        public const char ProtectedSymbol   = '&';
        public const char OwnerSymbol       = '~';

        public static char SymbolToMode(char symbol)
        {
            switch(symbol)
            {
                case VoiceSymbol:       return VoiceMode;
                case HalfOpSymbol:      return HalfOpMode;
                case OpSymbol:          return OpMode;
                case ProtectedSymbol:   return ProtectedMode;
                case OwnerSymbol:       return OwnerMode;
            }
            return char.MinValue;
        }

        public static char ModeToSymbol(char mode)
        {
            switch(mode)
            {
                case VoiceMode:         return VoiceSymbol;
                case HalfOpMode:        return HalfOpSymbol;
                case OpMode:            return OpSymbol;
                case ProtectedMode:     return ProtectedSymbol;
                case OwnerMode:         return OwnerSymbol;
            }
            return char.MinValue;
        }

        public static ChannelUserModeType SymbolType(char symbol)
        {
            switch(symbol)
            {
                case VoiceSymbol:       return ChannelUserModeType.Voice;
                case HalfOpSymbol:      return ChannelUserModeType.HalfOp;
                case OpSymbol:          return ChannelUserModeType.Op;
                case ProtectedSymbol:   return ChannelUserModeType.Protected;
                case OwnerSymbol:       return ChannelUserModeType.Owner;
            }
            return ChannelUserModeType.None;
        }

        public static ChannelUserModeType ModeType(char mode)
        {
            return SymbolType(ModeToSymbol(mode));
        }

        public static ChannelUserModeType ModeFlags(Mode modes)
        {
            ChannelUserModeType flags = ChannelUserModeType.None;
            foreach(char mode in modes.Modes)
                flags |= ModeType(mode);

            return flags;
        }

        public static ChannelUserModeType HighestMode(Mode mode)
        {
            ChannelUserModeType flags = ModeFlags(mode);
            if((flags & ChannelUserModeType.Owner) != 0)
                return ChannelUserModeType.Owner;
            else if((flags & ChannelUserModeType.Protected) != 0)
                return ChannelUserModeType.Protected;
            else if((flags & ChannelUserModeType.Op) != 0)
                return ChannelUserModeType.Op;
            else if((flags & ChannelUserModeType.HalfOp) != 0)
                return ChannelUserModeType.HalfOp;
            else if((flags & ChannelUserModeType.Voice) != 0)
                return ChannelUserModeType.Voice;
            return ChannelUserModeType.None;
        }
    }
}
