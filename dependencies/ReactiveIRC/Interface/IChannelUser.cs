using System;
using Gohla.Shared;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Interface representing a user on a certain channel.
    /// </summary>
    public interface IChannelUser : IMessageTarget, IDisposable, IKeyedObject<String>, IComparable<IChannel>, 
        IEquatable<IChannel>, IComparable<IUser>, IEquatable<IUser>, IComparable<IChannelUser>, IEquatable<IChannelUser>
    {
        /// <summary>
        /// Gets the channel.
        /// </summary>
        IChannel Channel { get; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// Gets a the modes that the user has on the channel.
        /// </summary>
        Mode Modes { get; }
    }
}
