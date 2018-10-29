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
        None = 0,
        /// <summary>
        /// Serializer supports serializing to and from Strings
        /// </summary>
        String = 1,
        /// <summary>
        /// Serializer supports serializing to and from Streams
        /// </summary>
        Stream = 2,
        /// <summary>
        /// [NOT IMPLEMENETD] - For future use only.
        /// </summary>
        All = 4,
    }
}
