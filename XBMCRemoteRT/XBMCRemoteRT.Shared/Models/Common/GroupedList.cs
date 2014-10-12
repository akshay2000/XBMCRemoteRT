using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Models.Common
{
    public class ListGroup<T> : List<T>
    {
        public string Key { get; private set; }

        public ListGroup(string key)
        {
            Key = key;
        }

        public ListGroup(string key, IEnumerable<T> items)
            : base(items)
        {
            Key = key;
        }
    }
}
