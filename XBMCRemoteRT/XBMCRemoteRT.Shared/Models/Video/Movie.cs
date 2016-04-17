using System.Collections.Generic;
using Newtonsoft.Json;

namespace XBMCRemoteRT.Models.Video
{
    public class Movie
    {
        [JsonProperty(PropertyName="art")]
        public TVShowArt Art { get; set; }
        public List<Cast> Cast { get; set; }
        public List<string> Country { get; set; }
        public string DateAdded { get; set; }
        public List<string> Director { get; set; }
        public string Fanart { get; set; }
        public string File { get; set; }
        public List<string> Genre { get; set; }
        public string ImdbNumber { get; set; }
        public string Label { get; set; }
        public string LastPlayed { get; set; }
        public int MovieId { get; set; }
        public string Mpaa { get; set; }
        public string OriginalTitle { get; set; }
        public string Plot { get; set; }
        public string PlotOutline { get; set; }
        public double Rating { get; set; }
        public Resume Resume { get; set; }
        public int Runtime { get; set; }
        public string Set { get; set; }
        public int SetId { get; set; }
        public List<object> ShowLink { get; set; }
        public string SortTitle { get; set; }
        public StreamDetails StreamDetails { get; set; }
        public List<string> Studio { get; set; }
        public List<object> Tag { get; set; }
        public string Tagline { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public int Top250 { get; set; }
        public string Trailer { get; set; }
        public string Votes { get; set; }
        public List<string> Writer { get; set; }
        public int Year { get; set; }

        //Should be moved to base class if used in more models
        public int PlayCount { get; set; }
    }
}
