using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Emilie.Core.Extensions;

namespace Emilie.Core.Network
{
    public class NetHttpClient : ICoreHttpClient
    {
        private HttpClient _client;

        public NetHttpClient(HttpClient client)
        {
            _client = client;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public Task<CoreHttpResponse> SendAsync(CoreHttpRequest request)
        {
            return SendAsync(request, default);
        }

        public async Task<CoreHttpResponse> SendAsync(CoreHttpRequest request, CancellationToken token)
        {
            CoreHttpResponse response = new CoreHttpResponse();

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(request.Method.ToString()), request.Uri))
            {
                requestMessage.Headers.Merge(request.Options.Headers);
                requestMessage.Properties.Merge(request.Options.RequestProperties);

                HttpContent requestContent = null;
                try
                {
                    token.SafeThrowIfCancellationRequested();

                    if (request.Content != null)
                    {
                        requestMessage.Content = requestContent = new ByteArrayContent(request.Content);
                        requestContent.Headers.Merge(request.Options.ContentHeaders);
                    }

                    using (HttpResponseMessage responseMessage = await _client.SendAsync(requestMessage, token).ConfigureAwait(false))
                    {
                        token.SafeThrowIfCancellationRequested();

                        response.StatusCode = (int)responseMessage.StatusCode;
                        response.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;

                        response.ContentHeaders = responseMessage.Content.Headers.AsDictionary();
                        response.Content = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        response.ResponseHeaders = responseMessage.Headers.AsDictionary();
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
    }
}
