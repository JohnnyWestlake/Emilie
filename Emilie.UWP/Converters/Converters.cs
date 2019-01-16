using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Emilie.UWP.Converters
{
    /// <summary>
    /// Static function converters that can be used with x:Bind
    /// </summary>
    public static class Converters
    {
        /* BOOLEAN */

        public static bool Invert(bool b) => !b;

        public static bool And(bool a, bool b) => a && b;

        public static bool TrueOrTrue(bool a, bool b) => a || b;

        public static bool TrueAndFalse(bool a, bool b) => a && !b;

        public static bool FalseAndFalse(bool a, bool b) => !a && !b;

        public static string ToUpper(object o) => o?.ToString()?.ToUpper();


        /* OBJECT */

        public static bool IsNull(object o) => o is null;

        public static bool IsNotNull(object o) => o != null;

        public static string Format(DateTime? o, string format) => o?.ToString(format);


        /* VISIBILITY */
        // Note: x:Bind natively supports binding bool to vis, so no converter needed.

        public static Visibility InvertToVis(bool b) => !b ? Visibility.Visible : Visibility.Collapsed;

    }
}
