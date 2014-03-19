using System;
using Gohla.Shared;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Interface representing channels on an IRC server.
    /// </summary>
    public interface IChannel : IMessageTarget, IDisposable, IKeyedObject<String>, IComparable<IChannel>, 
        IEquatable<IChannel>
    {
        /// <summary>
        /// Gets an observable stream of messages received by this channel.
        /// </summary>
        IObservable<IReceiveMessage> ReceivedMessages { get; }

        /// <summary>
        /// Gets the users in this channel.
        /// </summary>
        IObservableCollection<IChannelUser> Users { get; }

        /// <summary>
        /// Gets the modes of the channel.
        /// </summary>
        Mode Modes { get; }

        /// <summary>
        /// Gets the topic of the channel. Initially set to empty string. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<String> Topic { get; }

        /// <summary>
        /// Gets the user that set the topic. Initially set to null. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<IUser> TopicSetBy { get; }

        /// <summary>
        /// Gets the date when this topic was set. Initially set to 0. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<uint> TopicSetDate { get; }

        /// <summary>
        /// Gets the date at which the channel was created. Initially set to 0. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<uint> CreatedDate { get; }

        /// <summary>
        /// Query if the channel contains the user with given nickname.
        /// </summary>
        ///
        /// <param name="nickname">The nickname.</param>
        ///
        /// <returns>
        /// True if channel contains the user, false if not.
        /// </returns>
        bool ContainsUser(String nickname);

        /// <summary>
        /// Gets the channel user with given nickname.
        /// </summary>
        ///
        /// <param name="nickname">The nickname.</param>
        ///
        /// <returns>
        /// The channel user.
        /// </returns>
        IChannelUser GetUser(String nickname);
    }
}
