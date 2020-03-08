using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace PizzaApp.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
                return ((ListView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value) + 1;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
