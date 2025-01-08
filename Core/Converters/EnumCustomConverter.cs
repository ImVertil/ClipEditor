using ClipEditor.Model;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ClipEditor.Core.Converters
{
    internal class EnumCustomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is not ClipStatus || parameter is not string)
                return DependencyProperty.UnsetValue;

            if (parameter is string parameterString)
            {
                string[] parameters = parameterString.Split(',');
                return parameters.Contains(((ClipStatus)value).ToString());
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
