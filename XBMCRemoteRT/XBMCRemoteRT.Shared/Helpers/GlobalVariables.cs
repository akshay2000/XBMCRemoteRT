using GoogleAnalytics;
using GoogleAnalytics.Core;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Files;
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

        public static Source CurrentSource { get; set; }

        public static File CurrentFile { get; set; }

        public static Tracker CurrentTracker = EasyTracker.GetTracker();
    }
}
