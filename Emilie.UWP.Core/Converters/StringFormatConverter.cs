using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Emilie.UWP.Converters
{
    /// <summary>
    /// Applies String formatting to text. 
    /// Custom formatters {0:U} and {0:L} are supported for upper and lower case
    /// </summary>
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null || value == null)
            {
                return value;
            }

            var workingValue = value;
            var workingParam = parameter;

            bool toUpper = false;
            bool toLower = false;

            // Custom Upper / Lower case string formats
            var uPar = parameter.ToString().ToUpper();
            if (uPar.Contains("{0:U") || uPar.Contains("{0:L"))
            {
                if (uPar.Equals("{0:U}") || uPar.Equals("{0:L}"))
                {
                    return uPar.Contains("{0:U")
                    ? value.ToString().ToUpper()
                    : value.ToString().ToLower();
                }
                else
                {
                    toUpper = uPar.Contains("{0:U");
                    toLower = uPar.Contains("{0:L");
                    workingParam = uPar.Contains("{0:U") ? uPar.Replace("{0:U", "{0:") : uPar.Replace("{0:L", "{0:");
                }
            }

            workingValue = String.Format((String)workingParam, workingValue);

            if (toUpper)
                workingValue = workingValue.ToString().ToUpper();

            if (toLower)
                workingValue = workingValue.ToString().ToLower();

            return workingValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

    }
}
