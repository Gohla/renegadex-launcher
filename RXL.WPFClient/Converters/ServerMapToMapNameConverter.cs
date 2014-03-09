using System;
using System.Globalization;
using System.Windows.Data;

namespace RXL.WPFClient.Converters
{
    public class ServerMapToMapNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "cnc-field":
                    {
                        return "C&C Field";
                    }
                case "cnc-goldrush":
                    {
                        return "C&C Goldrush";
                    }
                case "cnc-whiteout":
                    {
                        return "C&C Whiteout";
                    }
                case "cnc-islands":
                    {
                        return "C&C Islands";
                    }
                case "cnc-lakeside":
                    {
                        return "C&C Lakeside";
                    }
                case "cnc-mesa_ii":
                    {
                        return "C&C Mesa II";
                    }
                case "cnc-walls_flying":
                    {
                        return "C&C Walls Flying";
                    }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
