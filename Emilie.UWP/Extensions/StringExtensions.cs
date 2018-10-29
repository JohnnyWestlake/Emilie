using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Emilie.UWP.Extensions
{
    public static class StringExtensions
    {
        public static Task<InMemoryRandomAccessStream> AsIRandomAccessStreamAsync(this string data)
        {
            return Task.Run(() =>
            {
                return Encoding.UTF8.GetBytes(data).AsBuffer().AsInMemoryRandomAccessStreamAsync();
            });
        }
    }
}
