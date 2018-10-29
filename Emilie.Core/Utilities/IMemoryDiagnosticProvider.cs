using System;
using System.Collections.Generic;
using System.Text;

namespace Emilie.Core.Utilities
{
    public struct AppMemoryDiagnosticReport
    {
        public ulong AppMemoryUsageLimit { get; }
        public ulong AppPrivateWorkingSetUsage { get; }

        public AppMemoryDiagnosticReport(ulong appPrivateWorkingSet, ulong appMemoryUsageLimit)
        {
            AppPrivateWorkingSetUsage = appPrivateWorkingSet;
            AppMemoryUsageLimit = appMemoryUsageLimit;
        }
    }

    public interface IAppMemoryDiagnosticProvider
    {
        AppMemoryDiagnosticReport GetMemoryDiagnosticReport();
    }
}
