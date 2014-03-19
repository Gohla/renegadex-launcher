using System;
using Gohla.Shared;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Interface representing an identity on IRC.
    /// </summary>
    public interface IIdentity : IDisposable, IEquatable<IIdentity>, IComparable<IIdentity>
    {
        /// <summary>
        /// Gets the name. Initially set to nickname of user. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<String> Name { get; }

        /// <summary>
        /// Gets the identifier. Initially the empty string. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<String> Ident { get; }

        /// <summary>
        /// Gets the host. Initially the empty string. Subscribe to receive changes.
        /// </summary>
        ObservableProperty<String> Host { get; }
    }
}
