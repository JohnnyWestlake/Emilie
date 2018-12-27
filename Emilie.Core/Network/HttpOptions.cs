using Microsoft.Collections.Extensions;
using System;
using System.Collections.Generic;
using Emilie.Core.Serialization;

namespace Emilie.Core.Network
{

    public enum CacheBehavior
    {
        /// <summary>
        /// Always caches the response, even if the server requests otherwise. 
        /// Does not apply when using <see cref="CacheMode.SystemManaged"/>.
        /// </summary>
        AlwaysIfRequested = 0,
        /// <summary>
        /// Caches if both the <see cref="CacheMode"/> and the server response allow it.
        /// </summary>
        WhenRequestedIfAllowed = 1
    }

    /// <summary>
    /// Additional options for configuring the behavior of an HTTP request.
    /// </summary>
    [QualityBand(QualityBand.Experimental)]
    public class HttpOptions
    {
        //public CacheBehavior CacheBehavior { get; set; }

        public CacheMode CacheMode { get; set; }

        public TimeSpan? CacheExpiry { get; set; }

        public INetworkCache CacheOverride { get; set; }

        public TimeSpan? RequestTimeout { get; set; }

        /// <summary>
        /// By default the cache uses the URI as the DB key. In circumstances where responses could
        /// change based upon headers / authentication / region / etc, a custom key override can
        /// be used to differentiate responses for same URI.
        /// </summary>
        public string CacheKeyOverride { get; set; }

        public HeaderCollection Headers { get; set; }

        public HeaderCollection ContentHeaders { get; set; }

        public ISerializer Serializer { get; set; }

        /// <summary>
        /// A collection of objects that can be set on a <see cref="HttpRequestMessage"/>
        /// and accessed by an underlying <see cref="IHttpFilter"/>
        /// </summary>
        public IDictionary<string, object> RequestProperties { get; set; }
    }
}
