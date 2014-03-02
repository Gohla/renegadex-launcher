using System.Windows;
using System.Windows.Controls;
using RXL.WPFClient.Observables;
using RXL.WPFClient.ViewModels;
using System.Linq;

namespace RXL.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ServersViewModel ViewModel
        {
            get { return (ServersViewModel) DataContext; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedServer = (ServerObservable)e.AddedItems[0];
        }
    }
}
