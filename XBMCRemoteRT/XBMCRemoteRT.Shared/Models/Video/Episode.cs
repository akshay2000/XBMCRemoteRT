using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XBMCRemoteRT.Models.Video
{ 
    public class Uniqueid
    {
        public string Unknown { get; set; }
    }

    public class Episode
    {
        [JsonProperty(PropertyName="art")]
        public EpisodeArt Art { get; set; }
        public string DateAdded { get; set; }

        [JsonProperty(PropertyName="director")]
        public List<string> Directors { get; set; }

        [JsonProperty(PropertyName="episode")]
        public int EpisodeNumber { get; set; }
        public int EpisodeId { get; set; }
        public string Fanart { get; set; }
        public string File { get; set; }
        public string FirstAired { get; set; }
        public string Label { get; set; }
        public string LastPlayed { get; set; }
        public string OriginalTitle { get; set; }
        public int PlayCount { get; set; }
        public string Plot { get; set; }
        public string ProductionCode { get; set; }
        public double Rating { get; set; }
        public Resume Resume { get; set; }
        public int Runtime { get; set; }
        public int Season { get; set; }
        public string ShowTitle { get; set; }
        public StreamDetails StreamDetails { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public int TvShowId { get; set; }
        public Uniqueid UniqueId { get; set; }
        public string Votes { get; set; }

        [JsonProperty(PropertyName = "writer")]
        public List<string> Writers { get; set; }
    }
}
