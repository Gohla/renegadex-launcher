using System;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Interface representing a received IRC message.
    /// </summary>
    public interface IReceiveMessage : IMessage
    {
        /// <summary>
        /// Gets the date/time when the message was received.
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// Gets the sender of the message.
        /// </summary>
        IMessageTarget Sender { get; }

        /// <summary>
        /// Gets the receiver of the message.
        /// </summary>
        IMessageTarget Receiver { get; }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        ReceiveType Type { get; }

        /// <summary>
        /// Gets the reply type of the message, if applicable.
        /// </summary>
        ReplyType ReplyType { get; }
    }
}
