using System.Collections.Generic;

namespace XBMCRemoteRT.Models.Audio
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string AlbumLabel { get; set; }
        public List<string> Artist { get; set; }
        public List<int> ArtistId { get; set; }
        public string Description { get; set; }
        public string DisplayArtist { get; set; }
        public string Fanart { get; set; }
        public List<string> Genre { get; set; }
        public List<int> GenreId { get; set; }
        public string Label { get; set; }
        public List<string> Mood { get; set; }
        public string MusicBrainzAlbumArtistId { get; set; }
        public string MusicBrainzAlbumId { get; set; }
        public int PlayCount { get; set; }
        public int Rating { get; set; }
        public List<string> Style { get; set; }
        public List<string> Theme { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
    }
}
