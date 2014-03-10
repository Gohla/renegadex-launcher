using AutoMapper;
using RXL.Core;
using RXL.Util;
using RXL.WPFClient.Observables;
using RXL.WPFClient.Utils;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;

namespace RXL.WPFClient.ViewModels
{
    public class ServersViewModel : BaseViewModel, IDisposable
    {
        private readonly ServerList _serverList;
        private readonly Launcher _launcher;
        private readonly PopupService _popupService;
        private readonly JSONStorage _storage;
        private Configuration _configuration;

        private readonly KeyedCollection<String, ServerObservable> _servers;
        private readonly ServersView _serversView;
        private ServerObservable _selectedServer;

        public Configuration Configuration { get { return _configuration; } }
        public IObservableCollection<ServerObservable> Servers { get { return _servers; } }
        public ServersView ServersView { get { return _serversView; } }

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
            _popupService = new PopupService();
            _storage = new JSONStorage();
            if (_storage.Exists("config.json"))
                _configuration = _storage.Read<Configuration>("config.json");
            else
                _configuration = new Configuration();

            _servers = new KeyedCollection<String, ServerObservable>(SynchronizationContext.Current);
            _serversView = new ServersView(_servers);

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

        public void Dispose()
        {
            _storage.Write(_configuration, "config.json");
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
            if (_serversView.CurrentSort.Equals("Latency"))
                _serversView.DisableLiveUpdates("Latency");
            HandlePingResult(result);
            if (_serversView.CurrentSort.Equals("Latency"))
                _serversView.EnableLiveUpdates("Latency");
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
                String name = _configuration.Name;
                if (name == null || name.Equals(String.Empty))
                {
                    await _popupService.ShowMessageBox("No player name",
                        "Please fill in a player name in the top right corner of the application.");
                    return;
                }

                String installLocation = _configuration.InstallLocation;
                if (!IsValidInstallLocation(installLocation))
                {
                    installLocation = _launcher.InstallLocationFromRegistry();
                }
                if (!IsValidInstallLocation(installLocation))
                {
                    installLocation = _launcher.DefaultInstallLocation();
                }
                if (!IsValidInstallLocation(installLocation))
                {
                    await _popupService.ShowMessageBox("Could not determine RenegadeX's installation location",
                        "Could not determine the RenegadeX's installation location, please locate the installation directory in the following popup.");
                    installLocation = await _popupService.ShowFolderDialog("Choose RenegadeX installation location");
                }
                if (!IsValidInstallLocation(installLocation))
                {
                    await _popupService.ShowMessageBox("Invalid installation location", "The specified installation location is invalid.");
                    installLocation = String.Empty;
                    return;
                }

                String password = String.Empty;
                if (server.RequiresPw)
                {
                    password = _popupService.ShowInputDialog("Password required", "Please fill in the server's password.", true);
                    if (password == null)
                        return;
                }

                await _launcher.Launch(installLocation, server.Address, name, password);
            }
        }

        private bool IsValidInstallLocation(String installLocation)
        {
            return installLocation != null && !installLocation.Equals(String.Empty) &&
                _launcher.IsValidInstallLocation(installLocation);
        }
    }
}
