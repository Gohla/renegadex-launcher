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

            Server server = new Server
            {
                Name = data[0],
                Address = data[1],
                Bots = ParseUInt(data[2]),
                RequiresPw = ParseBool(data[3]),
                Map = data[4],
                ServerSettings = ParseServerSettings(data[5]),
                Players = ParseUInt(data[6]),
                MaxPlayers = ParseUInt(data[7]),

                Latency = uint.MaxValue,
            };

            return server;
        }

        public ServerSettings ParseServerSettings(String segment)
        {
            String[] data = segment.Split(SETTING_SEPARATOR);

            if(data.Length < 10)
                return null;

            ServerSettings settings = new ServerSettings
            {
                MaxPlayers = ParseUInt(data[0]),
                VehicleLimit = ParseUInt(data[1]),
                MineLimit = ParseUInt(data[2]),
                SpawnCrates = ParseBool(data[3]),
                CrateRespawnTime = TimeSpan.FromSeconds(ParseDouble(data[4])),
                AutoBalance = ParseBool(data[5]),
                TimeLimit = TimeSpan.FromMinutes(ParseDouble(data[6])),
                AllowPm = ParseBool(data[7]),
                PmTeamOnly = ParseBool(data[8]),
                SteamRequired = ParseBool(data[9]),
                Version = data[10],
            };

            return settings;
        }

        private static bool ParseBool(String str, bool def = false)
        {
            bool b;
            if(bool.TryParse(str, out b))
                return b;
            else
                return def;
        }

        private static uint ParseUInt(String str, uint def = 0)
        {
            uint i;
            if(uint.TryParse(str, out i))
                return i;
            else
                return def;
        }

        private static double ParseDouble(String str, double def = 0)
        {
            double d;
            if(double.TryParse(str, out d))
                return d;
            else
                return def;
        }
    }
}
