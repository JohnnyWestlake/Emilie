using System;
using System.Runtime.Serialization;

namespace Emilie.Core
{
    /// <summary>
    /// Similar to ResultCollection, but for loading single-instance or
    /// non-list resources from a network
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[QualityBand(QualityBand.Preview, "ILoadableResult1 support for this UWP variant has not yet been used in a production project")]
    public class ResultItem<T> : BindableBase, ILoadableResult, ILoadableResult1
    {
        public override Boolean AutomaticallyMarshalToDispatcher => true;

        public T Content
        {
            get { return GetV<T>(); }
            set
            {
                Set(value,
                  nameof(Content),
                  nameof(IsEmpty),
                  nameof(IsFirstLoadedEmpty),
                  nameof(IsFirstLoadedWithContent));
            }
        }

        /// <summary>
        /// Arbitrary data field. Set to whatever you want
        /// (analogous to FrameworkElement.Tag). Be careful
        /// of what you put in here - you may cause garbage 
        /// collection to ignore the collection. Use Weak
        /// References if you're not sure.
        /// </summary>
        public object Tag
        {
            get { return Get<object>(null); }
            set { Set(value); }
        }

        /// <summary>
        /// You must set this from wherever is setting the GalleryData. Result collection
        /// has no concept of GalleryData loading... yet.
        /// </summary>
        public Boolean IsLoading
        {
            get { return GetV(false); }
            private set
            {
                Set(value);
                {
                    if (value)
                    {
                        IsFaulted = false;
                        ErrorMessage = null;
                        LoadingId = Guid.NewGuid();
                        SlowLoadingService.StartTracking(this);
                    }
                    else
                    {
                        IsSlowLoading = false;
                    }

                    OnPropertiesChanged(nameof(IsEmptyLoading), nameof(IsSubsequentLoading));
                }
            }
        }

        public Guid LoadingId
        {
            get { return Get(Guid.NewGuid); }
            private set { Set(value); }
        }

        public Boolean IsSlowLoading
        {
            get { return GetV(false); }
            private set { Set(value); }
        }

        public TimeSpan? SlowPeriod
        {
            get { return Get(() => TimeSpan.FromSeconds(5)); }
            set { Set(value); }
        }

        /// <summary>
        /// Do not call method yourself. This should only be called by the SlowLoadingService singleton.
        /// </summary>
        public void SetSlowLoading()
        {
            IsSlowLoading = true;
        }

        /// <summary>
        /// Returns true if the collection is loading with no
        /// items inside it
        /// </summary>
        public Boolean IsEmptyLoading
        {
            get { return IsLoading && IsEmpty; }
        }

        /// <summary>
        /// Returns true if the collection is loading with items
        /// inside it
        /// </summary>
        public Boolean IsSubsequentLoading
        {
            get { return IsLoading && !IsEmpty; }
        }

        /// <summary>
        /// Returns true if the collection has been true through one load operation 
        /// previously.
        /// Do not set this directly (unless you're a serializer, or have a very
        /// good reason for doing so)
        /// </summary>
        public Boolean IsFirstLoaded
        {
            get { return GetV(false); }
            set
            {
                Set(value,
                  nameof(IsFirstLoaded),
                  nameof(IsFirstLoadFaulted),
                  nameof(IsFirstLoadedEmpty),
                  nameof(IsFirstLoadedWithContent));
            }
        }

        public Boolean IsFirstLoadedWithContent
        {
            get { return IsFirstLoaded && !IsFirstLoadedEmpty; }
        }

        public ResultLoadState LoadState
        {
            get { return GetV<ResultLoadState>(ResultLoadState.NotLoaded); }
            set
            {
                Set(value);
                {
                    switch (value)
                    {
                        case ResultLoadState.Loaded:
                            IsFirstLoaded = true;
                            IsLoading = false;
                            break;
                        case ResultLoadState.Loading:
                            IsLoading = true;
                            break;
                        case ResultLoadState.NotLoaded:
                            IsFirstLoaded = false;
                            IsLoading = false;
                            break;
                        case ResultLoadState.FirstLoadFailed:
                        case ResultLoadState.SuccessiveLoadFailed:
                            IsFaulted = true;
                            IsLoading = false;
                            break;
                    }
                }
            }
        }

        public String Title
        {
            get { return Get<String>(); }
            set { Set(value); }
        }

        public String ErrorMessage
        {
            get { return Get<String>(); }
            set { Set(value); }
        }

        public String EmptyMessage
        {
            get { return Get<String>(); }
            set { Set(value); }
        }

        /// <summary>
        /// Returns true if there are no items in the collection
        /// </summary>
        public Boolean IsEmpty
        {
            get { return Content == null || Content.Equals(default(T)); }
        }

        /// <summary>
        /// Returns true if the collection's state has transitioned to loaded,
        /// but there are no items in the collection
        /// </summary>
        public Boolean IsFirstLoadedEmpty
        {
            get { return IsFirstLoaded && IsEmpty; }
        }

        /// <summary>
        /// Returns true is the collection is in the faulted state, and has
        /// not yet successfully been into the loaded state before
        /// </summary>
        public Boolean IsFirstLoadFaulted
        {
            get { return !IsFirstLoaded && IsFaulted; }
        }

        public Boolean IsFaulted
        {
            get { return GetV(false); }
            set
            {
                Set(value,
                  nameof(IsFaulted),
                  nameof(IsFirstLoadFaulted));
            }
        }

        /// <summary>
        /// Sets the relevant LoadState and IsFaulted properties of the collection.
        /// Optional sets an error message you can bind too to display on the UI.
        /// </summary>
        /// <param name="errorMessage"></param>
        public void SetFaulted(String errorMessage = null)
        {
            ErrorMessage = errorMessage;
            IsFaulted = true;

            if (!IsFirstLoaded)
                LoadState = ResultLoadState.FirstLoadFailed;
            else
                LoadState = ResultLoadState.SuccessiveLoadFailed;
        }

        bool _wasStateChangedForSerialize = false;

        [OnSerializing]
        public void PreSerializing()
        {
            // Ensure state is properly preserved for next use
            if (LoadState == ResultLoadState.Loading && !IsFirstLoaded)
            {
                LoadState = ResultLoadState.NotLoaded;
                _wasStateChangedForSerialize = true;
            }
        }

        [OnSerialized]
        public void PostSerializing()
        {
            if (_wasStateChangedForSerialize)
            {
                _wasStateChangedForSerialize = false;

                if (LoadState == ResultLoadState.NotLoaded)
                    LoadState = ResultLoadState.Loading;
            }
        }
    }
}