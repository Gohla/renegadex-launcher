using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Gohla.Shared;
using NLog;
using ReactiveIRC.Interface;
using ReactiveIRC.Protocol;

namespace ReactiveIRC.Client
{
    public class IRCClientConnection : RawClientConnection, IClientConnection
    {
        protected new static readonly Logger _logger = NLog.LogManager.GetLogger("IRCClientConnection");
        protected static readonly String _initialNickname = "***initial***";

        private SynchronizationContext _context;
        private Client _client;
        private MessageSender _messageSender;
        private MessageReceiver _messageReceiver;
        private User _me;
        private SynchronizedKeyedCollection<String, INetwork> _networks;
        private SynchronizedKeyedCollection<String, IChannel> _channels;
        private SynchronizedKeyedCollection<String, IUser> _users;
        private Subject<IMessage> _messages = new Subject<IMessage>();
        private Subject<IReceiveMessage> _internalReceivedMessages = new Subject<IReceiveMessage>();
        private Subject<IReceiveMessage> _receivedMessages = new Subject<IReceiveMessage>();
        private Subject<ISendMessage> _sentMessages = new Subject<ISendMessage>();
        private CompositeDisposable _disposables = new CompositeDisposable();

        public IClient Client { get { return _client; } }
        public IUser Me { get { return _me; } }
        public IObservableCollection<INetwork> Networks { get { return _networks; } }
        public IObservableCollection<IChannel> Channels { get { return _channels; } }
        public IObservableCollection<IUser> Users { get { return _users; } }
        public IObservable<IMessage> Messages { get { return _messages; } }
        public IObservable<IReceiveMessage> ReceivedMessages { get { return _receivedMessages; } }
        public IObservable<ISendMessage> SentMessages { get { return _sentMessages; } }
        public IMessageSender MessageSender { get { return _messageSender; } }

        public IRCClientConnection(String address, ushort port, SynchronizationContext context, Client client)
            : base(address, port)
        {
            _context = context;
            _client = client;
            _networks = new SynchronizedKeyedCollection<String, INetwork>(_context);
            _channels = new SynchronizedKeyedCollection<String, IChannel>(_context);
            _users = new SynchronizedKeyedCollection<String, IUser>(_context);

            _me = GetUser(_initialNickname) as User;

            _messageSender = new MessageSender(this);
            _messageReceiver = new MessageReceiver(this);

            _disposables.Add(RawMessages
                .Select(r => _messageReceiver.Receive(r))
                .Subscribe(_internalReceivedMessages)
                );
            _disposables.Add(_internalReceivedMessages.Subscribe(_receivedMessages));
            _disposables.Add(_receivedMessages.Subscribe(_messages));
            _disposables.Add(_sentMessages.Subscribe(_messages));
            _disposables.Add(_sentMessages
                .Where(m => m.Type == SendType.Privmsg || m.Type == SendType.Notice)
                .Subscribe(m => HandleMessage(m))
            );

            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.Ping)
                .Subscribe(HandlePing)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.Join)
                .Subscribe(HandleJoin)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.Part)
                .Subscribe(HandlePart)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.Kick)
                .Subscribe(HandleKick)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.Quit)
                .Subscribe(HandleQuit)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.NickChange)
                .Subscribe(HandleNickChange)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.TopicChange)
                .Subscribe(HandleTopicChange)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.ChannelModeChange)
                .Subscribe(HandleChannelModeChange)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.Type == ReceiveType.UserModeChange)
                .Subscribe(HandleUserModeChange)
                );

            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.Welcome)
                .Subscribe(HandleWelcome)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.ChannelModeIs)
                .Subscribe(HandleChannelModeIsReply)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.CreatedAt)
                .Subscribe(HandleCreatedAtReply)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.Topic)
                .Subscribe(HandleTopicReply)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.TopicSetBy)
                .Subscribe(HandleTopicSetByReply)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.NamesReply)
                .Subscribe(HandleNamesReply)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.WhoReply)
                .Subscribe(HandleWhoReply)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.Away)
                .Subscribe(HandleAwayReply)
                );
            _disposables.Add(_internalReceivedMessages
                .Where(m => m.ReplyType == ReplyType.UnAway)
                .Subscribe(HandleUnAwayReply)
                );
        }

        new public void Dispose()
        {
            if(_disposables == null)
                return;

            _disposables.Dispose();
            _disposables = null;

            _sentMessages.OnCompleted();
            _sentMessages.Dispose();
            _sentMessages = null;
            _internalReceivedMessages.OnCompleted();
            _internalReceivedMessages.Dispose();
            _internalReceivedMessages = null;
            _messages.OnCompleted();
            _messages.Dispose();
            _messages = null;

            _users.Do(u => u.Dispose());
            _users.Clear();
            _users = null;
            _channels.Do(c => c.Dispose());
            _channels.Clear();
            _channels = null;
            _networks.Do(n => n.Dispose());
            _networks.Clear();
            _networks = null;

            base.Dispose();
        }

        public IObservable<Unit> Send(IEnumerable<ISendMessage> messages)
        {
            messages.Do(m => _sentMessages.OnNext(m));

            return Observable.Merge(
                messages
                    .Select(m => WriteRaw(m.ToString()))
            );
        }

        public void SendAndForget(IEnumerable<ISendMessage> messages)
        {
            messages.Do(m => _sentMessages.OnNext(m));

            _disposables.Add(Observable.Merge(
                messages
                    .Select(m => WriteRaw(m.ToString()))
            ).Subscribe());
        }

        public IObservable<Unit> Login(String nickname)
        {
            return Login(nickname, nickname, nickname);
        }

        public IObservable<Unit> Login(String nickname, String username)
        {
            return Login(nickname, username, username);
        }

        public IObservable<Unit> Login(String nickname, String username, String realname)
        {
            return this.Send
            (
                _messageSender.Nick(nickname)
              , _messageSender.User(username, 0, realname)
            );
        }

        public IObservable<Unit> Login(String nickname, String username, String realname, String password)
        {
            return this.Send
            (
                _messageSender.Pass(password)
              , _messageSender.Nick(nickname)
              , _messageSender.User(username, 0, realname)
            );
        }

        public INetwork GetNetwork(String networkName)
        {
            if(_networks.Contains(networkName))
                return _networks[networkName];

            Network network = new Network(this, networkName);
            _networks.Add(network);
            return network;
        }

        public IChannel GetChannel(String channelName)
        {
            if(_channels.Contains(channelName))
                return _channels[channelName];

            Channel channel = new Channel(this, channelName, _context);
            _channels.Add(channel);
            return channel;
        }

        public IUser GetUser(String nickname)
        {
            if(_users.Contains(nickname))
                return _users[nickname];

            User user = new User(this, nickname, _context);
            _users.Add(user);
            return user;
        }

        public INetwork GetExistingNetwork(String networkName)
        {
            if(!_networks.Contains(networkName))
                return null;
            return _networks[networkName];
        }

        public IChannel GetExistingChannel(String channelName)
        {
            if(!_channels.Contains(channelName))
                return null;
            return _channels[channelName];
        }

        public IUser GetExistingUser(String nickname)
        {
            if(!_users.Contains(nickname))
                return null;
            return _users[nickname];
        }

        private ChannelUser AddUserToChannel(String nickname, String channelName)
        {
            User user = GetChannel(nickname) as User;
            Channel channel = GetUser(channelName) as Channel;
            return AddUserToChannel(user, channel);
        }

        private ChannelUser AddUserToChannel(User user, Channel channel)
        {
            user.AddChannel(channel);
            return channel.AddUser(user);
        }

        private void RemoveUserFromChannel(String nickname, String channelName)
        {
            User user = GetChannel(nickname) as User;
            Channel channel = GetUser(channelName) as Channel;
            RemoveUserFromChannel(user, channel);
        }

        private void RemoveUserFromChannel(User user, Channel channel)
        {
            channel.RemoveUser(user.Name);
            user.RemoveChannel(channel.Name);
        }

        private void RemoveUserFromChannel(ChannelUser channelUser)
        {
            RemoveUserFromChannel(channelUser.User as User, channelUser.Channel as Channel);
        }

        private void ChangeNickname(User user, String nickname)
        {
            if(_users.Contains(nickname))
            {
                _logger.Error("Changing nickname of " + user.Name + " into " + nickname +
                    ", but a user with that nickname already exists. Some observable subscriptions may be lost.");

                if(user.Equals(_me))
                    _me = _users[nickname] as User;
                user.Channels.Cast<Channel>().Do(c => c.ChangeName(user.Name, nickname));
                _users.Remove(user);
                return;
            }

            _users.ChangeItemKey(user, nickname);
            user.Channels.Cast<Channel>().Do(c => c.ChangeName(user.Name, nickname));
            user.Name.Value = nickname;
            user.Identity.Name.Value = nickname;
        }

        private void HandleMessage(ISendMessage message)
        {
            _receivedMessages.OnNext(_messageReceiver.Receive(_me.Identity, message.Contents));
        }

        private void HandlePing(IReceiveMessage message)
        {
            this.SendAndForget(_messageSender.Pong(message.Contents));
        }

        private void HandleJoin(IReceiveMessage message)
        {
            ChannelUser channelUser = message.Sender as ChannelUser;
            Channel channel = message.Receiver as Channel;
            AddUserToChannel(channelUser.User as User, channel);

            if(_me.Equals(channelUser.User))
            {
                this.SendAndForget(
                    _messageSender.Mode(channel)
                  , _messageSender.Who(channel.Name)
                );
            }
            else
            {
                this.SendAndForget(_messageSender.Who(channelUser.Name));
            }
        }

        private void HandlePart(IReceiveMessage message)
        {
            ChannelUser channelUser = message.Sender as ChannelUser;
            Channel channel = message.Receiver as Channel;
            RemoveUserFromChannel(channelUser.User as User, channel);
        }

        private void HandleKick(IReceiveMessage message)
        {
            ChannelUser channelUser = message.Receiver as ChannelUser;
            RemoveUserFromChannel(channelUser);
        }

        private void HandleQuit(IReceiveMessage message)
        {
            User user = message.Sender as User;
            Channel[] channels = user.Channels.Cast<Channel>().ToArray();
            foreach(Channel channel in channels)
            {
                _receivedMessages.OnNext(_client.CreateReceiveMessage(this, message.Contents, message.Date,
                    message.Sender, channel, message.Type, message.ReplyType));
                RemoveUserFromChannel(user, channel);
            }
            _users.Remove(user);
        }

        private void HandleNickChange(IReceiveMessage message)
        {
            User user = message.Sender as User;
            foreach(IChannel channel in user.Channels)
            {
                _receivedMessages.OnNext(_client.CreateReceiveMessage(this, message.Contents, message.Date,
                    message.Sender, channel, message.Type, message.ReplyType));
            }

            ChangeNickname(user, message.Contents);
        }

        private void HandleTopicChange(IReceiveMessage message)
        {
            Channel channel = message.Receiver as Channel;
            channel.Topic.Value = message.Contents;
        }

        private void HandleChannelModeChange(IReceiveMessage message)
        {
            Channel channel = message.Receiver as Channel;

            String[] split = message.Contents.Split(new[] { ' ' }, 2);
            if(split.Length == 1)
            {
                channel.Modes.ParseAndApply(split[0]);
            }
            else
            {
                ModeChange[] changes = Mode.Parse(split[0]);
                String[] nicknames = split[1].Split(' ');

                if(changes.Length != nicknames.Length)
                {
                    _logger.Error("Length of changes does not match length of nicknames.");
                    return;
                }

                for(int i = 0; i < changes.Length; ++i)
                {
                    ChannelUser channelUser = channel.GetUser(nicknames[i]) as ChannelUser;
                    channelUser.Modes.Apply(changes[i]);
                }
            }
        }

        private void HandleUserModeChange(IReceiveMessage message)
        {
            User user = message.Receiver as User;
            user.Modes.ParseAndApply(message.Contents);
        }

        private void HandleWelcome(IReceiveMessage message)
        {
            String nickname = message.Contents.Split(new[] { ' ' }, 2)[0];
            ChangeNickname(_me, nickname);
            _me.Network.Value = message.Sender as Network;
        }

        private void HandleChannelModeIsReply(IReceiveMessage message)
        {
            String[] split = message.Contents.Split(new[] { ' ' }, 3);

            if(split.Length != 3)
            {
                _logger.Error("Channel mode reply with less than 3 parameters.");
                return;
            }

            Channel channel = GetChannel(split[1]) as Channel;
            channel.Modes.ParseAndApply(split[2]);
        }

        private void HandleCreatedAtReply(IReceiveMessage message)
        {
            String[] split = message.Contents.Split(new[] { ' ' }, 3);

            if(split.Length != 3)
            {
                _logger.Error("Channel mode reply with less than 3 parameters.");
                return;
            }

            Channel channel = GetChannel(split[1]) as Channel;
            uint timestamp;
            if(uint.TryParse(split[2], out timestamp))
            {
                channel.CreatedDate.Value = timestamp;
            }
        }

        private void HandleTopicReply(IReceiveMessage message)
        {
            String[] split = message.Contents.Split(new[] { ' ' }, 3);

            if(split.Length != 3)
            {
                _logger.Error("Topic reply with less than 3 parameters.");
                return;
            }

            Channel channel = GetChannel(split[1]) as Channel;
            channel.Topic.Value = split[2].Substring(1);
        }

        private void HandleTopicSetByReply(IReceiveMessage message)
        {
            String[] split = message.Contents.Split(new[] { ' ' }, 4);

            if(split.Length != 4)
            {
                _logger.Error("Topic set by reply with less than 3 parameters.");
                return;
            }

            Channel channel = GetChannel(split[1]) as Channel;
            IIdentity identity = Identity.Parse(split[2]); // TODO: Search by identity and not nickname?
            if(identity != null)
            {
                String nickname = null;
                if(!String.IsNullOrWhiteSpace(identity.Name))
                    nickname = identity.Name;
                if(!String.IsNullOrWhiteSpace(identity.Host)) // For the case where it is a nickname, not an identity.
                    nickname = identity.Host;
                if(nickname != null)
                {
                    IUser user = GetUser(nickname);
                    channel.TopicSetBy.Value = user;
                }
            }
            uint timestamp;
            if(uint.TryParse(split[3], out timestamp))
            {
                channel.TopicSetDate.Value = timestamp;
            }
        }

        private void HandleNamesReply(IReceiveMessage message)
        {
            String[] split = message.Contents.Split(new[] { ' ' }, 4);

            if(split.Length != 4)
            {
                _logger.Error("Names reply with less than 4 parameters.");
                return;
            }

            Channel channel = GetChannel(split[2]) as Channel;
            String[] nicks = split[3].Substring(1).Split(' ');

            foreach(String nick in nicks)
            {
                char mode = ChannelUserMode.SymbolToMode(nick[0]);
                if(mode != char.MinValue)
                {
                    User user = GetUser(nick.Substring(1)) as User;
                    ChannelUser channelUser = AddUserToChannel(user, channel);
                    channelUser.Modes.AddMode(mode);
                }
                else
                {
                    User user = GetUser(nick) as User;
                    ChannelUser channelUser = AddUserToChannel(user, channel);
                }
            }
        }

        private void HandleWhoReply(IReceiveMessage message)
        {
            String[] split = message.Contents.Split(new[] { ' ' }, 9);

            if(split.Length != 9)
            {
                _logger.Error("Who reply with less than 9 parameters.");
                return;
            }

            Channel channel = GetChannel(split[1]) as Channel;
            User user = GetUser(split[5]) as User;
            ChannelUser channelUser = channel.GetUser(user.Name) as ChannelUser;
            user.Identity.Ident.Value = split[2];
            user.Identity.Host.Value = split[3];
            user.Network.Value = GetNetwork(split[4]);

            bool away = false;
            bool serverOp = false;
            bool channelVoice = false;
            bool channelOp = false;
            
            foreach(char c in split[6])
            {
                switch(c)
                {
                    case 'H': away = false; break;
                    case 'G': away = true; break;
                    case '*': serverOp = true; break;
                    case '@': channelOp = true; break;
                    case '+': channelVoice = true; break;
                }
            }

            user.Away.Value = away;

            // TODO: proper server operator mode.
            if(serverOp)
                user.Modes.AddMode('o');
            else
                user.Modes.RemoveMode('o');

            if(channelVoice)
                channelUser.Modes.AddMode('v');
            else
                channelUser.Modes.RemoveMode('v');

            if(channelOp)
                channelUser.Modes.AddMode('o');
            else
                channelUser.Modes.RemoveMode('o');

            // TODO: hop count.

            user.RealName.Value = split[8];
        }

        private void HandleAwayReply(IReceiveMessage message)
        {
            _me.Away.Value = true;
        }

        private void HandleUnAwayReply(IReceiveMessage message)
        {
            _me.Away.Value = false;
        }

        public int CompareTo(IClientConnection other)
        {
            if(ReferenceEquals(other, null))
                return 1;

            int result = 0;
            result = this.Address.CompareTo(other.Address);
            if(result == 0)
                result = this.Port.CompareTo(other.Port);
            return result;
        }

        public override bool Equals(object other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return Equals(other as IClientConnection);
        }

        public bool Equals(IClientConnection other)
        {
            if(ReferenceEquals(other, null))
                return false;

            return
                EqualityComparer<String>.Default.Equals(this.Address, other.Address)
             && EqualityComparer<ushort>.Default.Equals(this.Port, other.Port)
             ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + EqualityComparer<String>.Default.GetHashCode(this.Address);
                hash = hash * 23 + EqualityComparer<ushort>.Default.GetHashCode(this.Port);
                return hash;
            }
        }

        public override string ToString()
        {
            return this.Address.ToString() + ":" + this.Port.ToString();
        }
    }
}
