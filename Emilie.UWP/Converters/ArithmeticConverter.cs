using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Emilie.UWP.Converters
{
    public sealed class ArithmeticConverter : IValueConverter
    {
        public ArithmeticMode Mode { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
                return value;

            Double val;
            Double param;

            if (Double.TryParse(value.ToString(), out val) && Double.TryParse(parameter.ToString(), out param))
            {
                switch (Mode)
                {
                    case (ArithmeticMode.Addition):
                        return val + param;
                    case (ArithmeticMode.Subtratction):
                        return val - param;
                    case (ArithmeticMode.Multiplication):
                        return val * param;
                    case (ArithmeticMode.Division):
                        return val / param;
                    default:
                        return val;
                }
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
                return value;

            Double val;
            Double param;

            if (Double.TryParse(value.ToString(), out val) && Double.TryParse(parameter.ToString(), out param))
            {
                switch (Mode)
                {
                    case (ArithmeticMode.Addition):
                        return val - param;
                    case (ArithmeticMode.Subtratction):
                        return val + param;
                    case (ArithmeticMode.Multiplication):
                        return val / param;
                    case (ArithmeticMode.Division):
                        return val * param;
                    default:
                        return val;
                }
            }
            else
                return value;
        }
    }

    public enum ArithmeticMode
    {
        Addition,
        Subtratction,
        Multiplication,
        Division
    }
}
