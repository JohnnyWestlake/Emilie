using Emilie.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Emilie.UWP.Common
{
    /// <summary>
    /// When used in conjunction with <see cref="BasePage{T}"/>, provides automatic navigation and state handling
    /// </summary>
    public abstract class BasePageModel : BindableBase
    {
        public virtual void OnNavigatedTo(object parametre, NavigationMode mode) { }
        public virtual void OnNavigatingFrom()
        {
        }

        public bool IsCurrentPage { get; internal set; }

       
    }
}
