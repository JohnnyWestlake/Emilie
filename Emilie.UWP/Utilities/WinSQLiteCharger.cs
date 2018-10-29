using SQLite;
using SQLitePCL;

namespace Emilie.UWP.Utilities
{
    public class WinSQLiteCharger : ISqliteBatteryCharger
    {
        public ISQLite3Provider GetProvider()
        {
            return new SQLite3Provider_winsqlite3();
        }
    }
}
