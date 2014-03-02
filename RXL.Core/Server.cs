using System;

namespace RXL.Core
{
    public class Server
    {
        public String Name { get; set; }
        public String Address { get; set; }
        public uint Players { get; set; }
        public uint Bots { get; set; }
        public uint MaxPlayers { get; set; }
        public long Latency { get; set; }
        public bool RequiresPw { get; set; }
        public String MapIndex { get; set; }
        public ServerSettings ServerSettings { get; set; }
    }
}
