using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Emilie.UWP.Network
{
    public abstract class HttpFilter : IHttpFilter
    {
        public IHttpFilter InnerFilter { get; protected set; }

        public HttpFilter()
        {
            InnerFilter = new HttpBaseProtocolFilter();
        }

        public HttpFilter(IHttpFilter filter)
        {
            InnerFilter = filter;
        }

        public abstract IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> SendRequestAsync(HttpRequestMessage request);

        public virtual void Dispose()
        {
            InnerFilter.Dispose();
        }
    }
}
