using System;
using System.Windows;

namespace RXL.WPFClient.Utils
{
    public partial class InputDialog : Window
    {
        public InputDialog(String caption, String request, bool password, String input)
        {
            InitializeComponent();

            Title = caption;
            Request.Text = request;

            if (password)
            {
                Input.Visibility = Visibility.Collapsed;
                PasswordInput.Password = input;
            }
            else
            {
                PasswordInput.Visibility = Visibility.Collapsed;
                Input.Text = input;
            }
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
