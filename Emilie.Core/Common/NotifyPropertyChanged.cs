using Emilie.Core.Common;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Emilie.Core.Common
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Attempts to set the value of a referenced property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T storage, T value, bool marshall = true, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;

            if (marshall)
                DispatcherHelper.MarshallAsync(() => this.OnPropertyChanged(propertyName));
            else
                this.OnPropertyChanged(propertyName);


            return true;
        }
    }
}
