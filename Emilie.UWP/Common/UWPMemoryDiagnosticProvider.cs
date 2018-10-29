using Emilie.Core.Utilities;
using Windows.System;

namespace Emilie.UWP.Common
{
    public class UWPMemoryDiagnosticProvider : IAppMemoryDiagnosticProvider
    {
        public AppMemoryDiagnosticReport GetMemoryDiagnosticReport()
        {
            ProcessMemoryReport report = MemoryManager.GetProcessMemoryReport();
            return new AppMemoryDiagnosticReport(report.PrivateWorkingSetUsage, MemoryManager.AppMemoryUsageLimit);
        }
    }
}
