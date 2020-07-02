using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;

namespace Emilie.UWP.Attached
{
    public partial class Properties : DependencyObject
    {
        #region PathData

        public static string GetPathData(DependencyObject obj)
        {
            return (string)obj.GetValue(PathDataProperty);
        }

        public static void SetPathData(DependencyObject obj, string value)
        {
            obj.SetValue(PathDataProperty, value);
        }

        public static readonly DependencyProperty PathDataProperty =
            DependencyProperty.RegisterAttached("PathData", typeof(string), typeof(Properties), new PropertyMetadata(null, (d, e) =>
            {
                if (d is Path path)
                {
                    Binding b = new Binding { Source = e.NewValue };
                    path.SetBinding(Path.DataProperty, b);
                }
            }));

        #endregion
    }
}
