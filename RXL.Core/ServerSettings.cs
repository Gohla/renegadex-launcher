using System;

namespace RXL.Core
{
    public class ServerSettings
    {
        public uint MaxPlayers { get; set; }
        public uint VehicleLimit { get; set; }
        public uint MineLimit { get; set; }
        public bool SpawnCrates { get; set; }
        public bool RespawnCrates { get; set; }
        public bool AutoBalance { get; set; }
        public String TimeLimit { get; set; }
        public bool AllowPM { get; set; }
        public bool PMTeamOnly { get; set; }
        public bool SteamRequired { get; set; }
        public String Version { get; set; }
    }
}
