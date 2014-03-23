using System;
using System.Globalization;
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
            var splitedTag = image.Tag.ToString().Split(':'); // [0] = SortByProperty, [1] = in what order

            if (splitedTag[0].Equals(sortBy) && (sortInverted && splitedTag[1].Equals("Down") || !sortInverted && splitedTag[1].Equals("Up")))
            {
                return new BitmapImage(new Uri(String.Format("/Assets/Arrow{0}Selected.png", splitedTag[1]), UriKind.Relative));
            }

            return new BitmapImage(new Uri(String.Format("/Assets/Arrow{0}.png", splitedTag[1]), UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
