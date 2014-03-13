using Ookii.Dialogs.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace RXL.WPFClient.Utils
{
    public class PopupService
    {
        public async Task<MessageBoxResult> ShowMessageBox(String caption, String message, 
            MessageBoxImage image = MessageBoxImage.None, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            return await Task.Run(() => MessageBox.Show(message, caption, buttons, image));
        }

        public String ShowInputDialog(String caption, String request, bool password = false, String intialInput = "")
        {
            InputDialog dialog = new InputDialog(caption, request, password, intialInput);
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                if (password)
                    return dialog.PasswordInput.Password;
                else
                    return dialog.Input.Text;
            }
            else
                return null;
        }

        public async Task<String> ShowFolderDialog(String caption)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true;
            bool? result = await Task.Run(() => dialog.ShowDialog());

            if (result.HasValue && result.Value)
            {
                return dialog.SelectedPath;
            }
            else
            {
                return null;
            }
        }
    }
}
