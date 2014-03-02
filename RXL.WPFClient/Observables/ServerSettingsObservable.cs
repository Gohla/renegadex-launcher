using System;

namespace RXL.WPFClient.Observables
{
    public class ServerSettingsObservable : ObservableBase
    {
        private uint _maxPlayers;
        private uint _vehicleLimit;
        private uint _mineLimit;
        private bool _spawnCrates;
        private bool _crateRespawn;
        private bool _autoBalance;
        private String _timeLimit;
        private bool _allowPm;
        private bool _pmTeamOnly;
        private bool _steamRequired;
        private String _version;

        public uint MaxPlayers
        {
            get { return _maxPlayers; }
            set { SetField(ref _maxPlayers, value, "MaxPlayers"); }
        }

        public uint VehicleLimit
        {
            get { return _vehicleLimit; }
            set { SetField(ref _vehicleLimit, value, "VehicleLimit"); }
        }

        public uint MineLimit
        {
            get { return _mineLimit; }
            set { SetField(ref _mineLimit, value, "MineLimit"); }
        }

        public bool SpawnCrates
        {
            get { return _spawnCrates; }
            set { SetField(ref _spawnCrates, value, "SpawnCrates"); }
        }

        public bool RespawnCrates
        {
            get { return _crateRespawn; }
            set { SetField(ref _crateRespawn, value, "RespawnCrates"); }
        }

        public bool AutoBalance
        {
            get { return _autoBalance; }
            set { SetField(ref _autoBalance, value, "AutoBalance"); }
        }

        public String TimeLimit
        {
            get { return _timeLimit; }
            set { SetField(ref _timeLimit, value, "TimeLimit"); }
        }

        public bool AllowPm
        {
            get { return _allowPm; }
            set { SetField(ref _allowPm, value, "AllowPM"); }
        }

        public bool PmTeamOnly
        {
            get { return _pmTeamOnly; }
            set { SetField(ref _pmTeamOnly, value, "PMTeamOnly"); }
        }

        public bool SteamRequired
        {
            get { return _steamRequired; }
            set { SetField(ref _steamRequired, value, "SteamRequired"); }
        }

        public String Version
        {
            get { return _version; }
            set { SetField(ref _version, value, "Version"); }
        }

        public void Update(ServerSettingsObservable serverSettings)
        {
            MaxPlayers = serverSettings.MaxPlayers;
            VehicleLimit = serverSettings.VehicleLimit;
            MineLimit = serverSettings.MineLimit;
            SpawnCrates = serverSettings.SpawnCrates;
            RespawnCrates = serverSettings.RespawnCrates;
            AutoBalance = serverSettings.AutoBalance;
            TimeLimit = serverSettings.TimeLimit;
            AllowPm = serverSettings.AllowPm;
            PmTeamOnly = serverSettings.PmTeamOnly;
            SteamRequired = serverSettings.SteamRequired;
            Version = serverSettings.Version;
        }
    }
}
