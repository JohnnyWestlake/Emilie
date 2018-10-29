namespace Emilie.Core
{
    /// <summary>
    /// Helpers to describe some common loading states for these types of collections
    /// </summary>
    public enum ResultLoadState
    {
        /// <summary>
        /// Collection has not yet attempted to load
        /// </summary>
        NotLoaded,
        /// <summary>
        /// Collection is loading
        /// </summary>
        Loading,
        /// <summary>
        /// The Collections last loading operation completed successfully
        /// </summary>
        Loaded,
        /// <summary>
        /// The Collections first load operation has just failed
        /// </summary>
        FirstLoadFailed,
        /// <summary>
        /// The Collections Successive load operations have failed
        /// </summary>
        SuccessiveLoadFailed
    }
}
