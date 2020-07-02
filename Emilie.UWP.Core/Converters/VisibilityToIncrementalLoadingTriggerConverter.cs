using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Emilie.UWP.Converters
{
    /// <summary>
    /// Sets Incremental loading trigger to Edge ONLY when control
    /// is Visible, otherwise infinite loading requests are fired
    /// (use when using ResultCollection)
    /// </summary>
    public class VisibilityToIncrementalLoadingTriggerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is Visibility))
                return IncrementalLoadingTrigger.None;

            return ((Visibility)value) == Visibility.Visible ? IncrementalLoadingTrigger.Edge : IncrementalLoadingTrigger.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
