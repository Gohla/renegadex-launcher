using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        return "CnC Field";
                    }
                case "cnc-goldrush":
                    {
                        return "CnC Goldrush";
                    }
                case "cnc-whiteout":
                    {
                        return "CnC Whiteout";
                    }
                case "cnc-islands":
                    {
                        return "CnC Islands";
                    }
                case "cnc-lakeside":
                    {
                        return "CnC Lakeside";
                    }
                case "cnc-mesa_ii":
                    {
                        return "CnC Mesa II";
                    }
                case "cnc-walls_flying":
                    {
                        return "CnC Walls Flying";
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
