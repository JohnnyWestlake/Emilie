using Microsoft.Collections.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Emilie.Core.Network
{
    public interface ICoreHttpClientFactory : IDisposable
    {
        ICoreHttpClient CreateOptimisedCoreClient();
        ICoreHttpClient CreateNonCachingCoreClient();
        bool IsNetworkAvailable();
    }

    public interface ICoreHttpClient : IDisposable
    {
        Task<CoreHttpResponse> SendAsync(CoreHttpRequest request);
        Task<CoreHttpResponse> SendAsync(CoreHttpRequest request, CancellationToken token);
        //Task<CoreHttpResponse> SendAsync(CoreHttpRequest request, IProgress<CoreHttpProgress> progress, CancellationToken token);
    }

    public class CoreHttpRequest
    {
        public CoreHttpRequest() { }

        public CoreHttpRequest(string uri, CoreHttpMethod method, HttpOptions options)
        {
            Uri = uri;
            Method = method;
            Options = options;
        }

        public string Uri { get; set; }
        public CoreHttpMethod Method { get; set; }
        public byte[] Content { get; set; }
        public HttpOptions Options { get; set; }
    }

    public class CoreHttpResponse
    {
        public byte[] Content { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public MultiValueDictionary<string, string> ResponseHeaders { get; set; }
        public MultiValueDictionary<string, string> ContentHeaders { get; set; }
        public Exception Exception { get; set; }
    }

    public enum CoreHttpConnectionState
    {
        Other = 0,
        SendingContent = 1,
        ReceivingContent = 2
    }

    public struct CoreHttpProgress
    {
        public CoreHttpProgress(CoreHttpConnectionState state, ulong processed, ulong? total)
        {
            BytesProcessed = processed;
            TotalBytesToProcess = total;
            State = state;
        }

        /// <summary>
        /// Number of bytes processed for the current connection state
        /// </summary>
        public ulong BytesProcessed { get; }

        /// <summary>
        /// Total number of bytes that are expected to be processed for the current connection state.
        /// When receiving content this may not always be known.
        /// </summary>
        public ulong? TotalBytesToProcess { get; }

        /// <summary>
        /// Connection state for which the values of this progress update reflect.
        /// </summary>
        public CoreHttpConnectionState State { get; }
    }
}
