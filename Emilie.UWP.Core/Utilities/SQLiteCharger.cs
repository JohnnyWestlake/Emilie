using SQLite;
using SQLitePCL;
using System;

namespace Emilie.UWP.Utilities
{
    public class SQLiteCharger : ISqliteBatteryCharger
    {
        public ISQLite3Provider GetProvider()
        {
#if UWP
            return new SQLite3Provider_winsqlite3();
#endif

            throw new NotImplementedException();
        }
    }
}
