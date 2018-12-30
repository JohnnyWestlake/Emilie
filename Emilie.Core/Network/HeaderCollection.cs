using Microsoft.Collections.Extensions;
using System.Collections.Generic;

namespace Emilie.Core.Network
{
    public class HeaderCollection : MultiValueDictionary<string, string>
    {
        static char[] _splitters { get; } = new char[1] { ',' };

        public HeaderCollection() : base()
        {
        }

        public HeaderCollection(IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (headers == null)
                return;

            foreach(var header in headers)
            {
                if (!string.IsNullOrWhiteSpace(header.Value))
                    foreach (var subheader in header.Value.Split(_splitters, System.StringSplitOptions.RemoveEmptyEntries))
                        this.Add(header.Key, subheader.Trim());
            }
        }

        public HeaderCollection(IEnumerable<(string Key, string Value)> headers)
        {
            if (headers == null)
                return;

            foreach (var (Key, Value) in headers)
            {
                if (!string.IsNullOrWhiteSpace(Value))
                    foreach (var subheader in Value.Split(_splitters, System.StringSplitOptions.RemoveEmptyEntries))
                        this.Add(Key, subheader.Trim());
            }
        }

        public HeaderCollection(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            if (headers == null)
                return;

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
