using AutoMapper;
using RXL.Core;
using RXL.Util;
using RXL.WPFClient.Observables;
using RXL.WPFClient.Utils;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Input;

namespace RXL.WPFClient.ViewModels
{
    public class ServersViewModel : BaseViewModel
    {
        private readonly ServerList _serverList;
        private readonly KeyedCollection<String, ServerObservable> _servers;
        private ServerObservable _selectedServer;

        public IObservableCollection<ServerObservable> Servers { get { return _servers; } }

        public ServerObservable SelectedServer
        {
            get { return _selectedServer; }
            set
            {
                _selectedServer = value;
                RaisePropertyChanged(() => SelectedServer);
            }
        }

        public ICommand Refresh { get; private set; }
        public ICommand PingAll { get; private set; }
        public ICommand PingOne { get; private set; }

        public ServersViewModel()
        {
            _serverList = new ServerList();
            _servers = new KeyedCollection<String, ServerObservable>(SynchronizationContext.Current);

            Refresh = new RelayCommand(_ => true, _ => DoRefresh());
            PingAll = new RelayCommand(_ => true, _ => DoPingAll());

            Mapper.CreateMap<Server, ServerObservable>();
            Mapper.CreateMap<ServerSettings, ServerSettingsObservable>();
            Mapper.CreateMap<ServerObservable, ServerObservable>().ForMember(s => s.Latency, opt => opt.Ignore());
            Mapper.AssertConfigurationIsValid();

            DoRefresh();
        }

        public async void DoRefresh()
        {
            IEnumerable<Server> newServers = await _serverList.Refresh();

            ISet<ServerObservable> removedServers = new HashSet<ServerObservable>(_servers.Values);
            foreach(Server serverData in newServers)
            {
                ServerObservable server = Mapper.Map<Server, ServerObservable>(serverData);

                if(_servers.Contains(server.Key))
                {
                    ServerObservable existingServer = _servers[serverData.Address];
                    Mapper.Map<ServerObservable, ServerObservable>(server, existingServer);
                }
                else
                {
                    _servers.Add(server);
                }
                removedServers.Remove(server);
            }

            foreach(ServerObservable serverData in removedServers)
            {
                _servers.Remove(serverData.Key);
            }

            DoPingAll();
        }

        public async void DoPingAll()
        {
            IEnumerable<PingResult> results = await _serverList.Ping(_servers.Keys);

            foreach(PingResult result in results)
            {
                if(result == null)
                    continue;

                if(_servers.Contains(result.Address))
                {
                    ServerObservable server = _servers[result.Address];
                    if(result.Reply.Status == IPStatus.Success)
                        server.Latency = result.Reply.RoundtripTime;
                    else
                        server.Latency = -1;
                }
            }
        }
    }
}
