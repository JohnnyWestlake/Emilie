using System;
using System.Collections.Generic;
using System.Reflection;
using Emilie.Core;

namespace Emilie.UWP.Common
{
    /// <summary>
    /// This interface denotes a ViewModel that should be behave as a single static instance 
    /// when automatically injected at runtime, even if it's UI counterpart is instanced.
    /// </summary>
    public interface IStaticViewModel { }

    /// <summary>
    /// A factory to create or return instances of ViewModels.
    /// Allows for proper support of <see cref="IStaticViewModel"/>
    /// </summary>
    public static class ViewModelHelper
    {
        /// <summary>
        /// A pool containing all of our static viewmodels
        /// </summary>
        static Dictionary<Type, BindableBase> _pool { get; } = new Dictionary<Type, BindableBase>();

        /// <summary>
        /// Returns a new or static instanced version of the requested viewmodel depending on declared interfaces
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : BindableBase, new()
        {
            // If static viewmodel, let's go through our dictionary
            if (typeof(IStaticViewModel).IsAssignableFrom(typeof(T)))
            {
                // If in dictionary, return that instance
                if (_pool.TryGetValue(typeof(T), out BindableBase model))
                    return (T)model;

                // If not in dictionary, make a new instance, add to dictionary and return
                T viewmodel = new T();
                _pool.Add(typeof(T), viewmodel);
                return viewmodel;
            }

            // Non-static viewmodel - just create one!
            return new T();
        }

        public static bool Remove<T>() where T : BindableBase, new()
        {
            // If static viewmodel, try remove
            if (typeof(IStaticViewModel).IsAssignableFrom(typeof(T)))
                return _pool.Remove(typeof(T));

            // Non-static viewmodel - can't remove
            return false;
        }
    }
}
