﻿using Emilie.Core.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Emilie.Core
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// NOT Thread-Safe. Can be made threadsafe by uncommenting first property, 
    /// at cost of performance
    /// </summary>
    //[QualityBand(QualityBand.Mature)]
    public class BindableBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Private data store that contains all of the properties access through GetProperty 
        /// method.
        /// </summary>
        //readonly ConcurrentDictionary<String, Object> data = new ConcurrentDictionary<String, Object>();
        readonly Dictionary<String, Object> data = new Dictionary<String, Object>();

        /// <summary>
        /// When overridden, allows you to specify whether all standard SetProperty calls
        /// automatically attempt to fire their property change notifications on the dispatcher.
        /// Defaults to false. (Be aware of any possible threading issues this may cause - UI
        /// thread will be updated notable after the data layer)
        /// </summary>
        public virtual Boolean AutomaticallyMarshalToDispatcher => true;

        /// <summary>
        /// When overridden, allows you to specify the dispatcher priority used when 
        /// Automatically marshaling to the Dispatcher thread or when using 
        /// DispatcherSetProperty method
        /// </summary>
        public virtual DispatcherPriority DefaultMarshalingPriority => DispatcherPriority.Normal;

        /// <summary>
        /// Attempts to set the value of a referenced property rather than one contained inside
        /// the BindableBase's Key-Value data dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            this.OnPropertyChanged(propertyName, AutomaticallyMarshalToDispatcher);
            return true;
        }

        /// <summary>
        /// Attempts to set the value of a property to the internal Key-Value dictionary,
        /// and fires off a PropertyChangedEvent only if the value has changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected Boolean Set<T>(T value, [CallerMemberName] String propertyName = null)
        {
            if (data == null)
                return false;

            if (data.TryGetValue(propertyName, out object t) && object.Equals(t, value))
                return false;

            data[propertyName] = value;
            this.OnPropertyChanged(propertyName, AutomaticallyMarshalToDispatcher);
            return true;
        }

        /// <summary>
        /// Updates a property value. If the value has changed, property changed notifications are called
        /// for the specified property, and all dependent properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">New desired value of the property</param>
        /// <param name="propertyName">Name of the property being set. This cannot be inferred automatically because of the params overload.</param>
        /// <param name="dependentProperties">Names of additional properties to call PropertyChanged events for</param>
        /// <returns></returns>
        protected Boolean Set<T>(T value, String propertyName, params string[] dependentProperties)
        {
            if (Set(value, propertyName))
            {
                this.OnPropertiesChanged(dependentProperties);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Optimised for value types. Gets the value of a property. If the property does not exist, returns the defined default value (and sets that value in the model)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">Default value to set and return if property is null. Sets & returns as default(T) if no value is provided</param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetV<T>(T defaultValue = default, [CallerMemberName] String propertyName = null)
        {
            if (data.TryGetValue(propertyName, out object t))
                return (T)t;

            data[propertyName] = defaultValue;
            return defaultValue;
        }

        /// <summary>
        /// Optimised for object types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T Get<T>(Func<T> defaultValue = null, [CallerMemberName] String propertyName = null)
        {
            if (data.TryGetValue(propertyName, out object t))
                return (T)t;

            T value = (defaultValue == null) ? default : defaultValue.Invoke();
            data[propertyName] = value;
            return value;
        }

        /// <summary>
        /// A GET overload that returns a 'new(T)' instance of the given type if there is no value for the requested property, rather than 'default(T)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetN<T>([CallerMemberName] String propertyName = null) where T : new()
        {
            if (data.TryGetValue(propertyName, out object t))
                return (T)t;

            T value = new T();
            data[propertyName] = value;
            return value;
        }


        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null, bool forceFireOverDispatcher = false)
        {
            try
            {
                var eventHandler = this.PropertyChanged;
                if (eventHandler != null)
                {
                    bool forceFire = forceFireOverDispatcher || AutomaticallyMarshalToDispatcher;

                    if (!forceFire)
                        eventHandler(this, new PropertyChangedEventArgs(propertyName));
                    else
                        DispatcherHelper.MarshallAsync(() => eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        protected void OnPropertiesChanged(params String[] args)
        {
            // Marshall all of them together
            DispatcherHelper.MarshallAsync(() =>
            {
                foreach (var arg in args)
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(arg));
            });
        }

        /// <summary>
        /// Broadcasts an OnPropertyChanged notification signifying all properties should be re-evaluated
        /// </summary>
        /// <param name="forceFireOverDispatcher"></param>
        protected void OnAllPropertiesChanged(bool forceFireOverDispatcher = false)
        {
            try
            {
                var eventHandler = this.PropertyChanged;
                if (eventHandler != null)
                {
                    bool forceFire = forceFireOverDispatcher || AutomaticallyMarshalToDispatcher;

                    if (!forceFire)
                        eventHandler(this, new PropertyChangedEventArgs(String.Empty));
                    else
                        DispatcherHelper.MarshallAsync(() => eventHandler?.Invoke(this, new PropertyChangedEventArgs(String.Empty)));
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }



        /// <summary>
        /// Frees up memory associated with the internal dictionary that may otherwise cause memory leakage
        /// </summary>
        public virtual void Dispose()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            data.Clear();
        }


        public void DetachPropertyChangedListeners()
        {
            if (PropertyChanged != null)
            {
                foreach (Delegate d in PropertyChanged?.GetInvocationList())
                {
                    PropertyChanged -= (PropertyChangedEventHandler)d;
                }
            }
        }

        /// <summary>
        /// Removes the value of a property from the internal dictionary store.
        /// The default value of this property is returned if the property getter
        /// is accessed again
        /// </summary>
        /// <param name="name"></param>
        protected void ResetValue(String name)
        {
            if (data.ContainsKey(name))
            {
                if (data.Remove(name))
                    this.OnPropertyChanged(name);
            }
        }

        /// <summary>
        /// Returns a list of property names mapping to properties who have had their local values cleared.
        /// </summary>
        /// <param name="firePropertyChanged"></param>
        /// <returns></returns>
        protected virtual void ResetAllValues(bool firePropertyChanged = true)
        {
            data.Clear();

            if (firePropertyChanged)
                OnAllPropertiesChanged();
        }
    }
}