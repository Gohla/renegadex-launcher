using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RXL.WPFClient.Converters
{
    public class SortButtonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var image = (Image)values[0];
            var sortInverted = (bool)values[1];
            var sortBy = (string)values[2];

            if (image.Tag.Equals(sortBy))
            {
                image.Source = sortInverted ? new BitmapImage(new Uri("/Assets/ArrowDown.png", UriKind.Relative)) : new BitmapImage(new Uri("/Assets/ArrowUp.png", UriKind.Relative));
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
