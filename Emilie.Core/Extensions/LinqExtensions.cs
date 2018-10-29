using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Emilie.Core.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T item)
        {
            return source.Where(s => !EqualityComparer<T>.Default.Equals(s, item));
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Runs an action on all items of an IEnumerable. Causes the Input IEnumerable to evaluate itself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">IEnumerable to be evaluated and processed</param>
        /// <param name="action"></param>
        public static IEnumerable<T> DoImmediate<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // perf optimization. try to not use enumerator if possible
            if (source is IList<T> list)
            {
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    action(list[i]);
                }
            }
            else
            {
                foreach (var value in source)
                {
                    action(value);
                }
            }

            return source;
        }

        /// <summary>
        /// Runs an Asynchronous task on all items of an IEnumerable. Will cause evaluation of the input IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">IEnumerable to be evaluated and processed</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> DoImmediateAsync<T>(this IEnumerable<T> source, Func<T, Task> func, bool configureAwaiter = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            // perf optimization. try to not use enumerator if possible
            if (source is IList<T> list)
            {
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    await func(list[i]).ConfigureAwait(configureAwaiter);
                }
            }
            else
            {
                foreach (var value in source)
                {
                    await func(value).ConfigureAwait(configureAwaiter);
                }
            }

            return source;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return true;

            if (list is ICollection<T>)
                return ((ICollection<T>)list).Count == 0;
            else
                return !list.Any();
        }

        /// <summary>
        /// This code isn't used for anything. I just find it useful ~
        /// sorter.AddSort("NAME", m => m.Name);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="func"></param>
        public static void AddSort<T>(string name, Expression<Func<T, object>> func)
        {
            string fieldName = (func.Body as MemberExpression).Member.Name;
        }
    }
}
