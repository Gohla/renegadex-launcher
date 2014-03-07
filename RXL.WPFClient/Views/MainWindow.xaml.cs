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
        private readonly ServersViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new ServersViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void ServerBrowserOnSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0)
            {
                _viewModel.SelectedServer = null;
                return;
            }

            ServerObservable server = e.AddedItems[0] as ServerObservable;
            _viewModel.SelectedServer = server;
            _viewModel.DoPingOne(server);
        }

        private void ServerBrowserOnMouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            FrameworkElement source = e.OriginalSource as FrameworkElement;
            ServerObservable server = source.DataContext as ServerObservable;
            _viewModel.DoJoin(server);
        }

        private void ServerNameOnTextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
