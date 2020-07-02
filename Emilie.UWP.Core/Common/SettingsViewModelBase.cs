using Emilie.Core;
using System;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace Emilie.UWP
{
    public class SettingsViewModelBase : BindableBase
    {
        #region Getter / Setter wrappers 

        public T GetSetting<T>(ApplicationDataContainer container, T defaultValue = default(T), [CallerMemberName]String key = null)
        {
            if (!container.Values.ContainsKey(key))
                container.Values.Add(key, defaultValue);

            return (T)container.Values[key];
        }

        public Boolean SetSetting<T>(ApplicationDataContainer container, T value, [CallerMemberName]String key = null)
        {
            if (!container.Values.ContainsKey(key))
            {
                container.Values.Add(key, value);
                OnPropertyChanged(key);
                return true;
            }
            else
            {
                if (((T)container.Values[key]).Equals(value))
                    return false;
                else
                {
                    container.Values[key] = value;
                    OnPropertyChanged(key);
                    return true;
                }
            }
        }

        private ApplicationDataContainer Roaming
        {
            get { return ApplicationData.Current.RoamingSettings; }
        }

        private ApplicationDataContainer Local
        {
            get { return ApplicationData.Current.LocalSettings; }
        }

        #endregion
    }
}
