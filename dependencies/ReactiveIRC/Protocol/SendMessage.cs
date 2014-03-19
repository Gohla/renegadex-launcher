using System;
using System.Collections.Generic;
using ReactiveIRC.Interface;

namespace ReactiveIRC.Protocol
{
    public class SendMessage : ISendMessage
    {
        private readonly HashSet<IMessageTarget> _receivers;

        public IClientConnection Connection { get; private set; }
        public String PrefixHeader { get; private set; }
        public String Contents { get; private set; }
        public String PostfixHeader { get; private set; }
        public IEnumerable<IMessageTarget> Receivers { get { return _receivers; } }
        public SendType Type { get; private set; }

        public SendMessage(IClientConnection connection, String prefixHeader, String contents, String postfixHeader, 
            SendType type, params IMessageTarget[] receivers)
        {
            Connection = connection;
            PrefixHeader = prefixHeader;
            Contents = contents;
            PostfixHeader = postfixHeader;
            _receivers = new HashSet<IMessageTarget>(receivers);
            Type = type;
        }

        public SendMessage(IClientConnection connection, String prefixHeader, String contents, String postfixHeader, 
            SendType type, IEnumerable<IMessageTarget> receivers)
        {
            Connection = connection;
            PrefixHeader = prefixHeader;
            Contents = contents;
            PostfixHeader = postfixHeader;
            _receivers = new HashSet<IMessageTarget>(receivers);
            Type = type;
        }

        public bool ContainsReceiver(IMessageTarget receiver)
        {
            return _receivers.Contains(receiver);
        }

        public override String ToString()
        {
            return String.Concat(PrefixHeader, Contents, PostfixHeader);
        }
    }
}
