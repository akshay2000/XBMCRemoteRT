using Newtonsoft.Json;

namespace XBMCRemoteRT.Models.Common
{
    public class Filter
    {
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "operator")]
        public string Operator { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string value { get; set; }
    }
}
