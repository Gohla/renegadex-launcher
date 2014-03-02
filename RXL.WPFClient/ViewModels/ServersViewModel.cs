using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows.Input;
using AutoMapper;
using RXL.Core;
using RXL.Util;
using RXL.WPFClient.Observables;
using RXL.WPFClient.Utils;

namespace RXL.WPFClient.ViewModels
{
    public class ServersViewModel
    {
        private readonly ServerList _serverList;

        private readonly KeyedCollection<String, ServerObservable> _servers = new KeyedCollection<String, ServerObservable>();
        private ServerObservable _selectedServer;
        public IObservableCollection<ServerObservable> Servers { get { return _servers; } }

        public ServerObservable SelectedServer
        {
            get { return _selectedServer; }
            set { _selectedServer = value; }
        }

        public ICommand Refresh { get; private set; }
        public ICommand Ping { get; private set; }

        public ServersViewModel()
        {
            _serverList = new ServerList();

            Refresh = new RelayCommand(_ => true, _ => DoRefresh());
            Ping = new RelayCommand(_ => true, _ => DoPing());

            Mapper.CreateMap<Server, ServerObservable>();
            Mapper.CreateMap<ServerSettings, ServerSettingsObservable>();
            Mapper.AssertConfigurationIsValid();

            DoRefresh();
        }

        public async void DoRefresh()
        {
            IEnumerable<Server> newServers = await _serverList.Refresh();

            ISet<ServerObservable> removedServers = new HashSet<ServerObservable>(_servers.Values);
            foreach (Server server in newServers)
            {
                ServerObservable serverObservable = Mapper.Map<Server, ServerObservable>(server);

                if (!UpdateServer(serverObservable))
                {
                    AddServer(serverObservable);
                }
                removedServers.Remove(serverObservable);
            }

            foreach (ServerObservable server in removedServers)
            {
                RemoveServer(server.Key);
            }

            DoPing();
        }

        private void AddServer(ServerObservable server)
        {
            _servers.Add(server);
        }

        private bool UpdateServer(ServerObservable server)
        {
            if (_servers.Contains(server.Key))
            {
                ServerObservable existingServer = _servers[server.Address];
                existingServer.Update(server);
                return true;
            }
            return false;
        }

        private void RemoveServer(String key)
        {
            _servers.Remove(key);
        }

        public async void DoPing()
        {
            IEnumerable<PingResult> results = await _serverList.Ping(_servers.Keys);

            foreach (PingResult result in results)
            {
                ServerObservable server = _servers[result.Address];
                if (result.Reply.Status == IPStatus.Success)
                    server.Latency = result.Reply.RoundtripTime;
                else
                    server.Latency = -1;
            }
        }
    }
}
