using System;
using Gohla.Shared;

namespace ReactiveIRC.Interface
{
    /// <summary>
    /// Interface representing a network on an IRC server.
    /// </summary>
    public interface INetwork : IMessageTarget, IDisposable, IKeyedObject<String>, IComparable<INetwork>, 
        IEquatable<INetwork>
    {

    }
}
