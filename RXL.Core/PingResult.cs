using System;
using System.Net.NetworkInformation;

namespace RXL.Core
{
    public class PingResult
    {
        public readonly Server Server;
        public readonly PingReply Reply;

        public PingResult(Server server, PingReply reply)
        {
            Server = server;
            Reply = reply;
        }
    }
}
