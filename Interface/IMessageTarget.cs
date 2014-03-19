using System;
using Gohla.Shared;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Values that represent message target types.
    /// </summary>
    public enum MessageTargetType
    {
        Unknown,
        Network,
        Channel,
        User,
        ChannelUser
    }

    /// <summary>
    /// Interface representing a message target (sender or receiver)
    /// </summary>
    public interface IMessageTarget : IDisposable
    {
        /// <summary>
        /// Gets the connection this target belongs to.
        /// </summary>
        IClientConnection Connection { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        MessageTargetType Type { get; }

        /// <summary>
        /// Gets the name of the target. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<String> Name { get; }
    }
}
