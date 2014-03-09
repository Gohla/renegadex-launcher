using RXL.WPFClient.Observables;
using RXL.WPFClient.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace RXL.WPFClient.ViewModels
{
    public class ServersView : BaseViewModel
    {
        private readonly ListCollectionView _serversView;

        private bool _showEmpty = true;
        private bool _showFull = true;
        private bool _showPassworded = true;
        private uint _minPlayers;
        private uint _maxPlayers = 64;
        private uint _maxLatency = 600;
        private String _searchString = String.Empty;
        private String _currentSort = String.Empty;
        private bool _sortInverted = true;

        private readonly GenericComparer<ServerObservable, String> _serverNameComparer =
            new GenericComparer<ServerObservable, String>(s => s.Name);
        private readonly GenericComparer<ServerObservable, uint> _serverLatencyComparer =
            new GenericComparer<ServerObservable, uint>(s => s.Latency);
        private readonly GenericComparer<ServerObservable, uint> _serverPlayersComparer =
            new GenericComparer<ServerObservable, uint>(s => s.Players, true);

        public ListCollectionView View { get { return _serversView; } }

        public bool ShowEmpty
        {
            get { return _showEmpty; }
            set
            {
                _showEmpty = value; RefreshView();
            }
        }

        public bool ShowFull
        {
            get { return _showFull; }
            set
            {
                _showFull = value; RefreshView();
            }
        }

        public bool ShowPassworded
        {
            get { return _showPassworded; }
            set
            {
                _showPassworded = value; RefreshView();
            }
        }

        public uint MinPlayers
        {
            get { return _minPlayers; }
            set
            {
                _minPlayers = value; RefreshView();
            }
        }

        public uint MaxPlayers
        {
            get { return _maxPlayers; }
            set
            {
                _maxPlayers = value; RefreshView();
            }
        }

        public uint MaxLatency
        {
            get { return _maxLatency; }
            set
            {
                _maxLatency = value; RefreshView();
            }
        }

        public String SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value; RefreshView();
            }
        }

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

        public ServersView(IList<ServerObservable> servers)
        {
            _serversView = new CollectionViewSource { Source = servers }.View as ListCollectionView;
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

        public void SetServerSorting(String sortBy)
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

        public void DisableLiveUpdates(String propertyName)
        {
            _serversView.LiveFilteringProperties.Remove(propertyName);
            _serversView.LiveSortingProperties.Remove(propertyName);
        }

        public void EnableLiveUpdates(String propertyName)
        {
            _serversView.LiveFilteringProperties.Add(propertyName);
            _serversView.LiveSortingProperties.Add(propertyName);
        }

        public void RefreshView()
        {
            _serversView.Refresh();
        }
    }
}
