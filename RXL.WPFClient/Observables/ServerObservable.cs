using System;
using RXL.Util;

namespace RXL.WPFClient.Observables
{
    public class ServerObservable : ObservableBase, IKeyedObject<String>
    {
        private String _name;
        private String _address;
        private uint _players;
        private uint _bots;
        private uint _maxPlayers;
        private long _latency;
        private bool _requiresPw;
        private String _map;

        private ServerSettingsObservable _serverSettings;

        public String Name
        {
            get { return _name; }
            set { SetField(ref _name, value, () => Name); }
        }

        public String Address
        {
            get { return _address; }
            set { SetField(ref _address, value, () => Address); }
        }

        public String Key
        {
            get { return Address; }
        }

        public uint Players
        {
            get { return _players; }
            set { SetField(ref _players, value, () => Players); }
        }

        public uint Bots
        {
            get { return _bots; }
            set { SetField(ref _bots, value, () => Bots); }
        }

        public uint MaxPlayers
        {
            get { return _maxPlayers; }
            set { SetField(ref _maxPlayers, value, () => MaxPlayers); }
        }

        public long Latency
        {
            get { return _latency; }
            set { SetField(ref _latency, value, () => Latency); }
        }

        public bool RequiresPw
        {
            get { return _requiresPw; }
            set { SetField(ref _requiresPw, value, () => RequiresPw); }
        }

        public String Map
        {
            get { return _map; }
            set { SetField(ref _map, value, () => Map); }
        }

        public ServerSettingsObservable ServerSettings
        {
            get { return _serverSettings; }
            set { SetField(ref _serverSettings, value, () => ServerSettings); }
        }

        public void Update(ServerObservable server)
        {
            Name = server.Name;
            Address = server.Address;
            Players = server.Players;
            Bots = server.Bots;
            MaxPlayers = server.MaxPlayers;
            RequiresPw = server.RequiresPw;
            Map = server.Map;
            if (ServerSettings != null && server.ServerSettings != null)
            {
                ServerSettings.Update(server.ServerSettings);
                RaisePropertyChanged(() => ServerSettings);
            }
            else
                ServerSettings = server.ServerSettings;
        }

        public override bool Equals(Object other)
        {
            return Equals(other as ServerObservable);
        }

        public bool Equals(ServerObservable other)
        {
            if (other == null)
                return false;
            return Address.Equals(other.Address);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }
    }
}
