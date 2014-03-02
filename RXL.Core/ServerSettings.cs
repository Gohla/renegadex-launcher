using System;

namespace RXL.Core
{
    public class ServerSettings
    {
        public uint MaxPlayers { get; set; }
        public uint VehicleLimit { get; set; }
        public uint MineLimit { get; set; }
        public bool SpawnCrates { get; set; }
        public TimeSpan CrateRespawnTime { get; set; }
        public bool AutoBalance { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public bool AllowPm { get; set; }
        public bool PmTeamOnly { get; set; }
        public bool SteamRequired { get; set; }
        public String Version { get; set; }
    }
}
