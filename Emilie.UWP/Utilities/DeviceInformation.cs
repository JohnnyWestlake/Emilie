using Emilie.Core;
using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Emilie.UWP
{
    public enum DeviceFamily { Unknown, Desktop, Mobile, Team, IoT, Xbox }

    /// <summary>
    /// Provides Platform services
    /// </summary>
    public class DeviceInformation : BindableInstanceable<DeviceInformation>
    {
        #region Properties 

        /// <summary>
        /// Indicates support for:
        /// - Jumplists
        /// - Windows.UI.Composition Updates
        /// - CookieUsageBehavior
        /// - Windows.Devices.Perception
        /// + more - see here for details: 
        /// https://blogs.windows.com/buildingapps/2015/12/03/windows-10-build-10586-sdk-breakdown/
        /// </summary>
        public bool SupportsSDK10586 { get; }
        public bool SupportsSDK14393 { get; }



        public bool IsPhone { get; }

        public bool HasPhoneHardwareButtons { get; }

        public Size ScreenResolution { get; private set; }

        public bool IsContinuumMode
        {
            get { return GetV(false); }
            private set { Set(value, nameof(IsContinuumMode), nameof(IsContinuumOrTabletMode)); }
        }

        /// <summary>
        /// Warning, be careful about taking in to account Continuum here too - 
        /// ideally they should use the same layout!
        /// </summary>
        public bool IsTabletMode
        {
            get { return GetV(false); }
            private set { Set(value, nameof(IsTabletMode), nameof(IsContinuumOrTabletMode)); }
        }

        public bool IsContinuumOrTabletMode => IsContinuumMode || IsTabletMode;

        public string OSVersion => Get(GetOSVersion);

        public string DeviceArchitecture => Get(GetDeviceArchitecture);

        /// <summary>
        /// Returns the app's current memory usage in MB
        /// </summary>
        public double AppMemoryUsage => (double)Windows.System.MemoryManager.AppMemoryUsage / 1024d / 1024d;


        #endregion


        public DeviceInformation()
        {
            IsPhone = ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1);
            HasPhoneHardwareButtons = ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");

            // This Enum was added in the 10586.0 SDK, so we use it as a general feature set detection mechanism
            SupportsSDK10586 = ApiInformation.IsTypePresent("Windows.Web.Http.Filters.HttpCookieUsageBehavior");

            SupportsSDK14393 = ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService");

            UpdateScreenResolution();
            UpdateModes();

            Window.Current.SizeChanged += Current_SizeChanged;
        }



        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            UpdateModes();
        }

        void UpdateModes()
        {
            if (CurrentDeviceFamily == DeviceFamily.Desktop)
                UpdateIsTabletMode();
            else if (CurrentDeviceFamily == DeviceFamily.Mobile)
                UpdateContinuumMode();
        }

        /// <summary>
        /// Warning: This does not yet account for screen resolution changes that occur during an app session
        /// </summary>
        //[QualityBand(QualityBand.Preview, "Needs to account for screen resolution change during runtime")]
        public void UpdateScreenResolution()
        {
            try
            {
                if (Window.Current != null && Window.Current.Content != null)
                {
                    var bounds = Window.Current.Bounds;
                    double w, h;
                    var pointerDevice = PointerDevice.GetPointerDevices().FirstOrDefault();
                    if (pointerDevice != null)
                    {
                        w = pointerDevice.ScreenRect.Width;
                        h = pointerDevice.ScreenRect.Height;
                    }
                    else
                    {
                        w = bounds.Width;
                        h = bounds.Height;
                    }

                    var displayInfo = DisplayInformation.GetForCurrentView();
                    var scale = (double)(int)displayInfo.ResolutionScale / 100d;
                    w = Math.Round(w * scale);
                    h = Math.Round(h * scale);

                    if ((displayInfo.NativeOrientation & DisplayOrientations.Landscape) == DisplayOrientations.Landscape)
                    {
                        ScreenResolution = new Size((int)w, (int)h);
                    }
                    else // portrait
                    {
                        ScreenResolution = new Size((int)h, (int)w);
                    }
                }
            }
            catch { /* ignore, Bounds may not be ready yet. Investigate whether this ever happens */ }
        }

        public DeviceFamily CurrentDeviceFamily
        {
            get
            {
                switch (AnalyticsInfo.VersionInfo.DeviceFamily)
                {
                    case "Windows.Desktop": return DeviceFamily.Desktop;
                    case "Windows.Mobile": return DeviceFamily.Mobile;
                    case "Windows.Team": return DeviceFamily.Team;
                    case "Windows.IoT": return DeviceFamily.IoT;
                    case "Windows.Xbox": return DeviceFamily.Xbox;
                    default: return DeviceFamily.Unknown;
                }
            }
        }

        #region Mode & Metadata Detection

        // The UserInteractionMode API can be used to detect Continuum and Tablet modes 
        //
        // With Continuum, â€œtouchâ€ will always be returned when your app is on the mobile device, 
        // and â€œmouseâ€ will always be returned when your app is on the connected display.  
        // UserInteractionMode will only change when your app is moved between screens.  
        // UserInteractionMode does not send events â€“ youâ€™ll need to query this API upon receiving 
        // a SizeChanged event.  Note that UserInteractionMode is also used to inform developers 
        // when a desktop device is in tablet mode â€“ Continuum is not the only use case.

        void UpdateIsTabletMode()
        {
            IsTabletMode =
                CurrentDeviceFamily == DeviceFamily.Desktop &&
                UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch;
        }

        void UpdateContinuumMode()
        {
            IsContinuumMode =
                CurrentDeviceFamily == DeviceFamily.Mobile &&
                UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse;
        }

        string GetOSVersion()
        {
            ulong v = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            return $"{v1}.{v2}.{v3}.{v4}";
        }

        string GetDeviceArchitecture()
        {
            return Package.Current.Id.Architecture.ToString();
        }

        #endregion
    }
}