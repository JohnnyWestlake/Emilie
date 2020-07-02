using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Emilie.Core;

namespace Emilie.UWP
{
    public enum DeviceFamily
    {
        Unknown = 1,
        Desktop = 2,
        Mobile = 4,
        Team = 8,
        IoT = 16,
        Xbox = 32,
        HoloLens = 64
    }

    /// <summary>
    /// Provides Platform services
    /// </summary>
    public class DeviceInformation : BindableBase
    {
        #region Properties 

        private string _xboxModel = null;

        private static string _deviceId;

        private static DeviceInformation _defaultInstance;
        public static DeviceInformation Instance => _defaultInstance ?? (_defaultInstance = new DeviceInformation());

        public bool IsMobile { get; }

        public bool IsPhone { get; }

        public bool IsXbox { get; }

        public bool HasPhoneHardwareButtons { get; }

        public double ScaleFactor => (double)DisplayInformation.GetForCurrentView().ResolutionScale / 100;

        public bool SupportsSDK10586 { get; } = true;
        public bool SupportsSDK14393 { get; } = true;
        public bool SupportsSDK15063 { get; } = true;
        public bool SupportsSDK16299 { get; } = true;
        public bool SupportsSDK17134 { get; } = true;
        public bool SupportsSDK17763 { get; } = true;

        public string SystemFamily { get; }
        public string OSVersion { get; }
        public string SystemArchitecture { get; }
        public string ApplicationName { get; }
        public string ApplicationVersion { get; }
        public string DeviceManufacturer { get; }
        public string DeviceModel { get; }
        public string DeviceSku { get; }
        public string OS { get; }
        public Guid NonUniqueDeviceId { get; }
        public string UniqueDeviceId => GetUniqueDeviceID();
        public string UserAgent { get; set; }

        public ResolutionScale ResolutionScale { get; private set; } = ResolutionScale.Scale100Percent;

        #endregion
        /// <summary>
        /// Returns the app's current memory usage in MB
        /// </summary>
        public double AppMemoryUsage => (double)MemoryManager.AppMemoryUsage / 1024d / 1024d;

        DeviceInformation()
        {
            IsMobile = AnalyticsInfo.VersionInfo.DeviceFamily.ToLower().Contains("windows.mobile");
            IsPhone = ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1);
            HasPhoneHardwareButtons = ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");

            SupportsSDK10586 = ApiInformation.IsTypePresent("Windows.Web.Http.Filters.HttpCookieUsageBehavior");
            SupportsSDK14393 = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);
            SupportsSDK15063 = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4);
            SupportsSDK16299 = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5);
            SupportsSDK17134 = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6);
            SupportsSDK17763 = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7);

#if DEBUG_XBOX
            IsXbox = true;
#else
            IsXbox = AnalyticsInfo.VersionInfo.DeviceFamily.ToLower().Contains("windows.xbox");
#endif

            // following is from here: https://www.suchan.cz/2015/08/uwp-quick-tip-getting-device-os-and-app-info/
            // get the system family name
            AnalyticsVersionInfo ai = AnalyticsInfo.VersionInfo;
            SystemFamily = ai.DeviceFamily;

            // get the system version number
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            OSVersion = $"{v1}.{v2}.{v3}.{v4}";

            // get the package architecture
            Package package = Package.Current;
            SystemArchitecture = package.Id.Architecture.ToString();

            // get the user friendly app name
            ApplicationName = package.DisplayName;

            // get the app version
            PackageVersion pv = package.Id.Version;
            ApplicationVersion = $"{pv.Major}.{pv.Minor}.{pv.Build}.{pv.Revision}";

            // get the device manufacturer and model name
            EasClientDeviceInformation eas = new EasClientDeviceInformation();
            DeviceManufacturer = eas.SystemManufacturer;
            DeviceModel = eas.SystemProductName;
            DeviceSku = eas.SystemSku;
            OS = eas.OperatingSystem;
            NonUniqueDeviceId = eas.Id;
        }

        /// <summary>
        ///     Returns a unique ID for this device
        /// </summary>
        /// <returns>Device ID string</returns>
        public static string GetUniqueDeviceID()
        {
            if (_deviceId == null)
            {
                // If the OS supports Anniversary Update
                if (Instance.SupportsSDK14393)
                {
                    // Get the System ID
                    var systemId = SystemIdentification.GetSystemIdForPublisher();

                    // Make sure this device can generate the IDs from TPM or UEFI
                    if (systemId.Source != SystemIdentificationSource.None)
                    {
                        using (var dataReader = DataReader.FromBuffer(systemId.Id))
                        {
                            var bytes = new byte[systemId.Id.Length];
                            dataReader.ReadBytes(bytes);
                            _deviceId = BitConverter.ToString(bytes).Replace("-", "");
                        }

                    }
                }
            }

            return _deviceId;
        }

        public async Task<string> GetNonRoamingIdAsync()
        {
            IReadOnlyList<User> users = await User.FindAllAsync();
            return users?.FirstOrDefault()?.NonRoamableId;
        }

        public string GetWindowsSDKString()
        {
            if (SupportsSDK17763)
                return "17763";
            else if (SupportsSDK17134)
                return "17134";
            else if (SupportsSDK16299)
                return "16299";
            else if (SupportsSDK15063)
                return "15063";
            else if (SupportsSDK14393)
                return "14393";
            else if (SupportsSDK10586)
                return "10586";
            else
                return "10240";
        }


        private Size? CachedSize = null;
        public Size GetActualWindowResolution()
        {
            if (Window.Current != null && !Window.Current.Bounds.IsEmpty)
            {
                CachedSize = new Size(ConvertToActualResolution(Window.Current.Bounds.Width), ConvertToActualResolution(Window.Current.Bounds.Height));
                return CachedSize.Value;
            }

            // Return a cached size if I'm coming from a non-UI thread
            if (CachedSize.HasValue)
                return CachedSize.Value;
            else
                return Size.Empty;
        }

        public ProcessorArchitecture ProcessorArchitecture => Package.Current.Id.Architecture;

        public void UpdateResolutionScale()
        {
            ResolutionScale = DisplayInformation.GetForCurrentView().ResolutionScale;
        }

        /// <summary>
        /// Converts Device-Independent resolution to the actual screen resolution active on the device
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static double ConvertToActualResolution(double dimension)
        {
            ResolutionScale scale = DisplayInformation.GetForCurrentView().ResolutionScale;

            if (scale == ResolutionScale.Invalid)
                return dimension;

            return Math.Round((((Double)scale) / 100) * dimension);
        }

        public static double ConvertToScaledResolution(double dimension)
        {
            if (Instance.ResolutionScale == ResolutionScale.Invalid)
                return dimension;

            return Math.Round((dimension / 4) * ((Double)Instance.ResolutionScale) / 100);
        }

        public DeviceFamily CurrentDeviceFamily
        {
            get
            {
                var family = AnalyticsInfo.VersionInfo.DeviceFamily;
                switch (family)
                {
                    case "Windows.Desktop": return DeviceFamily.Desktop;
                    case "Windows.Mobile": return DeviceFamily.Mobile;
                    case "Windows.Team": return DeviceFamily.Team;
                    case "Windows.IoT": return DeviceFamily.IoT;
                    case "Windows.Xbox": return DeviceFamily.Xbox;
                    case "Windows.HoloLens": return DeviceFamily.HoloLens;
                    default: return DeviceFamily.Unknown;
                }
            }
        }


        /// <summary>
        /// Gets a friendly device name for an Xbox device. If the current device SKU is not 
        /// recognised it is returned as normal.
        /// </summary>
        /// <returns></returns>
        public string GetXboxModel()
        {
            if (_xboxModel == null)
            {
                if (DeviceSku.Equals("XBOX_ONE_DU"))
                    _xboxModel = "Xbox One";
                else if (DeviceSku.Equals("XBOX_ONE_ED"))
                    _xboxModel = "Xbox One S";
                else if (DeviceSku.Equals("XBOX_ONE_SC"))
                    _xboxModel = "Xbox One X";
                else
                    _xboxModel = DeviceSku;
            }

            return _xboxModel;
        }
    }

}
