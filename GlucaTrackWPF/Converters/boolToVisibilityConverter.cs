using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace GlucaTrackWPF.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class boolToVisibilityConverter : ConverterBase, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility v = ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value == Visibility.Visible) ? true : false;
        }
    }
}
