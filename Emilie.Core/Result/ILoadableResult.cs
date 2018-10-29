using System;

namespace Emilie.Core
{
    /// <summary>
    /// Interface for classes that hold data that loads from a high latency source.
    /// Supports useful error and loading properties
    /// </summary>
    public interface ILoadableResult
    {
        /// <summary>
        /// Arbitrary data field. Set to whatever you want
        /// (analogous to FrameworkElement.Tag). Be careful
        /// of what you put in here - you may cause garbage 
        /// collection to ignore the collection. Use Weak
        /// References if you're not sure.
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// You must set this from wherever is setting the GalleryData. Result collection
        /// has no concept of GalleryData loading... yet.
        /// </summary>
        Boolean IsLoading { get; }

        /// <summary>
        /// Returns true if the collection is loading with no
        /// items inside it
        /// </summary>
        Boolean IsEmptyLoading { get; }

        /// <summary>
        /// Returns true if the collection is loading with items
        /// inside it
        /// </summary>
        Boolean IsSubsequentLoading { get; }

        /// <summary>
        /// Returns true if the collection has been true through one load operation 
        /// previously.
        /// Do not set this directly (unless you're a serializer, or have a very
        /// good reason for doing so)
        /// </summary>
        Boolean IsFirstLoaded { get; set; }

        ResultLoadState LoadState { get; set; }

        String Title { get; set; }

        String ErrorMessage { get; set; }

        String EmptyMessage { get; set; }

        /// <summary>
        /// Returns true if there are no items in the collection
        /// </summary>
        Boolean IsEmpty { get; }

        /// <summary>
        /// Returns true if the collection's state has transitioned to loaded,
        /// but there are no items in the collection
        /// </summary>
        Boolean IsFirstLoadedEmpty { get; }

        /// <summary>
        /// Returns true is the collection is in the faulted state, and has
        /// not yet successfully been into the loaded state before
        /// </summary>
        Boolean IsFirstLoadFaulted { get; }

        Boolean IsFaulted { get; set; }

        /// <summary>
        /// Sets the relevant LoadState and IsFaulted properties of the collection.
        /// Optional sets an error message you can bind too to display on the UI.
        /// </summary>
        /// <param name="errorMessage"></param>
        void SetFaulted(String errorMessage = null);

        void PreSerializing();

        void PostSerializing();
    }


}
