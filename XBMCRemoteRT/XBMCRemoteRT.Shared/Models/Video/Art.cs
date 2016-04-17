using Newtonsoft.Json;

namespace XBMCRemoteRT.Models.Video
{
    public class EpisodeArt
    {
        public string Thumb { get; set; }

        [JsonProperty(PropertyName = "tvshow.banner")]
        public string Banner { get; set; }

        [JsonProperty(PropertyName = "tvshow.fanart")]
        public string Fanart { get; set; }

        [JsonProperty(PropertyName = "tvshow.poster")]
        public string Poster { get; set; }
    }

    public class TVShowArt
    {
        public string Banner { get; set; }
        public string Fanart { get; set; }
        public string Poster { get; set; }
    }
}
