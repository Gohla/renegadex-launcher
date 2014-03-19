using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Gohla.Shared;
using NLog;
using ReactiveIRC.Interface;
using ReactiveIRC.Protocol;

namespace ReactiveIRC.Client
{
    public class User : IUser
    {
        protected static readonly Logger _logger = NLog.LogManager.GetLogger("User");

        private SynchronizationContext _context;
        private SynchronizedKeyedCollection<String, IChannel> _channels;
        private SynchronizedKeyedCollection<String, IChannel> _knownChannels;

        public IObservable<IReceiveMessage> ReceivedMessages { get; private set; }
        public IObservable<IReceiveMessage> SentMessages { get; private set; }

        public IObservableCollection<IChannel> Channels { get { return _channels; } }
        public IIdentity Identity { get; private set; }
        public ObservableProperty<String> RealName { get; private set; }
        public ObservableProperty<INetwork> Network { get; private set; }
        public Mode Modes { get; private set; }
        public ObservableProperty<bool> Away { get; private set; }

        public IClientConnection Connection { get; private set; }
        public MessageTargetType Type { get { return MessageTargetType.User; } }
        public ObservableProperty<String> Name { get; private set; }

        public String Key { get { return Name; } }

        public User(IClientConnection connection, String name, SynchronizationContext context)
        {
            _context = context;
            _channels = new SynchronizedKeyedCollection<String, IChannel>(_context);
            _knownChannels = new SynchronizedKeyedCollection<String, IChannel>(_context);

            Connection = connection;
            Identity = new Identity(name, null, null);
            RealName = new ObservableProperty<String>(String.Empty);
            Network = new ObservableProperty<INetwork>(null);
            Modes = new Mode();
            Away = new ObservableProperty<bool>(false);
            Name = new ObservableProperty<String>(name);

            ReceivedMessages = connection.ReceivedMessages
                .Where(m => m.Receiver.Equals(this))
                ;
            SentMessages = connection.ReceivedMessages
                .Where(m => m.Sender.Equals(this))
                ;
        }

        public void Dispose()
        {
            if(Name == null)
                return;

            Name.Dispose();
            Name = null;
            Away.Dispose();
            Away = null;
            Modes.Dispose();
            Modes = null;
            Network.Dispose();
            Network = null;
            RealName.Dispose();
            RealName = null;
            Identity.Dispose();
            Identity = null;
            _knownChannels.Clear();
            _knownChannels.Dispose();
            _knownChannels = null;
            _channels.Clear();
            _channels.Dispose();
            _channels = null;
        }

        public bool ContainsChannel(String channelName)
        {
            return _channels.Contains(channelName);
        }

        private bool ContainsKnownChannel(String channelName)
        {
            return _knownChannels.Contains(channelName);
        }

        public IChannel GetChannel(String channelName)
        {
            if(ContainsKnownChannel(channelName))
                return _knownChannels[channelName];

            IChannel channel = Connection.GetChannel(channelName);
            _knownChannels.Add(channel);
            return channel;
        }

        internal void AddChannel(IChannel channel)
        {
            if(ContainsChannel(channel.Name))
                return;

            if(!ContainsKnownChannel(channel.Name))
            {
                _knownChannels.Add(channel);
            }

            _channels.Add(channel);
        }

        internal bool RemoveChannel(String channelName)
        {
            if(!ContainsChannel(channelName))
            {
                _logger.Error("Trying to remove channel " + channelName + " from user " + Name +
                    ", but user is not in this channel.");
                return false;
            }

            _channels.Remove(channelName);
            _knownChannels.Remove(channelName);
            return true;
        }

        public int CompareTo(IUser other)
        {
            if(ReferenceEquals(other, null))
                return 1;

            int result = 0;
            result = this.Name.Value.CompareTo(other.Name);
            return result;
        }

        public override bool Equals(object other)
        {
            /*if(ReferenceEquals(other, null))
                return false;

            return Equals(other as IUser);*/

            return this == other;
        }

        public bool Equals(IUser other)
        {
            /*if(ReferenceEquals(other, null))
                return false;

            return
                EqualityComparer<String>.Default.Equals(this.Name, other.Name)
             && EqualityComparer<IClientConnection>.Default.Equals(this.Connection, other.Connection)
             ;*/

            return this == other;
        }

        /*public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + EqualityComparer<String>.Default.GetHashCode(this.Name.Value);
                hash = hash * 23 + EqualityComparer<IClientConnection>.Default.GetHashCode(this.Connection);
                return hash;
            }
        }*/

        public override string ToString()
        {
            return this.Name;
        }
    }
}
