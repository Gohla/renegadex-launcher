using System;
using ReactiveIRC.Interface;

namespace ReactiveIRC.Protocol
{
    public class ReceiveMessage : IReceiveMessage
    {
        public IClientConnection Connection { get; private set; }
        public String Contents { get; private set; }
        public DateTime Date { get; private set; }
        public IMessageTarget Sender { get; private set; }
        public IMessageTarget Receiver { get; private set; }
        public ReceiveType Type { get; private set; }
        public ReplyType ReplyType { get; private set; }

        public ReceiveMessage(IClientConnection connection, String contents, DateTime date, IMessageTarget sender, 
            IMessageTarget receiver, ReceiveType type, ReplyType replyType)
        {
            Connection = connection;
            Contents = contents;
            Date = date;
            Sender = sender;
            Receiver = receiver;
            Type = type;
            ReplyType = replyType;
        }

        public override string ToString()
        {
            return Contents;
        }
    }
}
