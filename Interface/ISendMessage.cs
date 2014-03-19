using System;
using System.Collections.Generic;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Interface representing an IRC message that can be sent.
    /// </summary>
    public interface ISendMessage : IMessage
    {
        /// <summary>
        /// Gets the prefix header of the message.
        /// </summary>
        String PrefixHeader { get; }

        /// <summary>
        /// Gets the postfix header of the message.
        /// </summary>
        String PostfixHeader { get; }

        /// <summary>
        /// Gets the receivers of the message. If empty, receiver is global.
        /// </summary>
        IEnumerable<IMessageTarget> Receivers { get; }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        SendType Type { get; }

        /// <summary>
        /// Query if given receiver is one of the receivers of this message.
        /// </summary>
        ///
        /// <param name="receiver">The receiver.</param>
        ///
        /// <returns>
        /// True if given receiver is a receiver of this message, false if not.
        /// </returns>
        bool ContainsReceiver(IMessageTarget receiver);
    }
}
