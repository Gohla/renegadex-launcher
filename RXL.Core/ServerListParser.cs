using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RXL.Core
{
    public class ServerListParser
    {
        private const String TAG = "<@>";
        private const char SEPARATOR = '~';

        public Server Parse(String line)
        {
            if(!line.StartsWith(TAG))
                return null;

            String[] data = line.Substring(TAG.Length).Split(SEPARATOR);

            if(data.Length < 7)
                return null;

            try
            {
                Server server = new Server
                {
                    Name = data[0],
                    Address = data[1],
                    Bots = uint.Parse(data[2]),
                    RequiresPW = bool.Parse(data[3]),
                    MapIndex = data[4],
                    ServerSettings = data[5],
                    Players = uint.Parse(data[6]),
                    MaxPlayers = uint.Parse(data[7]),
                };

                return server;
            }
            catch(FormatException)
            {
                return null;
            }
        }
    }
}
