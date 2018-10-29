using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Emilie.UWP.Extensions
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public static class FrameworkElementExtensions
    {
        #region Scroll Related

        public static Double GetScrollOffset(this FrameworkElement element, Orientation orientation)
        {
            ScrollViewer sv = element as ScrollViewer;

            if (sv == null)
                sv = element.GetFirstDescendantOfType<ScrollViewer>();

            if (sv == null) 
                return Double.NaN;

            return (orientation == Orientation.Horizontal) ?
                sv.HorizontalOffset :
                sv.VerticalOffset;
        }

        /// <summary>
        /// Tries to set ScrollPosition of a child ScrollViewer of a framework element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="orientation"></param>
        /// <param name="offset"></param>
        /// <param name="parentPage"></param>
        /// <param name="maxDurationMS"></param>
        /// <returns></returns>
        public static async Task<Boolean> RestoreChildScrollOffsetAsync(this FrameworkElement element, Orientation orientation, double offset, Page parentPage, int maxDurationMS = 1500)
        {
            ScrollViewer sv = null;
            var start = DateTime.Now;

            if (element is ScrollViewer)
                sv = (ScrollViewer)element;

            while (sv == null && parentPage.IsCurrentPage())
            {
                sv = element.GetFirstDescendantOfType<ScrollViewer>();
                if (sv == null)
                {
                    if (((DateTime.Now - start).TotalMilliseconds > maxDurationMS)
                        || element == null)
                        break;
                    await Task.Delay(32);
                }
            }

            if (!parentPage.IsCurrentPage())
                return false;

            return await sv.RestoreScrollOffsetAsync(orientation, offset, parentPage, maxDurationMS);
        }

        /// <summary>
        /// Tries to reset the scroll offset of a ScrollViewer
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="offset"></param>
        /// <param name="maxDurationMS"></param>
        /// <returns></returns>
        public static async Task<Boolean> RestoreScrollOffsetAsync(this ScrollViewer sv, Orientation orientation, double offset, Page parentPage, int maxDurationMS = 1500)
        {
            if (sv == null) return false;

            var start = DateTime.Now;
            Double extentSize = (orientation == Orientation.Vertical) ? sv.ExtentHeight : sv.ExtentWidth;

            while (extentSize < offset && parentPage.IsCurrentPage())
            {
                extentSize = (orientation == Orientation.Vertical) ? sv.ExtentHeight : sv.ExtentWidth;

                if (((DateTime.Now - start).TotalMilliseconds > maxDurationMS)
                    || sv == null)
                    break;

                await Task.Delay(32);
            }

            if (!parentPage.IsCurrentPage())
                return false;

            if (!(extentSize < offset) || sv == null)
            {
                if (orientation == Orientation.Vertical)
                    return sv.ChangeView(null, offset, null, true);
                else
                    return sv.ChangeView(offset, null, null, true);
            }

            return false;
        }


        #endregion

        public static Boolean IsCurrentPage(this Page page)
        {
            return (page.Frame.Content == page);
        }

    }
}
