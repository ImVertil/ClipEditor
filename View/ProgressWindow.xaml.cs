using System.Windows;

namespace ClipEditor.View
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // should've done a custom window bar but this will do for now B)
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (cancelButton.IsEnabled)
            {
                cancelButton.Command.Execute(null);
                return;
            }

            if (okButton.IsEnabled)
            {
                e.Cancel = false;
            }
        }
    }
}
