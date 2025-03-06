using ClipEditor.Core;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ClipEditor.View
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            saveFolderType.SelectedIndex = (int)SettingsManager.OutputSaveType;

            updateCheck.IsChecked = SettingsManager.CheckForUpdates;

            customFolderPath.Text = SettingsManager.CustomOutputPath;
            customFolderPath.ToolTip = SettingsManager.CustomOutputPath;
        }

        private void saveFolderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OutputSaveFolderType type = (OutputSaveFolderType)saveFolderType.SelectedIndex;
            SettingsManager.OutputSaveType = type;

            if (type == OutputSaveFolderType.Custom)
            {
                customFolderButton.Visibility = Visibility.Visible;
                customFolderPanel.Visibility = Visibility.Visible;
                customFolderPath.Text = SettingsManager.CustomOutputPath;
            }
            else
            {
                customFolderButton.Visibility = Visibility.Collapsed;
                customFolderPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void updateCheck_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.CheckForUpdates = updateCheck.IsChecked ?? true;
        }
        private void customFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog opf = new();

            bool? success = opf.ShowDialog();
            if (success == true)
            {
                SettingsManager.CustomOutputPath = opf.FolderName;
                customFolderPath.Text = opf.FolderName;
                customFolderPath.ToolTip = opf.FolderName;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
