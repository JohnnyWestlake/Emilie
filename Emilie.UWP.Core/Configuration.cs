using Emilie.Core;
using Emilie.Core.Common;
using Emilie.Core.Network;
using Emilie.Core.Storage;
using Emilie.Core.Utilities;
using Emilie.UWP.Common;
using Emilie.UWP.Utilities;
using SQLite;

namespace Emilie.UWP
{
    public static class Configuration
    {
        public static void Configure()
        {
            CoreIoC.Register<IDispatcherTimer, UWPDispatcherTimer>();

            CoreIoC.RegisterSingleton<IDispatcher, UWPDispatcher>();
            CoreIoC.RegisterSingleton<IStorageHelper, UWPStorageHelper>();
            CoreIoC.RegisterSingleton<IAppMemoryDiagnosticProvider, UWPMemoryDiagnosticProvider>();


            //CoreIoC.RegisterSingleton<ICoreHttpClientFactory, UWPHttpClientFactory>();
        }

        public static void ConfigureSqlite()
        {
            CoreIoC.RegisterSingleton<ISqliteBatteryCharger, SQLiteCharger>();
        }
    }
}
