using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveIRC.Interface
{
    public static class ChannelUserExtensions
    {
        /// <summary>
        /// Query if the user has no modes in this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        ///
        /// <returns>
        /// True if the user has no modes in this channel, false if not.
        /// </returns>
        public static bool HasNone(this IChannelUser channelUser)
        {
            return ChannelUserMode.ModeFlags(channelUser.Modes) == 0;
        }

        /// <summary>
        /// Query if the user has voice in this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        ///
        /// <returns>
        /// True if user has voice, false if not.
        /// </returns>
        public static bool HasVoice(this IChannelUser channelUser)
        {
            return (ChannelUserMode.ModeFlags(channelUser.Modes) & ChannelUserModeType.Voice) == 
                ChannelUserModeType.Voice;
        }

        /// <summary>
        /// Query if the user has halfop in this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        ///
        /// <returns>
        /// True if user has halfop, false if not.
        /// </returns>
        public static bool HasHalfOp(this IChannelUser channelUser)
        {
            return (ChannelUserMode.ModeFlags(channelUser.Modes) & ChannelUserModeType.HalfOp) ==
                ChannelUserModeType.HalfOp;
        }

        /// <summary>
        /// Query if the user has op in this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        ///
        /// <returns>
        /// True if user has op, false if not.
        /// </returns>
        public static bool HasOp(this IChannelUser channelUser)
        {
            return (ChannelUserMode.ModeFlags(channelUser.Modes) & ChannelUserModeType.Op) ==
                ChannelUserModeType.Op;
        }

        /// <summary>
        /// Query if the user has protected in this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        ///
        /// <returns>
        /// True if user has protected, false if not.
        /// </returns>
        public static bool HasProtected(this IChannelUser channelUser)
        {
            return (ChannelUserMode.ModeFlags(channelUser.Modes) & ChannelUserModeType.Protected) ==
                ChannelUserModeType.Protected;
        }

        /// <summary>
        /// Query if the user has owner in this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        ///
        /// <returns>
        /// True if user has owner, false if not.
        /// </returns>
        public static bool HasOwner(this IChannelUser channelUser)
        {
            return (ChannelUserMode.ModeFlags(channelUser.Modes) & ChannelUserModeType.Owner) ==
                ChannelUserModeType.Owner;
        }

        /// <summary>
        /// Gets the highest mode this user has on this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        public static ChannelUserModeType HighestUserMode(this IChannelUser channelUser)
        {
            return ChannelUserMode.HighestMode(channelUser.Modes);
        }

        /// <summary>
        /// Kicks this user on this channel.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        public static void Kick(this IChannelUser channelUser)
        {
            IClientConnection connection = channelUser.Connection;
            connection.SendAndForget(connection.MessageSender.Kick(channelUser));
        }

        /// <summary>
        /// Kicks this user on this channel with a reason.
        /// </summary>
        ///
        /// <param name="channelUser">The channel user to act on.</param>
        /// <param name="reason">    The reason.</param>
        public static void Kick(this IChannelUser channelUser, String reason)
        {
            IClientConnection connection = channelUser.Connection;
            connection.SendAndForget(connection.MessageSender.Kick(channelUser, reason));
        }

        public static void Voice(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name, 
                Mode.AddChar.ToString() + ChannelUserMode.VoiceMode);
        }

        public static void DeVoice(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.RemoveChar.ToString() + ChannelUserMode.VoiceMode);
        }

        public static void HalfOp(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.AddChar.ToString() + ChannelUserMode.HalfOpMode);
        }

        public static void DeHalfOp(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.RemoveChar.ToString() + ChannelUserMode.HalfOpMode);
        }

        public static void Op(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.AddChar.ToString() + ChannelUserMode.OpMode);
        }

        public static void DeOp(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.RemoveChar.ToString() + ChannelUserMode.OpMode);
        }

        public static void Protect(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.AddChar.ToString() + ChannelUserMode.ProtectedMode);
        }

        public static void DeProtect(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.RemoveChar.ToString() + ChannelUserMode.ProtectedMode);
        }

        public static void Owner(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.AddChar.ToString() + ChannelUserMode.OwnerMode);
        }

        public static void DeOwner(this IChannelUser channelUser)
        {
            channelUser.Channel.SetUserMode(channelUser.User.Name,
                Mode.RemoveChar.ToString() + ChannelUserMode.OwnerMode);
        }
    }
}
