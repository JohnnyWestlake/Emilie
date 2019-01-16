using Emilie.UWP.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;

namespace Emilie.UWP.Extensions
{
    public static class ListViewExtensions
    {
        #region Scroll Persistence Handling

        private static Dictionary<Type, ListViewItemToKeyHandler> _itemToKeyHandlers { get; } = new Dictionary<Type, ListViewItemToKeyHandler>();
        private static ListViewItemToKeyHandler _ilviHandler { get; } = new ListViewItemToKeyHandler(i => ((IListViewItem)i).GetListViewKey());


        public static void RegisterItemToKeyConverter(Type t, Func<object, string> converter)
        {
            _itemToKeyHandlers[t] = new ListViewItemToKeyHandler(converter);
        }

        public static string GetScrollPersistenceKey(ListViewBase listView)
        {
            // 1. If no items in the list, we can't do anything so return no key
            if (listView.Items.Count == 0 || listView.ItemsPanelRoot == null)
                return null;

            // 2. If the type explicitly implements IListViewItem, go down special case to return
            //    key quickly
            if (listView.Items[0] is IListViewItem lvi)
                return ListViewPersistenceHelper.GetRelativeScrollPosition(listView, _ilviHandler);

            // 3. Check our registered converters to see if we have one that can convert the type
            //    of the items to a valid scroll key.
            Type type = listView.Items[0].GetType();
            if (_itemToKeyHandlers.TryGetValue(type, out ListViewItemToKeyHandler handler))
                return ListViewPersistenceHelper.GetRelativeScrollPosition(listView, handler);
            else
                throw new InvalidOperationException($"There is no ListViewItem converter registered for type '{type}'");
        }

        public static Task<object> GetRestoreTargetAsync(ListViewBase listView, string key)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            void VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
            {
                if (TryGetItem() is object o)
                {
                    listView.Items.VectorChanged -= VectorChanged;
                    tcs.SetResult(o);
                }
            }

            object TryGetItem()
            {
                Type t = listView.Items[0].GetType();
                ListViewItemToKeyHandler handler = _itemToKeyHandlers[t];
                return listView.Items.FirstOrDefault(i => handler(i) == key);
            }

            if (listView.Items == null || listView.Items.Count == 0 || !(TryGetItem() is object obj))
            {
                listView.Items.VectorChanged -= VectorChanged;
                listView.Items.VectorChanged += VectorChanged;
            }
            else
            {
                tcs.SetResult(obj);
            }

            return tcs.Task;
        }

        #endregion

    }
}
