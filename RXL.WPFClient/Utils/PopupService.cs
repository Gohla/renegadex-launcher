using Ookii.Dialogs.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RXL.WPFClient.Utils
{
    public class PopupService
    {
        public async Task ShowMessageBox(String caption, String message)
        {
            await Task.Run(() => MessageBox.Show(message));
        }

        public String ShowInputDialog(String caption, String request, String intialInput = "")
        {
            InputDialog dialog = new InputDialog(caption, request, intialInput);
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                return dialog.Input.Text;
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
