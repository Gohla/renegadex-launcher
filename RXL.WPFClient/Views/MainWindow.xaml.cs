using RXL.WPFClient.Observables;
using RXL.WPFClient.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RXL.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ServersViewModel ViewModel
        {
            get { return (ServersViewModel)DataContext; }
        }

        private ServerObservable _serverObservableCheck;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerBrowserOnSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedServer = _serverObservableCheck = (ServerObservable)e.AddedItems[0];
            ViewModel.DoPingOneSelectedServer();
        }

        private void ServerBrowserOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!ViewModel.SelectedServer.Equals(_serverObservableCheck))
                throw new ArgumentException("Selected server not ze zame");
            ViewModel.DoJoinSelectedServer();
        }
    }
}
