using System;
using System.Collections.Generic;
using Gohla.Shared;
using ReactiveIRC.Interface;

namespace ReactiveIRC.Client
{
    public class ChannelUser : IChannelUser
    {
        public IChannel Channel { get; private set; }
        public IUser User { get; private set; }
        public Mode Modes { get; private set; }

        public IClientConnection Connection { get; private set; }
        public MessageTargetType Type { get { return MessageTargetType.ChannelUser; } }
        public ObservableProperty<String> Name { get { return User.Name; } }

        public String Key { get { return Name; } }

        public ChannelUser(IClientConnection connection, IChannel channel, IUser user)
        {
            Connection = connection;
            Channel = channel;
            User = user;
            Modes = new Mode();
        }

        public void Dispose()
        {
            if(Modes == null)
                return;

            Modes.Dispose();
            Modes = null;
        }

        public int CompareTo(IChannel other)
        {
            if(ReferenceEquals(other, null))
                return 1;

            int result = 0;
            result = this.Channel.CompareTo(other);
            return result;
        }

        public int CompareTo(IUser other)
        {
            if(ReferenceEquals(other, null))
                return 1;

            int result = 0;
            result = this.User.CompareTo(other);
            return result;
        }

        public int CompareTo(IChannelUser other)
        {
            if(ReferenceEquals(other, null))
                return 1;

            int result = 0;
            result = this.Channel.CompareTo(other.Channel);
            if(result == 0)
                result = this.User.CompareTo(other.User);
            return result;
        }

        public override bool Equals(object other)
        {
            if(ReferenceEquals(other, null))
                return false;

            if(other is IChannel)
                return Equals(other as IChannel);
            else if(other is IUser)
                return Equals(other as IUser);
            else
                return Equals(other as IChannelUser);
        }

        public bool Equals(IChannel other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return
                EqualityComparer<IChannel>.Default.Equals(this.Channel, other)
             ;
        }

        public bool Equals(IUser other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return
                EqualityComparer<IUser>.Default.Equals(this.User, other)
             ;
        }

        public bool Equals(IChannelUser other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return
                EqualityComparer<IChannel>.Default.Equals(this.Channel, other.Channel)
             && EqualityComparer<IUser>.Default.Equals(this.User, other.User)
             ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + EqualityComparer<IChannel>.Default.GetHashCode(this.Channel);
                hash = hash * 23 + EqualityComparer<IUser>.Default.GetHashCode(this.User);
                return hash;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
