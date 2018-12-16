//---------------------------------------------------------------------------
//
// <copyright file="ResultCollection.cs" company="Johnny Westlake [UIC]">
//    Based on ObservableCollection by Microsoft (2004)
// </copyright>
//
//
// Description: Implementation of an Collection<T> implementing INotifyCollectionChanged
//              to notify listeners of dynamic changes of the list, and featuring bindable
//              properties useful for web-list based scenarios
//
// See spec at http://avalon/connecteddata/Specs/Collection%20Interfaces.mht
//
// History:
//  10:51 01/12/2014 : [Johnny] - created + initial commit
//
//---------------------------------------------------------------------------

using Emilie.Core.Common;
using Emilie.Core.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emilie.Core
{
    /// <summary>
    /// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;,
    /// implementing INotifyCollectionChanged to notify listeners
    /// when items get added, removed or the whole list is refreshed.
    /// </summary>
    public class IncrementalCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IIncrementalLoading, ILoadableResult
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors
        /// <summary>
        /// Initializes a new instance of ResultCollection that is empty and has default initial capacity.
        /// </summary>
        public IncrementalCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the ResultCollection class
        /// that contains elements copied from the specified list
        /// </summary>
        /// <param name="list">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ResultCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference </exception>
        public IncrementalCollection(List<T> list)
            : base((list != null) ? new List<T>(list.Count) : list)
        {
            // Workaround for VSWhidbey bug 562681 (tracked by Windows bug 1369339).
            // We should be able to simply call the base(list) ctor.  But Collection<T>
            // doesn't copy the list (contrary to the documentation) - it uses the
            // list directly as its storage.  So we do the copying here.
            // 
            CopyFrom(list);
        }

        /// <summary>
        /// Initializes a new instance of the ResultCollection class that contains
        /// elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ResultCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
        public IncrementalCollection(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            CopyFrom(collection);
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            IList<T> items = Items;
            if (collection != null && items != null)
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }

        #endregion Constructors




        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public void ReplaceItems(IList<T> items)
        {
            base.ClearItems();
            foreach (var item in items)
                base.Add(item);

            OnCollectionChangedNotifiers();
            OnCollectionReset();
        }

        public void SilentClear()
        {
            base.ClearItems();
        }

        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Adds a collection of items to a ResultCollection. If adding a large amount, consider using the Reset Notify
        /// type. Use "Add" only if you want UI animations from Theme Transitions
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="notifyType"></param>
        public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notifyType = NotifyCollectionChangedAction.Add)
        {
            IList<T> items = Items;
            if (collection != null && items != null)
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    if (notifyType == NotifyCollectionChangedAction.Add)
                    {
                        while (enumerator.MoveNext())
                        {
                            items.Add(enumerator.Current);
                            OnCollectionChanged(NotifyCollectionChangedAction.Add, enumerator.Current, items.Count - 1);
                        }
                    }
                    else
                    {
                        // Separate / duplicated to avoid checking notifyType every enumeration
                        while (enumerator.MoveNext())
                        {
                            items.Add(enumerator.Current);
                        }

                        if (notifyType == NotifyCollectionChangedAction.Reset)
                            OnCollectionReset();
                    }



                }

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

        #endregion Public Methods




        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------

        #region Public Events

        //------------------------------------------------------
        #region INotifyPropertyChanged implementation
        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }
        #endregion INotifyPropertyChanged implementation


        //------------------------------------------------------
        /// <summary>
        /// Occurs when the collection changes, either by adding or removing an item.
        /// </summary>
        /// <remarks>
        /// see <seealso cref="INotifyCollectionChanged"/>
        /// </remarks>
#if !FEATURE_NETCORE
#endif
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion Public Events




        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when the list is being cleared;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void ClearItems()
        {
            CheckReentrancy();
            base.ClearItems();
            OnCollectionChangedNotifiers();
            OnCollectionReset();
        }

        void OnCollectionChangedNotifiers()
        {
            OnPropertiesChanged(
                nameof(IsFirstLoadedEmpty),
                nameof(IsEmpty),
                CountString,
                IndexerName);
        }


        /// <summary>
        /// Called by base class Collection&lt;T&gt; when an item is removed from list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            CheckReentrancy();
            T removedItem = this[index];

            base.RemoveItem(index);

            OnCollectionChangedNotifiers();

            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when an item is added to list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();
            base.InsertItem(index, item);

            OnCollectionChangedNotifiers();

            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when an item is set in list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            CheckReentrancy();
            T originalItem = this[index];
            base.SetItem(index, item);

            OnCollectionChangedNotifiers();

            OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        /// <summary>
        /// Called by base class ResultCollection&lt;T&gt; when an item is to be moved within the list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();

            T removedItem = this[oldIndex];

            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, removedItem);

            OnCollectionChangedNotifiers();

            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }


        /// <summary>
        /// Raises a PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                MarshallHelper(() => PropertyChanged?.Invoke(this, e));
            }
        }

        protected void OnPropertiesChanged(params string[] properties)
        {
            if (PropertyChanged != null)
            {
                MarshallHelper(() =>
                {
                    foreach (var p in properties)
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
                });
            }
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this ResultCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <remarks>
        /// When overriding this method, either call its base implementation
        /// or call <see cref="BlockReentrancy"/> to guard against reentrant collection changes.
        /// </remarks>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                using (BlockReentrancy())
                {
                    MarshallHelper(() => CollectionChanged?.Invoke(this, e));
                }
            }
        }

        /// <summary>
        /// Disallow reentrant attempts to change this collection. E.g. a event handler
        /// of the CollectionChanged event is not allowed to make changes to this collection.
        /// </summary>
        /// <remarks>
        /// typical usage is to wrap e.g. a OnCollectionChanged call with a using() scope:
        /// <code>
        ///         using (BlockReentrancy())
        ///         {
        ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
        ///         }
        /// </code>
        /// </remarks>
        protected IDisposable BlockReentrancy()
        {
            _monitor.Enter();
            return _monitor;
        }

        /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
        /// <exception cref="InvalidOperationException"> raised when changing the collection
        /// while another collection change is still being notified to other listeners </exception>
        protected void CheckReentrancy()
        {
            if (_monitor.Busy)
            {
                // we can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if ((CollectionChanged != null) && (CollectionChanged.GetInvocationList().Length > 1))
                    throw new InvalidOperationException("Re-entrancy not allowed");
            }
        }

        #endregion Protected Methods




        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods
        /// <summary>
        /// Helper to raise a PropertyChanged event  />).
        /// </summary>
        private void OnPropertyChanged(string propertyName, Boolean forceUIThreadMarshall = false)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event with action == Reset to any listeners
        /// </summary>
        private void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

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

        #endregion Private Methods




        //------------------------------------------------------
        //
        //  Private Types
        //
        //------------------------------------------------------

        #region Private Types

        // this class helps prevent reentrant calls
        private class SimpleMonitor : IDisposable
        {
            public void Enter()
            {
                ++_busyCount;
            }

            public void Dispose()
            {
                --_busyCount;
            }

            public bool Busy { get { return _busyCount > 0; } }

            int _busyCount;
        }

        #endregion Private Types




        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private const string CountString = "Count";

        // This must agree with Binding.IndexerName.  It is declared separately
        // here so as to avoid a dependency on PresentationFramework.dll.
        private const string IndexerName = "Item[]";

        private SimpleMonitor _monitor = new SimpleMonitor();

        private bool _wasStateChangedForSerialize = false;

        #endregion Private Fields




        //------------------------------------------------------
        //
        //  Result Collection Bindable Properties
        //
        //------------------------------------------------------

        #region Result Collection Bindable Properties

        /// <summary>
        /// When set to false, prevents IncrementalLoading from continuing.
        /// </summary>
        public Boolean CanLoadMoreItems
        {
            get { return GetProperty(true); }
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(HasMoreItems));
                }
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
            get { return GetProperty<object>(null); }
            set { SetProperty(value); }
        }

        /// <summary>
        /// You must set this from wherever is setting the GalleryData. Result collection
        /// has no concept of GalleryData loading... yet.
        /// </summary>
        public Boolean IsLoading
        {
            get { return GetProperty(false); }
            private set
            {
                if (SetProperty(value))
                {
                    if (value)
                    {
                        IsFaulted = false;
                        ErrorMessage = null;
                    }

                    OnPropertiesChanged(
                        nameof(IsEmptyLoading),
                        nameof(IsSubsequentLoading),
                        nameof(CanLoadMoreItems));
                }
            }
        }

        /// <summary>
        /// Returns true if the collection is loading with no
        /// items inside it
        /// </summary>
        public Boolean IsEmptyLoading
        {
            get { return IsLoading && Count == 0; }
        }

        /// <summary>
        /// Returns true if the collection is loading with items
        /// inside it
        /// </summary>
        public Boolean IsSubsequentLoading
        {
            get { return IsLoading && this.Count > 0; }
        }

        /// <summary>
        /// Returns true if the collection has been true through one load operation 
        /// previously.
        /// Do not set this directly (unless you're a serializer, or have a very
        /// good reason for doing so)
        /// </summary>
        public Boolean IsFirstLoaded
        {
            get { return GetProperty<Boolean>(false); }
            set
            {
                if (SetProperty(value))
                {
                    OnPropertiesChanged(
                        nameof(IsFirstLoadFaulted),
                        nameof(IsFirstLoadedEmpty));
                }
            }
        }

        public Int32 PageSize
        {
            get { return GetProperty<Int32>(10); }
            set { SetProperty(value); }
        }

        public ResultLoadState LoadState
        {
            get { return GetProperty<ResultLoadState>(ResultLoadState.NotLoaded); }
            set
            {
                if (SetProperty(value))
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

        public Int32 PagingOffset
        {
            get { return GetProperty<Int32>(0); }
            set { SetProperty(value); }
        }



        public String Title
        {
            get { return GetProperty<String>(); }
            set { SetProperty(value); }
        }

        public String SubTitle
        {
            get { return GetProperty<String>(); }
            set { SetProperty(value); }
        }

        public String ErrorMessage
        {
            get { return GetProperty<String>(); }
            set { SetProperty(value); }
        }

        public String EmptyMessage
        {
            get { return GetProperty<String>(); }
            set { SetProperty(value); }
        }

        /// <summary>
        /// Returns true if there are no items in the collection
        /// </summary>
        public Boolean IsEmpty
        {
            get { return this.Items.Count == 0; }
        }

        /// <summary>
        /// Returns true if the collection's state has transitioned to loaded,
        /// but there are no items in the collection
        /// </summary>
        public Boolean IsFirstLoadedEmpty
        {
            get { return IsFirstLoaded && this.Items.Count == 0; }
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
            get { return GetProperty<Boolean>(false); }
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(IsFirstLoadFaulted));
                }
            }
        }

        #endregion




        //------------------------------------------------------
        //
        //  Bindable Base Implementation
        //
        //------------------------------------------------------

        #region BindableBase

        private Dictionary<String, Object> data = new Dictionary<String, Object>();

        /// <summary>
        /// When overridden, allows you to set properties on a background thread and
        /// automatically attempt to fire PropertyChanged and CollectionChanged notifications 
        /// on the dispatcher thread. Defaults to false. (Be aware of any possible threading 
        /// issues this may cause when true - UI thread will be updated notably after the data 
        /// layer)
        /// </summary>
        public Boolean AutomaticallyMarshalToDispatcher = true;

        /// <summary>
        /// Default Priority to use when AutomaticallyMarshalToDispatcher is set to true and 
        /// PropertyChangedNotifications are called from a background thread. 
        /// </summary>
        public DispatcherPriority DefaultMarshalingPriority = DispatcherPriority.Normal;

        /// <summary>
        /// Returns the UI thread Dispatcher
        /// </summary>
        protected IDispatcher Dispatcher
        {
            get { return DispatcherHelper.Dispatcher; }
        }

        /// <summary>
        /// Returns whether HasMoreItems is true, IsLoading is false, and AsyncLoadAction is not null
        /// </summary>
        public Boolean HasMoreItems
        {
            get
            {
                try
                {
                    return AsyncLoadAction != null && CanLoadMoreItems && !IsLoading;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes all the items and resets
        /// the state of the Result Collection...
        /// Hopefully. Title is not changed.
        /// </summary>
        public void Reset()
        {
            IsLoading = false;
            IsFirstLoaded = false;
            ErrorMessage = null;
            IsFaulted = false;
            PagingOffset = 0;
            CanLoadMoreItems = true;
            Clear();
        }


        /// <summary>
        /// Attempts to set the value of a property to the internal Key-Value dictionary,
        /// and fires off a PropertyChangedEvent only if the value has changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected Boolean SetProperty<TProperty>(TProperty value, [CallerMemberName] String propertyName = null)
        {
            if (data.ContainsKey(propertyName) && object.Equals(data[propertyName], value))
                return false;

            data[propertyName] = value;
            OnPropertyChanged(propertyName, AutomaticallyMarshalToDispatcher);
            return true;
        }

        /// <summary>
        /// Attempts to set the value of a referenced property rather than one contained inside
        /// the BindableBase's Key-Value GalleryData dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<TProperty>(ref TProperty storage, TProperty value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName, AutomaticallyMarshalToDispatcher);
            return true;
        }


        /// <summary>
        /// Gets the value of a property. If the property does not exist, returns the defined default value (and sets that value in the model)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">Default value to set and return if property is null. Sets & returns as default(T) if no value is provided</param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected TProperty GetProperty<TProperty>(TProperty defaultValue = default(TProperty), [CallerMemberName] String propertyName = null)
        {
            if (data.ContainsKey(propertyName))
                return (TProperty)data[propertyName];

            data[propertyName] = defaultValue;
            return defaultValue;
        }

        /// <summary>
        /// Allows automatic Marshall of events to the UI thread if Automatic Marshalling
        /// is set as the default behavior
        /// </summary>
        /// <param name="action"></param>
        /// <param name="forceMarshalling"></param>
        public void MarshallHelper(Action action, Boolean? forceFireOverDispatcher = null)
        {
            bool forceFire = forceFireOverDispatcher ?? AutomaticallyMarshalToDispatcher;

            if (Dispatcher.HasThreadAccess || !forceFire)
                action.Invoke();
            else
            {
                var a = Dispatcher.MarshallAsync(() => action.Invoke(), DefaultMarshalingPriority);
            }
        }


        #endregion




        //------------------------------------------------------
        //
        //  Incremental Loading Support
        //
        //------------------------------------------------------

        #region Incremental Loading Support

        /// <summary>
        /// Action called by ISupportIncrementalLoading
        /// </summary>
        public CoreWeakFunc<Task<uint>, Func<uint, Task<uint>>> WeakAsyncLoadAction { get; set; }

        public Func<IncrementalCollection<T>, uint, Task<uint>> AsyncLoadAction { get; set; }

        Task<uint> ExecuteLoadMoreActionAsync(uint count)
        {
            if (WeakAsyncLoadAction != null)
                return WeakAsyncLoadAction.Execute().Invoke(count);

            if (AsyncLoadAction != null)
                return AsyncLoadAction.Invoke(this, count);

            return Task.FromResult((uint)0);
        }

        bool HasLoadAction()
        {
            if (WeakAsyncLoadAction != null)
                return WeakAsyncLoadAction.IsAlive;

            if (AsyncLoadAction != null)
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to load more items into the collection
        /// </summary>
        /// <param name="count">Number of items to load</param>
        /// <returns>Number of items actually loaded</returns>
        public Task<uint> LoadMoreAsync(uint count)
        {
            if (HasLoadAction())
                return ExecuteLoadMoreActionAsync(count);
            else if (WeakAsyncLoadAction == null)
                throw new ArgumentNullException();
            else if (!WeakAsyncLoadAction.IsAlive)
                throw new ArgumentOutOfRangeException("Load Action is no longer alive");

            return null;
        }

        #endregion




    }
}