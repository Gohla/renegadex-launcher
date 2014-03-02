using System;

namespace RXL.Core
{
    public class ServerListParser
    {
        private const String TAG = "<@>";
        private const char SERVER_SEPARATOR = '~';
        private const char SETTING_SEPARATOR = ';';

        public Server ParseServer(String line)
        {
            if(!line.StartsWith(TAG))
                return null;

            String[] data = line.Substring(TAG.Length).Split(SERVER_SEPARATOR);

            if(data.Length < 7)
                return null;

            try
            {
                Server server = new Server
                {
                    Name = data[0],
                    Address = data[1],
                    Bots = uint.Parse(data[2]),
                    RequiresPw = bool.Parse(data[3]),
                    MapIndex = data[4],
                    ServerSettings = ParseServerSettings(data[5]),
                    Players = uint.Parse(data[6]),
                    MaxPlayers = uint.Parse(data[7]),
                };

                return server;
            }
            catch(FormatException e)
            {
                Console.Error.WriteLine("Could not parse server data: " + e.Message);
                return null;
            }
        }

        public ServerSettings ParseServerSettings(String segment)
        {
            String[] data = segment.Split(SETTING_SEPARATOR);

            if(data.Length < 10)
                return null;

            try
            {
                ServerSettings settings = new ServerSettings
                {
                    MaxPlayers = uint.Parse(data[0]),
                    VehicleLimit = uint.Parse(data[1]),
                    MineLimit = uint.Parse(data[2]),
                    SpawnCrates = bool.Parse(data[3]),
                    RespawnCrates = bool.Parse(data[4]),
                    AutoBalance = bool.Parse(data[5]),
                    TimeLimit = data[6],
                    AllowPm = bool.Parse(data[7]),
                    PmTeamOnly = bool.Parse(data[8]),
                    SteamRequired = bool.Parse(data[9]),
                    Version = data[10],
                };

                return settings;
            }
            catch(FormatException e)
            {
                Console.Error.WriteLine("Could not parse server setting data: " + e.Message);
                return null;
            }
        }
    }
}
