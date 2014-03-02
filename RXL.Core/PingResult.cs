using System;
using System.Net.NetworkInformation;

namespace RXL.Core
{
    public class PingResult
    {
        public readonly String Address;
        public readonly PingReply Reply;

        public PingResult(String address, PingReply reply)
        {
            Address = address;
            Reply = reply;
        }
    }
}
