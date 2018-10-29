using System;
using System.Threading.Tasks;
using System.Threading;

namespace Emilie.Core.Network
{
    [QualityBand(QualityBand.Preview)]
    public static partial class Http
    {
        /* 
        /*
        /* This file contains all the methods related to HttpDelete requests 
        /*
        */

        #region DELETE

        public static Task<HttpResult<T>> DeleteAsync<T>(
            Endpoint<T> endpoint,
            ICoreHttpClient client = null,
            CancellationToken token = default)
        {
            return DeleteAsync<T>(
                endpoint.Uri.AbsoluteUri,
                endpoint,
                client,
                token);
        }

        /// <summary>
        /// Attempts to download and parse Serialized data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Uri"></param>
        /// <param name="serialiser"></param>
        /// <param name="mode"></param>
        /// <param name="cacheExpiry"></param>
        /// <param name="cacheFolderName"></param>
        /// <param name="client"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<HttpResult<T>> DeleteAsync<T>(
            String Uri,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            CancellationToken token = default)
        {
            // 1. Download the data
            var byteResult = await DeleteAsync(Uri, options, client, token).ConfigureAwait(false);

            // 2. Convert from HttpResult<byte[]> to HttpResult<T>
            return await ParseAsync<T>(byteResult, options.Serializer).ConfigureAwait(false);
        }

        public static async Task<HttpResult<Byte[]>> DeleteAsync(
            String Uri,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            CancellationToken token = default)
        {
            EnsureInitialised();
            HttpResult<Byte[]> result = new HttpResult<Byte[]>
            {
                OriginalUri = Uri,
                WasConnectionAvaliable = IsConnectionAvailable,
                Success = false,
                FromCache = false
            };

            if (IsConnectionAvailable)
            {
                try
                {
                    if (client == null)
                        client = Http.NonCachingClient;

                    await SendInternalAsync(Uri, CoreHttpMethod.Delete, options, client, result, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException tce)
                {
                    result.Exception = tce;
                    Logger.Log(tce);
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                    result.Success = false;
                    Logger.Log(ex);
                }
            }

            // 5 - Return our final result!
            return result;
        }

        #endregion
    }
}