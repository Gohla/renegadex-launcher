using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RXL.WPFClient
{
    public class ServerSettingsObservable : INotifyPropertyChanged
    {
        private uint _maxPlayers;
        public uint MaxPlayers { get { return _maxPlayers; } set { SetField(ref _maxPlayers, value, "MaxPlayers"); } }

        private uint _vehicleLimit;
        public uint VehicleLimit { get { return _vehicleLimit; } set { SetField(ref _vehicleLimit, value, "VehicleLimit"); } }

        private uint _mineLimit;
        public uint MineLimit { get { return _mineLimit; } set { SetField(ref _mineLimit, value, "MineLimit"); } }

        private bool _spawnCrates;
        public bool SpawnCrates { get { return _spawnCrates; } set { SetField(ref _spawnCrates, value, "SpawnCrates"); } }

        private bool _crateRespawn;
        public bool RespawnCrates { get { return _crateRespawn; } set { SetField(ref _crateRespawn, value, "RespawnCrates"); } }

        private bool _autoBalance;
        public bool AutoBalance { get { return _autoBalance; } set { SetField(ref _autoBalance, value, "AutoBalance"); } }

        private String _timeLimit;
        public String TimeLimit { get { return _timeLimit; } set { SetField(ref _timeLimit, value, "TimeLimit"); } }

        private bool _allowPM;
        public bool AllowPM { get { return _allowPM; } set { SetField(ref _allowPM, value, "AllowPM"); } }

        private bool _pmTeamOnly;
        public bool PMTeamOnly { get { return _pmTeamOnly; } set { SetField(ref _pmTeamOnly, value, "PMTeamOnly"); } }

        private bool _steamRequired;
        public bool SteamRequired { get { return _steamRequired; } set { SetField(ref _steamRequired, value, "SteamRequired"); } }

        private String _version;
        public String Version { get { return _version; } set { SetField(ref _version, value, "Version"); } }

        public void Update(ServerSettingsObservable serverSettings)
        {
            this.MaxPlayers = serverSettings.MaxPlayers;
            this.VehicleLimit = serverSettings.VehicleLimit;
            this.MineLimit = serverSettings.MineLimit;
            this.SpawnCrates = serverSettings.SpawnCrates;
            this.RespawnCrates = serverSettings.RespawnCrates;
            this.AutoBalance = serverSettings.AutoBalance;
            this.TimeLimit = serverSettings.TimeLimit;
            this.AllowPM = serverSettings.AllowPM;
            this.PMTeamOnly = serverSettings.PMTeamOnly;
            this.SteamRequired = serverSettings.SteamRequired;
            this.Version = serverSettings.Version;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, String propertyName)
        {
            if(EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
