using RXL.WPFClient.Observables;
using RXL.WPFClient.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RXL.WPFClient.Views
{
    public partial class MainWindow : Window, IDisposable
    {
        private readonly ServersViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new ServersViewModel();

            DataContext = _viewModel;
            InitializeComponent();

            Unloaded += (s, ea) => Dispose();
            Dispatcher.ShutdownStarted += (s, ea) => Dispose();
        }

        public void Dispose()
        {
            _viewModel.Dispose();
        }

        private void ServerBrowserOnSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
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

        private void StackPanelMouseUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ServersView.SetServerSorting(((StackPanel)sender).Tag.ToString());
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (FilterOptionsGrid.Visibility == Visibility.Visible)
            {
                FilterOptionsImage.Source = new BitmapImage(new Uri("/Assets/MoreOptions.png", UriKind.Relative));
                FilterOptionsGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                FilterOptionsImage.Source = new BitmapImage(new Uri("/Assets/LessOptions.png", UriKind.Relative));
                FilterOptionsGrid.Visibility = Visibility.Visible;
            }
        }

    }
}
