using System;
using System.Globalization;
using System.Windows.Data;

namespace EktoplazmExtractor.Converters
{
    internal class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((value as bool?) ?? false) == false;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}