using System;
using Emilie.Core.Serialization;

namespace Emilie.Core.Network
{
#pragma warning disable RECS0096 // Type parameter is never used : Parameter is used by consumer

    [QualityBand(QualityBand.Experimental, "Liable to resurfacing")]
    public class Endpoint<T> : HttpOptions
    {
        public Uri Uri { get; set; }

        public CoreHttpMethod Method { get; set; } = CoreHttpMethod.Get;

        public Endpoint() { }

        public Endpoint(
            String uri,
            CacheMode mode = CacheMode.None,
            SerializerFormat serializer = SerializerFormat.JSON,
            HeaderCollection headers = null)
        {
            Uri = new Uri(uri, UriKind.Absolute);
            CacheMode = mode;
            Headers = headers;

            // Infer the correct serializer from the endpoint
            switch (serializer)
            {
                case SerializerFormat.XML:
                    Serializer = XML.Default;
                    break;
                default:
                    Serializer = Json.Default;
                    break;
            }
        }

        public Endpoint(
            Uri uri,
            CacheMode mode = CacheMode.None,
            SerializerFormat serializer = SerializerFormat.JSON,
            HeaderCollection headers = null)
        {
            Uri = uri;
            CacheMode = mode;
            Headers = headers;

            // Infer the correct serializer from the endpoint
            switch (serializer)
            {
                case SerializerFormat.XML:
                    Serializer = XML.Default;
                    break;
                default:
                    Serializer = Json.Default;
                    break;
            }
        }

    }


    [QualityBand(QualityBand.Experimental, "Liable to resurfacing")]
    public class RequestEndpoint<TResponse, TRequest> : Endpoint<TResponse>
    {

    }

#pragma warning restore RECS0096 // Type parameter is never used
}
