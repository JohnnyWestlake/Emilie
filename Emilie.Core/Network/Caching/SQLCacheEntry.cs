using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emilie.Core.Network
{
    public class SQLCacheStringEntry
    {
        [PrimaryKey]
        [Indexed]
        public string Key { get; set; }
        public string Data { get; set; }

        [Indexed]
        public DateTime DateAdded { get; set; }

        [Indexed]
        public DateTime DateLastAccessed { get; set; }
    }

    public class SQLCacheEntry
    {
        [PrimaryKey]
        [Indexed]
        public string Key { get; set; }

        public byte[] Data { get; set; }

        [Indexed]
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Note: In the current implementation, this refers to when this item was last *updated*,
        /// rather than the last time it was accessed from the cache
        /// </summary>
        [Indexed]
        public DateTime DateLastAccessed { get; set; }
    }
}
