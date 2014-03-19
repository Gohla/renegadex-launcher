using System;

namespace ReactiveIRC.Interface
{
    public static class ClientExtensions
    {
        public static ISendMessage CreateSendMessage(this IClient client, IClientConnection connection, 
            String prefixHeader, String contents, String postfixHeader, SendType type, 
            params IMessageTarget[] receivers)
        {
            return client.CreateSendMessage(connection, prefixHeader, contents, postfixHeader, type, 
                receivers);
        }

        public static ISendMessage CreateSendMessage(this IClient client, ISendMessage message, String newContents)
        {
            return client.CreateSendMessage(message.Connection, message.PrefixHeader, newContents,
                message.PostfixHeader, message.Type, message.Receivers);
        }
    }
}
