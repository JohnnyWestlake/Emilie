using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Emilie.Core.Extensions;
using Emilie.Core.Network;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Emilie.UWP.Network
{
    public class UWPHttpClient : ICoreHttpClient
    {
        HttpClient _client { get; }

        public UWPHttpClient()
        {
            _client = UWPHttpClientFactory.CreateOptimisedClient();
        }

        public UWPHttpClient(HttpClient client)
        {
            _client = client;
        }

        public Task<CoreHttpResponse> SendAsync(CoreHttpRequest request)
        {
            return SendAsync(request, default);
        }

        //public async Task<CoreHttpResponse> SendAsync(CoreHttpRequest request, IProgress<CoreHttpProgress> progress, CancellationToken token)
        public async Task<CoreHttpResponse> SendAsync(CoreHttpRequest request, CancellationToken token)
        {
            CoreHttpResponse response = new CoreHttpResponse();

            //Action<double> d;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(request.Method.ToString()), new Uri(request.Uri)))
            {
                requestMessage.Headers.Merge(request.Options.Headers);
                requestMessage.Properties.Merge(request.Options.RequestProperties);

                //IProgress<HttpProgress> _progressCallback = null;
                //if (progress != null)
                //{
                //    _progressCallback = new Progress<HttpProgress>(p =>
                //    {
                //        if (p.Stage == HttpProgressStage.ReceivingContent)
                //        {
                //            progress.Report(new CoreHttpProgress(((double)p.BytesReceived / (double)p.TotalBytesToReceive) * 100d));
                //        }
                //    });
                //}

                IHttpContent requestContent = null;
                try
                {
                    token.SafeThrowIfCancellationRequested();

                    if (request.Content != null)
                    {
                        requestMessage.Content = requestContent = new HttpBufferContent(request.Content.AsBuffer());
                        requestContent.Headers.Merge(request.Options.ContentHeaders);
                    }

                    using (HttpResponseMessage responseMessage = await _client.SendRequestAsync(requestMessage).AsTask(token).ConfigureAwait(false))
                    {
                        token.SafeThrowIfCancellationRequested();

                        response.StatusCode = (int)responseMessage.StatusCode;
                        response.ResponseHeaders = new HeaderCollection(responseMessage.Headers);
                        response.ContentHeaders = new HeaderCollection(responseMessage.Content.Headers);

                        IBuffer buffer = await responseMessage.Content.ReadAsBufferAsync().AsTask(token).ConfigureAwait(false);
                        response.Content = buffer.ToArray();
                        response.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
                    }
                }
                catch (OperationCanceledException oce)
                {
                    response.Exception = oce;
                }
                catch (Exception ex)
                {
                    response.Exception = ex;
                }
                finally
                {
                    requestContent?.Dispose();
                }
            }

            return response;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
