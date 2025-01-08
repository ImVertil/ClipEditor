using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ClipEditor.Core.Converters
{
    internal class BooleanCustomConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] is not bool || values[1] is not bool || parameter is not string)
                return DependencyProperty.UnsetValue;

            string operation = (string)parameter;
            bool value1 = (bool)values[0];
            bool value2 = (bool)values[1];

            switch(operation)
            {
                case "XOR":
                    return (value1 && !value2) || (!value1 && value2);
                case "NOR":
                    return !(value1 || value2);
                default:
                    return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
