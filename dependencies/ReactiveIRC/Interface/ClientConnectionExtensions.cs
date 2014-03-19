using System;
using System.Reactive;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Client connection extensions.
    /// </summary>
    public static class ClientConnectionExtensions
    {
        /// <summary>
        /// Send given messages to the server. Returns an observable that must be subscribed to to actually send the
        /// message.
        /// </summary>
        ///
        /// <param name="messages">The messages to send.</param>
        ///
        /// <returns>
        /// An observable stream that notifies when message is sent or has failed to send.
        /// </returns>
        public static IObservable<Unit> Send(this IClientConnection connection, params ISendMessage[] messages)
        {
            return connection.Send(messages);
        }

        /// <summary>
        /// Send given messages to the server.
        /// </summary>
        ///
        /// <param name="messages">The messages to send.</param>
        public static void SendAndForget(this IClientConnection connection, params ISendMessage[] messages)
        {
            connection.SendAndForget(messages);
        }

        /// <summary>
        /// Joins given channel name
        /// </summary>
        ///
        /// <param name="connection"> The connection to act on.</param>
        /// <param name="channelName">Name of the channel.</param>
        public static IChannel Join(this IClientConnection connection, String channelName)
        {
            IChannel channel = connection.GetChannel(channelName);
            connection.SendAndForget(connection.MessageSender.Join(channel));
            return channel;
        }

        /// <summary>
        /// Joins given channel.
        /// </summary>
        ///
        /// <param name="connection">The connection to act on.</param>
        /// <param name="channel">   The channel.</param>
        public static void Join(this IClientConnection connection, IChannel channel)
        {
            connection.SendAndForget(connection.MessageSender.Join(channel));
        }

        /// <summary>
        /// Parts given channel name.
        /// </summary>
        ///
        /// <param name="connection"> The connection to act on.</param>
        /// <param name="channelName">Name of the channel.</param>
        public static IChannel Part(this IClientConnection connection, String channelName)
        {
            IChannel channel = connection.GetChannel(channelName);
            connection.SendAndForget(connection.MessageSender.Part(channel));
            return channel;
        }

        /// <summary>
        /// Parts given channel.
        /// </summary>
        ///
        /// <param name="connection">The connection to act on.</param>
        /// <param name="channel">   The channel.</param>
        public static void Part(this IClientConnection connection, IChannel channel)
        {
            connection.SendAndForget(connection.MessageSender.Part(channel));
        }

        /// <summary>
        /// Parts given channel name with a message.
        /// </summary>
        ///
        /// <param name="connection"> The connection to act on.</param>
        /// <param name="channelName">Name of the channel.</param>
        /// <param name="message">    The part message.</param>
        public static IChannel Part(this IClientConnection connection, String channelName, String message)
        {
            IChannel channel = connection.GetChannel(channelName);
            connection.SendAndForget(connection.MessageSender.Part(channel, message));
            return channel;
        }

        /// <summary>
        /// Parts given channel with a message.
        /// </summary>
        ///
        /// <param name="connection">The connection to act on.</param>
        /// <param name="channel">   The channel.</param>
        /// <param name="message">   The part message.</param>
        public static void Part(this IClientConnection connection, IChannel channel, String message)
        {
            connection.SendAndForget(connection.MessageSender.Part(channel, message));
        }

        /// <summary>
        /// Leaves and joins the given channel name.
        /// </summary>
        ///
        /// <param name="connection"> The connection to act on.</param>
        /// <param name="channelName">Name of the channel.</param>
        public static IChannel Hop(this IClientConnection connection, String channelName)
        {
            connection.Part(channelName);
            return connection.Join(channelName);
        }

        /// <summary>
        /// Leaves and joins the given channel.
        /// </summary>
        ///
        /// <param name="connection">The connection to act on.</param>
        /// <param name="channel">   The channel.</param>
        public static void Hop(this IClientConnection connection, IChannel channel)
        {
            connection.Part(channel);
            connection.Join(channel);
        }

        /// <summary>
        /// Leaves and joins the given channel name with a message.
        /// </summary>
        ///
        /// <param name="connection"> The connection to act on.</param>
        /// <param name="channelName">Name of the channel.</param>
        /// <param name="message">    The part message.</param>
        public static IChannel Hop(this IClientConnection connection, String channelName, String message)
        {
            connection.Part(channelName, message);
            return connection.Join(channelName);
        }

        /// <summary>
        /// Leaves and joins the given channel with a message.
        /// </summary>
        ///
        /// <param name="connection">The connection to act on.</param>
        /// <param name="channel">   The channel.</param>
        /// <param name="message">   The part message.</param>
        public static void Hop(this IClientConnection connection, IChannel channel, String message)
        {
            connection.Part(channel, message);
            connection.Join(channel);
        }

        /// <summary>
        /// Sets a new nickname.
        /// </summary>
        ///
        /// <param name="connection">The connection to act on.</param>
        /// <param name="nickname">  The new nickname.</param>
        public static void SetNickname(this IClientConnection connection, String nickname)
        {
            connection.SendAndForget(connection.MessageSender.Nick(nickname));
        }
    }
}
