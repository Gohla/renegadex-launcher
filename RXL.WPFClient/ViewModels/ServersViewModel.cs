using AutoMapper;
using RXL.Core;
using RXL.Util;
using RXL.WPFClient.Observables;
using RXL.WPFClient.Utils;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;

namespace RXL.WPFClient.ViewModels
{
    public class ServersViewModel : BaseViewModel
    {
        private readonly ServerList _serverList;
        private readonly Launcher _launcher;

        private readonly KeyedCollection<String, ServerObservable> _servers;
        private readonly ListCollectionView _serversView;
        private ServerObservable _selectedServer;

        private bool _showEmpty = true;
        private bool _showFull = true;
        private bool _showPassworded = true;
        private uint _minPlayers;
        private uint _maxPlayers = 64;
        private uint _maxLatency = 600;
        private String _searchString = String.Empty;
        private string _currentSort = "";
        private bool _sortInverted = true;

        private readonly GenericComparer<ServerObservable, String> _serverNameComparer = new GenericComparer<ServerObservable, String>(s => s.Name);
        private readonly GenericComparer<ServerObservable, uint> _serverLatencyComparer = new GenericComparer<ServerObservable, uint>(s => s.Latency);
        private readonly GenericComparer<ServerObservable, uint> _serverPlayersComparer = new GenericComparer<ServerObservable, uint>(s => s.Players, true);

        public IObservableCollection<ServerObservable> Servers { get { return _servers; } }
        public ListCollectionView ServersView { get { return _serversView; } }

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

        public bool ShowEmpty { get { return _showEmpty; } set { _showEmpty = value; RefreshView(); } }
        public bool ShowFull { get { return _showFull; } set { _showFull = value; RefreshView(); } }
        public bool ShowPassworded { get { return _showPassworded; } set { _showPassworded = value; RefreshView(); } }
        public uint MinPlayers { get { return _minPlayers; } set { _minPlayers = value; RefreshView(); } }
        public uint MaxPlayers { get { return _maxPlayers; } set { _maxPlayers = value; RefreshView(); } }
        public uint MaxLatency { get { return _maxLatency; } set { _maxLatency = value; RefreshView(); } }
        public String SearchString { get { return _searchString; } set { _searchString = value; RefreshView(); } }

        public bool SortInverted
        {
            get { return _sortInverted; }
            set
            {
                _sortInverted = value;
                RaisePropertyChanged(() => SortInverted);
            }
        }

        public string CurrentSort
        {
            get { return _currentSort; }
            set
            {
                _currentSort = value;
                RaisePropertyChanged(() => CurrentSort);
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

            _serversView = new CollectionViewSource { Source = _servers }.View as ListCollectionView;
            _serversView.Filter = FilterServer;
            _serversView.CustomSort = _serverPlayersComparer;

            CurrentSort = "Players";

            foreach (String liveProperty in new[] { "Name", "Players", "Bots", "MaxPlayers", "RequiresPw", "Map", "Latency" })
            {
                _serversView.LiveFilteringProperties.Add(liveProperty);
                _serversView.LiveSortingProperties.Add(liveProperty);
            }
            _serversView.IsLiveFiltering = true;
            _serversView.IsLiveSorting = true;

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

        private bool FilterServer(Object obj)
        {
            return FilterServer(obj as ServerObservable);
        }

        private bool FilterServer(ServerObservable server)
        {
            if (!_showEmpty && server.Players == 0)
                return false;
            if (!_showFull && server.Players == server.MaxPlayers)
                return false;
            if (!_showPassworded && server.RequiresPw)
                return false;
            if (server.Players < _minPlayers)
                return false;
            if (server.Players > _maxPlayers)
                return false;
            if (server.Latency != uint.MaxValue && server.Latency > _maxLatency)
                return false;
            if (!_searchString.Equals(String.Empty) && !Regex.IsMatch(server.Name, Regex.Escape(_searchString), RegexOptions.IgnoreCase))
                return false;

            return true;
        }

        private void RefreshView()
        {
            _serversView.Refresh();
        }

        public async void DoRefresh()
        {
            IEnumerable<Server> newServers = await _serverList.Refresh();

            ISet<ServerObservable> removedServers = new HashSet<ServerObservable>(_servers.Values);
            foreach (Server serverData in newServers)
            {
                ServerObservable server = Mapper.Map<Server, ServerObservable>(serverData);

                if (_servers.Contains(server.Key))
                {
                    ServerObservable existingServer = _servers[serverData.Address];
                    Mapper.Map(server, existingServer);
                }
                else
                {
                    _servers.Add(server);
                }
                removedServers.Remove(server);
            }

            foreach (ServerObservable serverData in removedServers)
            {
                _servers.Remove(serverData.Key);
            }

            DoPingAll();
        }

        public async void DoPingAll()
        {
            IEnumerable<PingResult> results = await _serverList.Ping(_servers.Keys);
            foreach (PingResult result in results)
            {
                HandlePingResult(result);
            }
        }

        public async void DoPingOne(ServerObservable server)
        {
            PingResult result = await _serverList.PingOne(server.Address);

            // Disable and enable filtering and sorting on latency to prevent servers from moving/disappearing.
            _serversView.LiveFilteringProperties.Remove("Latency");
            _serversView.LiveSortingProperties.Remove("Latency");
            HandlePingResult(result);
            _serversView.LiveFilteringProperties.Add("Latency");
            _serversView.LiveSortingProperties.Add("Latency");
        }

        private void HandlePingResult(PingResult result)
        {
            if (result == null)
                return;

            if (_servers.Contains(result.Address))
            {
                ServerObservable server = _servers[result.Address];
                if (result.Reply.Status == IPStatus.Success)
                    server.Latency = (uint)result.Reply.RoundtripTime;
                else
                    server.Latency = uint.MaxValue;
            }
        }

        public async void DoJoin(ServerObservable server)
        {
            if (server != null)
            {
                await _launcher.Launch(server.Address);
            }
        }

        public void SortServers(string sortBy)
        {
            if (_currentSort.Equals(sortBy))
            {
                ((BaseComparer)_serversView.CustomSort).Invert = SortInverted = !SortInverted;
            }
            else
            {
                switch (sortBy)
                {
                    case "Server":
                        {
                            _serverNameComparer.Invert = SortInverted = false;
                            _serversView.CustomSort = _serverNameComparer;
                        }
                        break;
                    case "Players":
                        {
                            _serverPlayersComparer.Invert = SortInverted = true;
                            _serversView.CustomSort = _serverPlayersComparer;
                        }
                        break;
                    case "Latency":
                        {
                            _serverLatencyComparer.Invert = SortInverted = false;
                            _serversView.CustomSort = _serverLatencyComparer;
                        }
                        break;
                }
                CurrentSort = sortBy;
            }

            RefreshView();
        }
    }
}
