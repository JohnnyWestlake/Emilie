using System;
using System.Collections.Generic;

namespace Emilie.Core
{
    /// <summary>
    /// A very basic IoC container that stays performant by ignoring type checks
    /// and not implementing dependency injection. As a result, ensure types are 
    /// valid and assignable from each other when registering.
    /// </summary>
    public static class CoreIoC
    {
        private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, Type> _lazySingletons = new Dictionary<Type, Type>();

        private static readonly Dictionary<Type, Type> _typeMap = new Dictionary<Type, Type>();

        /// <summary>
        /// Lazily an object instance for singleton usage in the IoC provider
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TC"></typeparam>
        /// <param name="instance"></param>
        public static void RegisterSingleton<TI, TC>(TC instance) where TI : class
        {
            if (instance is TI _)
                _singletons[typeof(TI)] = instance;
        }

        /// <summary>
        /// Lazily registers a type for singleton usage in the IoC provider
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TC"></typeparam>
        public static void RegisterSingleton<TI, TC>() where TI : class where TC : new()
        {
            _lazySingletons[typeof(TI)] = typeof(TC);
        }

        public static void Register<TI, TC>() where TI : class where TC : new()
        {
            _typeMap.Add(typeof(TI), typeof(TC));
        }

        /// <summary>
        /// Returns an implementation of an interface registered using <see cref="RegisterSingleton{TI, TC}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : class
        {
            if (_singletons.TryGetValue(typeof(T), out object val))
                return (T)val;

            if (_lazySingletons.TryGetValue(typeof(T), out Type lazy))
            {
                T inst;
                lock (_lazySingletons)
                {
                    _lazySingletons.Remove(typeof(T));
                    inst = (T)Activator.CreateInstance(lazy);
                    _singletons.Add(typeof(T), inst);
                }

                return inst;
            }

            return default;
        }

        /// <summary>
        /// Returns a new instance of a class registered using <see cref="Register{TI, TC}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T New<T>() where T : class
        {
            // TODO : Perhaps we should have different methods for getting singletons
            //        and getting normal types to avoid checking multiple dictionaries?
            //        Depends on end code usage as to whether this would be beneficial
            if (_typeMap.TryGetValue(typeof(T), out Type nval))
                return (T)Activator.CreateInstance(nval);

            return default;
        }

    }
}
