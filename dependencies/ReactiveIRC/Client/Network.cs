using System;
using System.Collections.Generic;
using Gohla.Shared;
using ReactiveIRC.Interface;

namespace ReactiveIRC.Client
{
    public class Network : INetwork
    {
        public IClientConnection Connection { get; set; }
        public MessageTargetType Type { get { return MessageTargetType.Network; } }
        public ObservableProperty<String> Name { get; private set; }

        public String Key { get { return Name; } }

        public Network(IClientConnection connection, String name)
        {
            Connection = connection;
            Name = new ObservableProperty<String>(name);
        }

        public void Dispose()
        {
            if(Name == null)
                return;

            Name.Dispose();
            Name = null;
        }

        public int CompareTo(INetwork other)
        {
            if(ReferenceEquals(other, null))
                return 1;

            int result = 0;
            result = this.Name.Value.CompareTo(other.Name);
            if(result == 0)
                result = this.Connection.CompareTo(other.Connection);
            return result;
        }

        public override bool Equals(object other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return Equals(other as INetwork);
        }

        public bool Equals(INetwork other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return
                EqualityComparer<String>.Default.Equals(this.Name, other.Name)
             && EqualityComparer<IClientConnection>.Default.Equals(this.Connection, other.Connection)
             ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + EqualityComparer<String>.Default.GetHashCode(this.Name);
                hash = hash * 23 + EqualityComparer<IClientConnection>.Default.GetHashCode(this.Connection);
                return hash;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
