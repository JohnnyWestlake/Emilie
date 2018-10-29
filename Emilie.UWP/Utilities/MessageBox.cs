using Emilie.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Emilie.UWP.Utilities
{
    public class MessageBox
    {
        public const string OK_LABEL = "Ok";
        public const string CANCEL_LABEL = "Cancel";
        public const string NOTHANKS_LABEL = "No thanks";
        public const string NOT_NOW_LABEL = "Not Now";
        public const string NEVER = "Never";
        

        public static void Show(String message, String title)
        {
            try
            {
                var md = new MessageDialog(message, title);
#pragma warning disable CS4014
                md.ShowAsync();
#pragma warning restore CS4014
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static Task<IUICommand> ShowAsync(String message, String title, String okLabel = OK_LABEL, String cancelLabel = CANCEL_LABEL)
        {
            var md = new MessageDialog(message, title);

            UICommand ok = new UICommand { Id = "OK", Label = okLabel };
            UICommand cancel = new UICommand { Id = "Cancel", Label = cancelLabel };

#if WINDOWS_PHONE_APP
            md.Options = MessageDialogOptions.AcceptUserInputAfterDelay;
#endif

            md.Commands.Add(ok);
            md.Commands.Add(cancel);

            md.CancelCommandIndex = 1;
            md.DefaultCommandIndex = 1;

            return md.ShowAsync().AsTask();
        }



        /// <summary>
        /// Shows an all encompassing internet connectivity error message.
        /// </summary>
        /// <param name="actionToDo">to ... [ insert action here: e.g. "add this recipe to your shopping list" ]</param>
        public static void ShowNoInternet(string actionToDo)
        {
            MessageBox.Show(String.Format("Sorry, you need an internet connection to {0}. Please check your internet connection and try again later.", actionToDo), "No Internet Connection" );
        }

    }
}
