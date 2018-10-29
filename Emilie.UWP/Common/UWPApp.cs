using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Emilie.Core;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Emilie.UWP
{
    [QualityBand(QualityBand.Experimental)]
    public abstract class UWPApp : Application
    {
        public static new UWPApp Current { get; private set; }

        public SplashScreen SplashScreen { get; protected set; }

        public UWPApp()
        {
            Current = this;

            // Hook suspending event
            this.Suspending += Application_Suspending;
            this.Resuming += Application_Resuming;

            // Hook error handling
            this.UnhandledException += Application_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Hook UWP interfaces for Emilie
            Emilie.UWP.Configuration.Configure();
            Emilie.UWP.Configuration.ConfigureSqlite();

            // Application specific pre-initialization configuration
            Configure();
        }

        protected virtual void Configure()
        {

        }

        //------------------------------------------------------
        //
        //  Launch
        //
        //------------------------------------------------------

        #region Launch Handling

        protected override void OnActivated(IActivatedEventArgs args)
        {
            OnLaunch(args);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PrelaunchActivated)
            {
                // We're clearly a prelaunch here, so handle it.
                Logger.Log("Prelaunching");
                OnPrelaunch(args);
            }
            else
            {
                OnLaunch(args);
            }
        }

        private void OnLaunch(IActivatedEventArgs args)
        {
            // Store the splash screen for anyone who wishes to use it
            SplashScreen = args.SplashScreen;

            // Check to see if we can handle this
            if (Window.Current.Content == null)
            {
#if DEBUG
                ConfigureDebug(this.DebugSettings);
#endif
                OnNewLaunch(args);
            }
            else
            {
                OnActiveLaunched(args);
            }
        }

        protected virtual void ConfigureDebug(DebugSettings settings)
        {
            if (Debugger.IsAttached)
            {
                settings.EnableFrameRateCounter = true;
            }
        }

        /// <summary>
        /// Called when the application is launched and has no current Window content
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnNewLaunch(IActivatedEventArgs args)
        {
        }

        /// <summary>
        /// Called when the application is prelaunched. The window is not activated
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPrelaunch(LaunchActivatedEventArgs args)
        {
        }

        protected virtual void OnActiveLaunched(IActivatedEventArgs args)
        {

        }

        #endregion




        //------------------------------------------------------
        //
        //  Suspension
        //
        //------------------------------------------------------

        #region Suspension Handling

        private void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            Logger.Log($"Application suspending\n\n");
            //Logger.Flush();
            OnSuspending(deferral, e.SuspendingOperation.Deadline);
        }

        protected virtual void OnSuspending(SuspendingDeferral deferral, DateTimeOffset deadline)
        {
            deferral.Complete();
        }

        #endregion




        //------------------------------------------------------
        //
        //  Resuming
        //
        //------------------------------------------------------

        #region Resuming Handling

        private void Application_Resuming(object sender, object e)
        {
            OnResuming();
        }

        protected virtual void OnResuming()
        {

        }

        #endregion




        //------------------------------------------------------
        //
        //  Exception Handling
        //
        //------------------------------------------------------

        #region Exception Handling

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            OnUnobservedTaskException(sender, e);
        }

        /// <summary>
        /// Default behaviour logs the exception and marks it as handled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Logger.Log(e.Exception);
        }

        private void Application_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            OnUnhandledException(sender, e);
            Logger.Flush();
        }

        /// <summary>
        /// Default behaviour logs the exception and marks it as handled, and attempts to flush the logs.
        /// This may not complete successfully if the application process is terminated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Logger.Log(e.Exception);
            //await Logger.FlushAsync();
        }

        #endregion




        //------------------------------------------------------
        //
        //  Resourcing
        //
        //------------------------------------------------------

        public static T GetResource<T>(string key)
        {
            return (T)Current.Resources[key];
        }
    }

}