using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Emilie.UWP.Extensions
{
    public static class MediaExtensions
    {
        /// <summary>
        /// Asynchronously sets the source of a Bitmap Image to a Uri. 
        /// Not Thread safe.
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Task SetUriSourceAsync(this BitmapImage bitmap, string uri)
        {
            return SetUriSourceAsync(bitmap, new Uri(uri, UriKind.Absolute));
        }

        /// <summary>
        /// Asynchronously sets the source of a Bitmap Image to a Uri. 
        /// Not Thread safe.
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task SetUriSourceAsync(this BitmapImage bitmap, Uri uri)
        {
            var _ref = RandomAccessStreamReference.CreateFromUri(uri);
            using (var stream = await _ref.OpenReadAsync())
            {
                await bitmap.SetSourceAsync(stream).AsTask().ConfigureAwait(false);
            }
        }

        public static async Task SetSourceAsync(this BitmapImage bitmap, byte[] bytes)
        {
            using (var stream = await bytes.AsInMemoryRandomAccessStreamAsync())
            {
                await bitmap.SetSourceAsync(stream).ConfigureAwait(false);
            }
        }
    }
}
