using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Models.Video
{
    public class Audio
    {
        public int Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
    }

    public class Subtitle
    {
        public string Language { get; set; }
    }

    public class Video
    {
        public double Aspect { get; set; }
        public string Codec { get; set; }
        public int Duration { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class StreamDetails
    {
        public List<Audio> Audio { get; set; }
        public List<Subtitle> Subtitle { get; set; }
        public List<Video> Video { get; set; }
    }
}
