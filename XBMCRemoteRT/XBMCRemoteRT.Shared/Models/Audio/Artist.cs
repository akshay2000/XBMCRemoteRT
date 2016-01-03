using Newtonsoft.Json;
using System.Collections.Generic;

namespace XBMCRemoteRT.Models.Audio
{
    public class Artist
    {
        [JsonProperty(PropertyName="artist")]
        public string ArtistName { get; set; }
        public int ArtistId { get; set; }
        public string Born { get; set; }
        public string Description { get; set; }
        public string Died { get; set; }
        public string Disbanded { get; set; }
        public string Formed { get; set; }
        public List<string> Instrument { get; set; }
        public string Label { get; set; }
        public List<string> Mood { get; set; }
        public string MusicbrainzArtistId { get; set; }
        public List<string> Style { get; set; }
        public List<string> YearsActive { get; set; }
        public string Thumbnail { get; set; }
        public string Fanart { get; set; }
    }
}
