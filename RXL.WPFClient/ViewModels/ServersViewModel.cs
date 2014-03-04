using AutoMapper;
using RunProcessAsTask;
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
        private readonly Launcher _launcher;

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
                PingSelected.NotifyCanExecuteChanged(_selectedServer);
                JoinSelected.NotifyCanExecuteChanged(_selectedServer);
            }
        }

        public RelayCommand Refresh { get; private set; }
        public RelayCommand PingAll { get; private set; }
        public RelayCommand PingSelected { get; private set; }
        public RelayCommand JoinSelected { get; private set; }

        public ServersViewModel()
        {
            _serverList = new ServerList();
            _launcher = new Launcher();

            _servers = new KeyedCollection<String, ServerObservable>(SynchronizationContext.Current);

            Refresh = new RelayCommand(_ => true, _ => DoRefresh());
            PingAll = new RelayCommand(_ => true, _ => DoPingAll());
            PingSelected = new RelayCommand(_ => _selectedServer != null, _ => DoPingOne(_selectedServer));
            JoinSelected = new RelayCommand(_ => _selectedServer != null, _ => DoJoin(_selectedServer));

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
                HandlePingResult(result);
            }
        }

        public async void DoPingOne(ServerObservable server)
        {
            PingResult result = await _serverList.PingOne(server.Address);
            HandlePingResult(result);
        }

        public void DoPingOneSelectedServer()
        {
            DoPingOne(SelectedServer);
        }

        private void HandlePingResult(PingResult result)
        {
            if(result == null)
                return;

            if(_servers.Contains(result.Address))
            {
                ServerObservable server = _servers[result.Address];
                if(result.Reply.Status == IPStatus.Success)
                    server.Latency = result.Reply.RoundtripTime;
                else
                    server.Latency = -1;
            }
        }

        public async void DoJoin(ServerObservable server)
        {
            ProcessResults results = await _launcher.Launch(server.Address);
        }

        public void DoJoinSelectedServer()
        {
            DoJoin(SelectedServer);
        }
    }
}
