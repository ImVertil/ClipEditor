using ClipEditor.ViewModel;
using System.IO;
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

            base.OnStartup(e);
        }
    }
}
