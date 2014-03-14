using AutoMapper;
using RXL.Core;
using RXL.Util;
using RXL.WPFClient.Observables;
using RXL.WPFClient.Utils;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace RXL.WPFClient.ViewModels
{
    public class ServersViewModel : BaseViewModel, IDisposable
    {
        private readonly ServerList _serverList;
        private readonly Launcher _launcher;
        private readonly PopupService _popupService;
        private readonly JSONStorage _storage;
        private readonly Configuration _configuration;

        private readonly KeyedCollection<String, ServerObservable> _servers;
        private readonly ServersView _serversView;
        private ServerObservable _selectedServer;

        private readonly RelayCommand _refresh;
        private readonly RelayCommand _pingAll;
        private readonly RelayCommand _pingSelected;
        private readonly RelayCommand _joinSelected;
        private readonly RelayCommand _copyAddressSelected;

        public Configuration Configuration { get { return _configuration; } }
        public IObservableCollection<ServerObservable> Servers { get { return _servers; } }
        public ServersView ServersView { get { return _serversView; } }

        public ServerObservable SelectedServer
        {
            get { return _selectedServer; }
            set
            {
                if (SetField(ref _selectedServer, value, () => SelectedServer))
                {
                    _pingSelected.NotifyCanExecuteChanged(_selectedServer);
                    _joinSelected.NotifyCanExecuteChanged(_selectedServer);
                    _copyAddressSelected.NotifyCanExecuteChanged(_selectedServer);
                }
            }
        }

        public ICommand Refresh { get { return _refresh; } }
        public ICommand PingAll { get { return _pingAll; } }
        public ICommand PingSelected { get { return _pingSelected; } }
        public ICommand JoinSelected { get { return _joinSelected; } }
        public ICommand CopyAddressSelected { get { return _copyAddressSelected; } }

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

            _refresh = new RelayCommand(_ => true, _ => DoRefresh());
            _pingAll = new RelayCommand(_ => true, _ => DoPingAll());
            _pingSelected = new RelayCommand(_ => _selectedServer != null, _ => DoPingOne(_selectedServer));
            _joinSelected = new RelayCommand(_ => _selectedServer != null, _ => DoJoin(_selectedServer));
            _copyAddressSelected = new RelayCommand(_ => _selectedServer != null, _ => CopyAddress(_selectedServer));

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
            Exception exception = null;
            try
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
            catch (Exception e) { exception = e; }

            if (exception != null)
                await _popupService.ShowMessageBox("Error refreshing server list",
                    "Could not refresh the server list. " + exception.Message, MessageBoxImage.Error);
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
                        "Please fill in a player name in the top right corner of the application.", MessageBoxImage.Hand);
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
                    MessageBoxResult result = await _popupService.ShowMessageBox(
                        "Could not determine RenegadeX's installation location",
                        "Could not determine the RenegadeX's installation location, please locate the installation directory in the following popup.",
                        MessageBoxImage.Question, MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                        installLocation = await _popupService.ShowFolderDialog("Choose RenegadeX installation location");
                    else
                    {
                        installLocation = String.Empty;
                        return;
                    }
                }
                if (!IsValidInstallLocation(installLocation))
                {
                    await _popupService.ShowMessageBox("Invalid installation location",
                        "The specified installation location is invalid.", MessageBoxImage.Error);
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

                Exception exception = null;
                try
                {
                    await _launcher.Launch(installLocation, server.Address, name, password);
                }
                catch (Exception e) { exception = e; }

                if (exception != null)
                    await _popupService.ShowMessageBox("Error launching game", "Could not launch the game. " +
                        exception.Message, MessageBoxImage.Error);
            }
        }

        private bool IsValidInstallLocation(String installLocation)
        {
            return installLocation != null && !installLocation.Equals(String.Empty) &&
                _launcher.IsValidInstallLocation(installLocation);
        }

        public void CopyAddress(ServerObservable server)
        {
            Clipboard.SetText(server.Address);
        }
    }
}
