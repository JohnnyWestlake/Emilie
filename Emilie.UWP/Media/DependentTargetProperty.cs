using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emilie.UWP.Media
{
    /// <summary>
    /// Strings used to create property paths for CPU bound animate-able properties
    /// </summary>
    public static class DependentTargetProperty
    {
        public static string Margin = "(FrameworkElement.Margin)";
        public static string Padding = "Padding";
        public static string Height = "(FrameworkElement.Height)";
        public static string Width = "(FrameworkElement.Width)";
    }
}
