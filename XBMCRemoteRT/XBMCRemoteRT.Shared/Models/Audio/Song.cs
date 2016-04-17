using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Models.Audio
{
    public class Song
    {
        public string Album { get; set; }
        public List<string> AlbumArtist { get; set; }
        public List<int> AlbumArtistiId { get; set; }
        public int AlbumId { get; set; }
        public string Comment { get; set; }
        public int Disc { get; set; }
        public int Duration { get; set; }
        public string File { get; set; }
        public string Label { get; set; }
        public string LastPlayed { get; set; }
        public string Lyrics { get; set; }
        public int PlayCount { get; set; }
        public int SongId { get; set; }
        public int Track { get; set; }

    }
}
