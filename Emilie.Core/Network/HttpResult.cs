using Microsoft.Collections.Extensions;
using System;

namespace Emilie.Core.Network
{
    /// <summary>
    /// Basic wrapper class for returning results of HttpCalls. 
    /// This version returns only the status code - if returning data
    /// use the generic HttpResult[T] instead.
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// If true, internet connection was available when the call was made
        /// </summary>
        public Boolean WasConnectionAvaliable { get; set; }
        /// <summary>
        /// If true, data has come from the local cache
        /// </summary>
        public Boolean FromCache { get; set; }
        /// <summary>
        /// If true the Http call completed successfully and any additional 
        /// parsing or type conversions succeeded
        /// </summary>
        public Boolean Success { get; set; }
        /// <summary>
        /// If any exception is raised during the call or parsing, it will be
        /// returned here
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// A string representation of the Uri originally passed to HttpClient
        /// (or whatever makes the request in manual implementations)
        /// </summary>
        public String OriginalUri { get; set; }

        /// <summary>
        /// The HttpStatusCode returned by the HttpClient whilst making the call.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Returns the headers returned by the result of the HTTP request.
        /// If from cache, this is expected to be null
        /// </summary>
        public MultiValueDictionary<string, string> ResponseHeaders { get; set; }

        public MultiValueDictionary<string, string> ContentHeaders { get; set; }

        /// <summary>
        /// Throws an exception if <see cref="Success"/> is false.
        /// If <see cref="Exception"/> is not null this is re-thrown, otherwise
        /// a new <see cref="InvalidOperationException"/> is thrown.
        /// </summary>
        public void ThrowIfNotSuccess()
        {
            if (!this.Success)
            {
                if (this.Exception != null)
                    throw this.Exception;

                throw new InvalidOperationException($"HttpResult not success. StatusCode: {this.StatusCode}");
            }
        }

        public static HttpResult CreateBasicCopy(HttpResult original)
        {
            return new HttpResult
            {
                WasConnectionAvaliable = original.WasConnectionAvaliable,
                Success = original.Success,
                Exception = original.Exception,
                FromCache = original.FromCache,
                OriginalUri = original.OriginalUri,
                ResponseHeaders = original.ResponseHeaders,
                StatusCode = original.StatusCode,
                ContentHeaders = original.ContentHeaders
            };
        }
    }

    /// <summary>
    /// A basic generic wrapper class for returning type data
    /// from HTTP calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpResult<T> : HttpResult
    {
        public T Content { get; set; }

        /// <summary>
        /// Returns a type specific HttpResult based on the input result.
        /// The actual Result property is not parsed or set, you must do this manually.
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static HttpResult<TResult> CreateBasedOn<TResult>(HttpResult original)
        {
            return new HttpResult<TResult>
            {
                WasConnectionAvaliable = original.WasConnectionAvaliable,
                Success = original.Success,
                Exception = original.Exception,
                FromCache = original.FromCache,
                OriginalUri = original.OriginalUri,
                ResponseHeaders = original.ResponseHeaders,
                StatusCode = original.StatusCode,
                ContentHeaders = original.ContentHeaders
            };
        }
    }
}
