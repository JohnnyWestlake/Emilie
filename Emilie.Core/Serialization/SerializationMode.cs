using System;

namespace Emilie.Core.Serialization
{
    /// <summary>
    /// Defines the types of serialization the serializer supports.
    /// Methods will return NotSupportedExceptions depending on this value
    /// </summary>
    [Flags]
    public enum SerializationMode
    {
        /// <summary>
        /// Serializer supports serializing to and from Strings
        /// </summary>
        String,
        /// <summary>
        /// Serializer supports serializing to and from Streams
        /// </summary>
        Stream,
        /// <summary>
        /// [NOT IMPLEMENETD] - For future use only.
        /// </summary>
        All
    }
}
