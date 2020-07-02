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

using Emilie.Core;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Emilie.UWP
{
    /// <summary>
    /// Implementation of <see cref="IncrementalCollection{T}"/> that has automatic support for UWP's
    /// <see cref="ISupportIncrementalLoading"/> interface.
    /// </summary>
    public class ResultCollection<T> : IncrementalCollection<T>, ISupportIncrementalLoading
    {
        public ResultCollection() : base() { }

        public ResultCollection(List<T> list) : base(list) { }

        public ResultCollection(IEnumerable<T> collection) : base(collection) { }

        //------------------------------------------------------
        //
        //  Incremental Loading Support
        //
        //------------------------------------------------------

        #region Incremental Loading Support

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadMoreAsync(count).ContinueWith(r => new LoadMoreItemsResult() { Count = r.Result }).AsAsyncOperation();
        }
    
        #endregion
    }
}