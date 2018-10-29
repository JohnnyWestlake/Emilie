namespace Emilie.Core.Network
{
    /// <summary>
    /// Retrieves standard HTTP methods such as GET and POST and creates new HTTP methods.
    /// </summary>
    public sealed class CoreHttpMethod
    {
        /// <summary>
        /// Initializes a new instance of the Windows.Web.Http.HttpMethod class with a specific
        //     HTTP method.
        /// </summary>
        /// <param name="method"> The HTTP method.</param>
        public CoreHttpMethod(string method)
        {
            Method = method;
        }

        /// <summary>Returns a string that represents the current <see cref="CoreHttpMethod"/> object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Method;

        /// <summary>
        /// The HTTP method.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Returns the HTTP DELETE method.
        /// </summary>
        public static CoreHttpMethod Delete { get; } = new CoreHttpMethod("DELETE");

        /// <summary>
        /// Returns the HTTP GET method.
        /// </summary>
        public static CoreHttpMethod Get { get; } = new CoreHttpMethod("GET");

        /// <summary>
        /// Returns the HTTP HEAD method.
        /// </summary>
        public static CoreHttpMethod Head { get; } = new CoreHttpMethod("HEAD");

        /// <summary>
        /// Returns the HTTP OPTIONS method.
        /// </summary>
        public static CoreHttpMethod Options { get; } = new CoreHttpMethod("OPTIONS");

        /// <summary>
        /// Returns the HTTP PATCH method.
        /// </summary>
        public static CoreHttpMethod Patch { get; } = new CoreHttpMethod("PATCH");

        /// <summary>
        /// Returns the HTTP POST method.
        /// </summary>
        public static CoreHttpMethod Post { get; } = new CoreHttpMethod("POST");

        /// <summary>
        /// Returns the HTTP PUT method.
        /// </summary>
        public static CoreHttpMethod Put { get; } = new CoreHttpMethod("PUT");
    }
}
