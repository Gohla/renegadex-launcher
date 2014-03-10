using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RXL.WPFClient.Utils
{
    public partial class InputDialog : Window
    {
        public InputDialog(String caption, String request, String input)
        {
            InitializeComponent();

            Title = caption;
            Request.Text = request;
            Input.Text = input;
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
