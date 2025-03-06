using ClipEditor.Core;
using ClipEditor.Core.Dialogs;
using ClipEditor.Core.Tools;
using ClipEditor.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Xabe.FFmpeg;

namespace ClipEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            FFmpeg.SetExecutablesPath(ffmpegPath);

            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel()
            };
            MainWindow.Show();
#if DEBUG
#else
            if (SettingsManager.CheckForUpdates)
            {
                UpdateCheck();
            }
#endif
            base.OnStartup(e);

        }

        private async void UpdateCheck()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                string response = await httpClient.GetStringAsync("https://api.github.com/repos/ImVertil/ClipEditor/releases/latest");
                using JsonDocument doc = JsonDocument.Parse(response);

                Version.TryParse(doc.RootElement.GetProperty("tag_name").GetString()?.TrimStart('v'), out Version? githubVersion);
                Version.TryParse(Tools.GetAppVersion().TrimStart('v'), out Version? localAppVersion);


                if (githubVersion != null && localAppVersion != null && githubVersion > localAppVersion)
                {
                    string msg = $"An update is available!\n\nCurrent: {localAppVersion}\nLatest: {githubVersion}\n\nDo you want to download it now?";
                    var result = MessageBox.Show(msg, "Info", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer", "https://github.com/ImVertil/ClipEditor/releases/latest");
                    }
                }
            } 
            catch (Exception ex) 
            {
                DialogManager.ShowErrorDialog(ex.Message);
            }
        }
    }
}
