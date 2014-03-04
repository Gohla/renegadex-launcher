using RXL.WPFClient.Observables;
using RXL.WPFClient.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RXL.WPFClient.Views
{
    public partial class MainWindow : Window
    {
        public ServersViewModel ViewModel
        {
            get { return (ServersViewModel)DataContext; }
        }

        private ServerObservable _lastClickedServer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerBrowserOnSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0)
            {
                _lastClickedServer = null;
                ViewModel.SelectedServer = null;
                return;
            }

            _lastClickedServer = e.AddedItems[0] as ServerObservable;
            ViewModel.SelectedServer = _lastClickedServer;
            ViewModel.DoPingOneSelectedServer();
        }

        private void ServerBrowserOnMouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if(!ViewModel.SelectedServer.Equals(_lastClickedServer))
                throw new ArgumentException("Selected server not ze zame");
            ViewModel.DoJoinSelectedServer();
        }
    }
}
