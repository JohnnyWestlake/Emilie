using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Emilie.UWP.Attached
{
    [Bindable]
    public partial class Properties : DependencyObject
    {
        #region CornerRadius

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(Properties), new PropertyMetadata(new CornerRadius()));

        #endregion

        #region Thickness

        public static Thickness GetThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ThicknessProperty);
        }

        public static void SetThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ThicknessProperty, value);
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.RegisterAttached("Thickness", typeof(Thickness), typeof(Properties), new PropertyMetadata(new Thickness()));

        #endregion

        #region Double

        public static double GetDouble(DependencyObject obj)
        {
            return (double)obj.GetValue(DoubleProperty);
        }

        public static void SetDouble(DependencyObject obj, double value)
        {
            obj.SetValue(DoubleProperty, value);
        }

        public static readonly DependencyProperty DoubleProperty =
            DependencyProperty.RegisterAttached("Double", typeof(double), typeof(Properties), new PropertyMetadata(0d));

        #endregion
    }
}
