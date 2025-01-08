using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ClipEditor.Core.Tools
{
    internal static class Tools
    {
        public static string GetTimeStringFromTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes >= 60)
            {
                return timeSpan.ToString(@"h\:mm\:ss");
            }
            else
            {
                return timeSpan.ToString(@"m\:ss");
            }
        }

        public static DependencyObject GetParentByName(DependencyObject child, string parentName)
        {
            while (child != null)
            {
                if (child is FrameworkElement frameworkElement && frameworkElement.Name == parentName)
                {
                    return child;
                }

                child = VisualTreeHelper.GetParent(child);
            }

            return null;
        }

        public static string GetAppVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            StringBuilder sb = new("v");
            sb.Append(version.Substring(0, version.LastIndexOf('.')));
#if DEBUG
            sb.Append(" dev");
#endif
            return sb.ToString();
        }
    }
}
