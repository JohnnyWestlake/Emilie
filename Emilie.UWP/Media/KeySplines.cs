using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

namespace Emilie.UWP.Media
{
    public static class KeySplines
    {
        /// <summary>
        /// Returns a KeySpline for use as an easing function
        /// to replicate the easing of the EntranceThemeTransition
        /// </summary>
        /// <returns></returns>
        public static KeySpline EntranceTheme => 
            Animation.CreateKeySpline(0.1, 0.9, 0.2, 1);

        /// <summary>
        /// A KeySpline that closely matches the default easing curve applied to 
        /// Composition animations by Windows when the developer does not specify
        /// any easing function.
        /// </summary>
        public static KeySpline CompositionDefault =>
            Animation.CreateKeySpline(0.395, 0.56, 0.06, 0.95);

        /// <summary>
        /// Intended for 500 millisecond opacity animation for depth animations
        /// </summary>
        public static KeySpline DepthZoomOpacity => 
            Animation.CreateKeySpline(0.2, 0.6, 0.3, 0.9);

        /// <summary>
        /// A more precise alternative to EntranceTheme KeySpline
        /// </summary>
        public static KeySpline Popup =>
            Animation.CreateKeySpline(0.100000001490116, 0.899999976158142, 0.200000002980232, 1);




        //------------------------------------------------------
        //
        //  Fluent KeySplines
        //
        //------------------------------------------------------

        /* 
            These splines are taken from Microsoft's official animation documentation for 
            fluent animation design system.

            For reference recommended durations are:
                Exit Animations         : 150ms
                Entrance Animations     : 300ms
                Translation Animations  : <= 500ms
        */

        
        /// <summary>
        /// Analogous to Exponential EaseIn, Exponent 4.5
        /// </summary>
        public static KeySpline FluentAccelerate =>
            Animation.CreateKeySpline(0.7, 0, 1, 0.5);

        /// <summary>
        /// Analogous to Exponential EaseOut, Exponent 7
        /// </summary>
        public static KeySpline FluentDecelerate =>
            Animation.CreateKeySpline(0.1, 0.9, 0.2, 1);

        /// <summary>
        /// Analogous to Circle EaseInOut
        /// </summary>
        public static KeySpline FluentStandard =>
            Animation.CreateKeySpline(0.8, 0, 0.2, 1);
    }
}
