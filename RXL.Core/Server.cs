﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RXL.Core
{
    public class Server : INotifyPropertyChanged
    {
        private String _name;
        public String Name { get { return _name; } set { SetField(ref _name, value, "Name"); } }

        private String _address;
        public String Address { get { return _address; } set { SetField(ref _address, value, "Address"); } }

        private uint _players;
        public uint Players { get { return _players; } set { SetField(ref _players, value, "Players"); } }

        private uint _bots;
        public uint Bots { get { return _bots; } set { SetField(ref _bots, value, "Bots"); } }

        private uint _maxPlayers;
        public uint MaxPlayers { get { return _maxPlayers; } set { SetField(ref _maxPlayers, value, "MaxPlayers"); } }

        private uint _ping;
        public uint Ping { get { return _ping; } set { SetField(ref _ping, value, "Ping"); } }

        private bool _requiresPW;
        public bool RequiresPW { get { return _requiresPW; } set { SetField(ref _requiresPW, value, "RequiresPW"); } }

        private String _mapIndex;
        public String MapIndex { get { return _mapIndex; } set { SetField(ref _mapIndex, value, "MapIndex"); } }

        private String _serverSettings;
        public String ServerSettings { get { return _serverSettings; } set { SetField(ref _serverSettings, value, "ServerSettings"); } }

        public void Update(Server server)
        {
            this.Name = server.Name;
            this.Address = server.Address;
            this.Players = server.Players;
            this.Bots = server.Bots;
            this.MaxPlayers = server.MaxPlayers;
            this.Ping = server.Ping;
            this.RequiresPW = server.RequiresPW;
            this.MapIndex = server.MapIndex;
            this.ServerSettings = server.ServerSettings;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, String propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public override string ToString()
        {
            return Address + " - " + Name;
        }

        public override bool Equals(Object other)
        {
            return Equals(other as Server);
        }

        public bool Equals(Server other)
        {
            if (other == null)
                return false;
            return this.Address.Equals(other.Address);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }
    }
}