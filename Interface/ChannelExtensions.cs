using System;

namespace ReactiveIRC.Interface
{
    public static class ChannelExtensions
    {
        /// <summary>
        /// Sets the topic for this channel.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="topic">  The topic.</param>
        public static void SetTopic(this IChannel channel, String topic)
        {
            IClientConnection connection = channel.Connection;
            connection.SendAndForget(connection.MessageSender.Topic(channel, topic));
        }

        /// <summary>
        /// Clears the topic for this channel.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        public static void ClearTopic(this IChannel channel)
        {
            IClientConnection connection = channel.Connection;
            connection.SendAndForget(connection.MessageSender.Topic(channel));
        }

        /// <summary>
        /// Sets a new mode for this channel.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="mode">   The mode.</param>
        public static void SetMode(this IChannel channel, String mode)
        {
            IClientConnection connection = channel.Connection;
            connection.SendAndForget(connection.MessageSender.Mode(channel, mode));
        }

        /// <summary>
        /// Sets a new mode for a user on this channel.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="mode">   The mode.</param>
        public static void SetUserMode(this IChannel channel, String userName, String mode)
        {
            IClientConnection connection = channel.Connection;
            connection.SendAndForget(connection.MessageSender.Mode(channel, mode, userName));
        }

        /// <summary>
        /// Kicks a user with given name from this channel.
        /// </summary>
        ///
        /// <param name="channel"> The channel to act on.</param>
        /// <param name="userName">Name of the user.</param>
        public static void Kick(this IChannel channel, String userName)
        {
            IClientConnection connection = channel.Connection;
            IChannelUser channelUser = channel.GetUser(userName);
            connection.SendAndForget(connection.MessageSender.Kick(channelUser));
        }

        /// <summary>
        /// Kicks a user with given user from this channel.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="user">   The user.</param>
        public static void Kick(this IChannel channel, IUser user)
        {
            IClientConnection connection = channel.Connection;
            IChannelUser channelUser = channel.GetUser(user.Name);
            connection.SendAndForget(connection.MessageSender.Kick(channelUser));
        }

        /// <summary>
        /// Kicks a user with given user name from this channel with a reason.
        /// </summary>
        ///
        /// <param name="channel"> The channel to act on.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="reason">  The reason.</param>
        public static void Kick(this IChannel channel, String userName, String reason)
        {
            IClientConnection connection = channel.Connection;
            IChannelUser channelUser = channel.GetUser(userName);
            connection.SendAndForget(connection.MessageSender.Kick(channelUser, reason));
        }

        /// <summary>
        /// Kicks a user with given user from this channel with a reason.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="user">   The user.</param>
        /// <param name="reason"> The reason.</param>
        public static void Kick(this IChannel channel, IUser user, String reason)
        {
            IClientConnection connection = channel.Connection;
            IChannelUser channelUser = channel.GetUser(user.Name);
            connection.SendAndForget(connection.MessageSender.Kick(channelUser, reason));
        }
    }
}
