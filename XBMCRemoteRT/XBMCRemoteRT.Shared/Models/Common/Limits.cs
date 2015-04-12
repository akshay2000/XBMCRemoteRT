using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Models.Common
{
    public class Limits
    {
        public Limits() { }

        public Limits(int start, int end)
        {
            Start = start;
            End = end;
        }

        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        [JsonProperty(PropertyName = "end")]
        public int End { get; set; }
    }
}
