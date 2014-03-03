using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RXL.WPFClient.Converters
{
    [ValueConversion(typeof(String), typeof(ImageSource))]
    public class MapImageConverter : IValueConverter
    {
        private Dictionary<String, ImageSource> _images = new Dictionary<String, ImageSource>();

        public MapImageConverter()
        {
            AddImage("cnc-field", "cnc-field.png");
            AddImage("cnc-goldrush", "cnc-goldrush.png");
            AddImage("cnc-whiteout", "cnc-hourglassii.png");
            AddImage("cnc-islands", "cnc-island.png");
            AddImage("cnc-lakeside", "cnc-lakeside.png");
            AddImage("cnc-mesa_ii", "cnc-mesaii.png");
            AddImage("cnc-walls_flying", "cnc-walls.png");
        }

        private void AddImage(String mapName, String imageName)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/RXL.WPFClient;component/Assets/Maps/" + imageName);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            _images.Add(mapName, image);
        }

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            String mapName = (String)value;
            if(_images.ContainsKey(mapName))
                return _images[mapName];
            else
                return _images["cnc-field"];
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
