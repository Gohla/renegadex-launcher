using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveIRC.Interface
{
    public static class MessageTargetExtensions
    {
        /// <summary>
        /// Sends a message to this target.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="message">The message.</param>
        public static void SendMessage(this IMessageTarget target, String message)
        {
            IClientConnection connection = target.Connection;
            connection.SendAndForget(connection.MessageSender.Message(target, message));
        }

        /// <summary>
        /// Sends an action to this target.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="action"> The action.</param>
        public static void SendAction(this IMessageTarget target, String action)
        {
            IClientConnection connection = target.Connection;
            connection.SendAndForget(connection.MessageSender.Action(target, action));
        }

        /// <summary>
        /// Sends a notice to this target.
        /// </summary>
        ///
        /// <param name="channel">The channel to act on.</param>
        /// <param name="notice"> The notice.</param>
        public static void SendNotice(this IMessageTarget target, String notice)
        {
            IClientConnection connection = target.Connection;
            connection.SendAndForget(connection.MessageSender.Notice(target, notice));
        }
    }
}
