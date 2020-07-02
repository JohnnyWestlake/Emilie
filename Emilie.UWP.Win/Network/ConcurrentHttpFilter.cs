using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Emilie.UWP.Network
{
    /*
     * When using a singleton HttpClient in UWP parallel requests can occasionally
     * result in an underlying exception with the HttpClient that causes the request
     * to fail. This HttpFilter automatically catches the exceptions for this 
     * specific case and retries the request.
     */

    /// <summary>
    /// A base HttpFilter that automatically attempts to mitigate concurrency failures.
    /// </summary>
    public class ConcurrentHttpFilter : HttpFilter
    {
        const string CONCURRENT_EXCEPTION
            = "A concurrent or interleaved operation changed the state of the object, invalidating this operation.";

        public ConcurrentHttpFilter() : base()
        {
        }

        public ConcurrentHttpFilter(IHttpFilter filter) : base(filter)
        {
        }

        public override IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> SendRequestAsync(HttpRequestMessage request)
        {
            return AsyncInfo.Run<HttpResponseMessage, HttpProgress>(async (cancellationToken, progress) =>
            {
                try
                {
                    return await InnerFilter.SendRequestAsync(request).AsTask(cancellationToken, progress);
                }
                catch (Exception ex) when (ex.ToString().Contains(CONCURRENT_EXCEPTION))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return await InnerFilter.SendRequestAsync(request.Clone()).AsTask(cancellationToken);
                }
            });
        }
    }
}
