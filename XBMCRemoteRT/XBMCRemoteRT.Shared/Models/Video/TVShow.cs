using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Models.Video
{
    public class TVShow
    {
        [JsonProperty(PropertyName = "art")]
        public TVShowArt Art { get; set; }
        public List<Cast> Cast { get; set; }
        public string DateAdded { get; set; }
        public int Episode { get; set; }
        public string EpisodeGuide { get; set; }
        public string Fanart { get; set; }
        public string File { get; set; }
        public List<string> Genre { get; set; }
        public string ImdbNumber { get; set; }
        public string Label { get; set; }
        public string LastPlayed { get; set; }
        public string Mpaa { get; set; }
        public string OriginalTitle { get; set; }
        public int PlayCount { get; set; }
        public string Plot { get; set; }
        public string Premiered { get; set; }
        public double Rating { get; set; }
        public int Season { get; set; }
        public string SortTitle { get; set; }
        public List<string> Studio { get; set; }
        public List<object> Tag { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public int TvShowId { get; set; }
        public string Votes { get; set; }
        public int WatchedEpisodes { get; set; }
        public int Year { get; set; }
    }
}
