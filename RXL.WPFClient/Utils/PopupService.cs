using Ookii.Dialogs.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace RXL.WPFClient.Utils
{
    public class PopupService
    {
        public async Task ShowMessageBox(String caption, String message)
        {
            await Task.Run(() => MessageBox.Show(message));
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
