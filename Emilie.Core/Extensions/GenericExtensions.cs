using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Emilie.Core.Extensions
{
    public static class GenericExtensions
    {

        //------------------------------------------------------
        //
        //  Miscellaneous
        //
        //------------------------------------------------------

        #region Misc

        public static void SafeThrowIfCancellationRequested(this CancellationToken token)
        {
            if (token != null && token != CancellationToken.None)
                token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Returns whether or not the input is null or equal to the default
        /// value for the type if non-nullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Boolean IsNullOrDefault<T>(this T obj)
        {
            if (obj == null)
                return true;

            try
            {
                return (EqualityComparer<T>.Default.Equals(obj, default(T)));
            }
            catch (NullReferenceException)
            {
                // Is Null
                return true;
            }
            catch (ArgumentNullException)
            {
                return true;
            }
        }

        /// <summary>
        /// Validates whether a <see cref="PropertyChangedEventArgs"/> should signify a change for the input
        /// property. Use rather than direct property name comparisons as this also catches blanket
        /// notifications (null PropertyName) which signifies all properties have changed.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool PropertyChanged(this PropertyChangedEventArgs args, string property)
        {
            // IsNullOrEmpty is used by x:Bind code-gen rather than IsNullOrWhiteSpace, so we do the same.
            return String.IsNullOrEmpty(args.PropertyName) || args.PropertyName.Equals(property);
        }

        #endregion




        //------------------------------------------------------
        //
        //  Enumerable
        //
        //------------------------------------------------------

        #region Enumerable

        public static T InsertItem<T, Y>(this T list, int index, Y item) where T : IList<Y>
        {
            list.Insert(index, item);
            return list;
        }

        public static T AddItem<T, Y>(this T list, Y item) where T : ICollection<Y>
        {
            list.Add(item);
            return list;
        }

        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source)
                throw new ArgumentNullException(nameof(source));
            return list.Contains(source);
        }

        #endregion




        //------------------------------------------------------
        //
        //  Weak Reference
        //
        //------------------------------------------------------

        #region Weak Reference

        /// <summary>
        /// Tries to get the item inside a generic WeakReference.
        /// Returns default(T) if fails.
        /// (If dealing with structs or non-nullable classes, use
        /// standard TryGetTarget)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wref"></param>
        /// <returns></returns>
        [Obsolete("Use GetTarget<T> instead", false)]
        public static T TryGetItem<T>(this WeakReference<T> wref) where T : class
        {
            wref.TryGetTarget(out T container);
            return container;
        }

        public static T GetTarget<T>(this WeakReference<T> reference) where T : class
        {
            reference.TryGetTarget(out T target);
            return target;
        }

        public static bool IsNull<T>(this WeakReference<T> reference) where T : class
        {
            return !reference.TryGetTarget(out _);
        }

        #endregion




        //------------------------------------------------------
        //
        //  Enum
        //
        //------------------------------------------------------

        #region Enum 

        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }


        #endregion
    }
}
