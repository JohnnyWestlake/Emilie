using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Emilie.UWP.Common
{
    /// <summary>
    /// A base page that is associated with a viewmodel, and automatically deflates / inflates its viewmodel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasePage<T> : BasePage, INotifyPropertyChanged where T : BasePageModel, new()
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<V>(ref V storage, V value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            this.OnPropertyChanged(propertyName);

            return true;
        }

        T _viewModel = null;
        public T ViewModel
        {
            get { return _viewModel; }
            set { Set(ref _viewModel, value); }
        }


        protected override void LoadState(object parameter, Dictionary<string, object> pageState)
        {
            if (pageState != null && pageState.ContainsKey(NavigationService.VIEWMODEL))
            {
                // Only NON-STATIC viewmodels go through this return path.
                // IStaticViewModel's go through the return path below
                ViewModel = pageState[NavigationService.VIEWMODEL] as T;
                ViewModel.OnNavigatedTo(parameter, NavigationMode.Back);
            }
            else
            {
                T viewModel = ViewModelHelper.Get<T>();
                viewModel.OnNavigatedTo(parameter, pageState == null ? NavigationMode.New : NavigationMode.Back);
                ViewModel = viewModel;
            }

            ViewModel.IsCurrentPage = true;
        }

        protected override void SaveState(NavigationEventArgs e, Dictionary<string, object> pageState)
        {
            ViewModel.OnNavigatingFrom();

            // Don't bother recording the static viewmodel. Tombstoning won't work either way with static VM's 
            // until we explicitly write code to do so.
            if (!(ViewModel is IStaticViewModel))
            {
                pageState.Add(NavigationService.VIEWMODEL, ViewModel);
            }

            ViewModel.IsCurrentPage = false;
        }

    }
}
