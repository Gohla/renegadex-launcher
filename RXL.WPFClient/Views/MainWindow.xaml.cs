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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerBrowserOnSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0)
            {
                ViewModel.SelectedServer = null;
                return;
            }

            ViewModel.SelectedServer = e.AddedItems[0] as ServerObservable;
            ViewModel.DoPingOne(ViewModel.SelectedServer);
        }

        private void ServerBrowserOnMouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            FrameworkElement source = e.OriginalSource as FrameworkElement;
            ServerObservable server = source.DataContext as ServerObservable;
            ViewModel.DoJoin(server);
        }
    }
}
