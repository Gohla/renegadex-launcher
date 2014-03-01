using RXL.Core;
using RXL.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RXL.WPFClient
{
    public class ServersViewModel
    {
        private readonly ServerList _serverList;

        public IObservableCollection<Server> Servers { get { return _serverList.Servers; } }

        public ICommand Refresh { get; private set; }
        public ICommand Ping { get; private set; }

        public ServersViewModel()
        {
            _serverList = new ServerList();
            _serverList.Refresh();

            Refresh = new RelayCommand(_ => true, _ => _serverList.Refresh());
            Ping = new RelayCommand(_ => true, _ => _serverList.Ping());
        }
    }
}
