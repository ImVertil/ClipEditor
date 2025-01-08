using System.Windows;

namespace ClipEditor.Core.Dialogs
{
    internal static class DialogManager
    {
        public static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
