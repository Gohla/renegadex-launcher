using System;
using System.Collections.Generic;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Interface representing an IRC message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the connection this message belongs to.
        /// </summary>
        IClientConnection Connection { get; }

        /// <summary>
        /// Gets the contents of the message.
        /// </summary>
        String Contents { get; }
    }
}
