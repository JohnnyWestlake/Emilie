using Microsoft.Collections.Extensions;
using System.Collections.Generic;

namespace Emilie.Core.Network
{
    public class HeaderCollection : MultiValueDictionary<string, string>
    {
        public HeaderCollection() : base()
        {
        }

        public HeaderCollection(IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach(var header in headers)
            {
                this.Add(header.Key, header.Value);
            }
        }

        public HeaderCollection(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            foreach (var header in headers)
            {
                this.AddRange(header.Key, header.Value);
            }
        }

        public void Set(string key, string value)
        {
            this.Remove(key);
            this.Add(key, value);
        }
    }
}
