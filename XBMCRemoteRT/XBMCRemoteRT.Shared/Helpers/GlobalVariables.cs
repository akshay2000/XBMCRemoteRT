using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Video;

namespace XBMCRemoteRT.Helpers
{
    public static class GlobalVariables
    {
        public static Album CurrentAlbum { get; set; }

        public static Artist CurrentArtist { get; set; }

        public static TVShow CurrentTVShow { get; set; }

        public static Movie CurrentMovie { get; set; }

        public static PlayerState CurrentPlayerState { get; set; }
    }
}
