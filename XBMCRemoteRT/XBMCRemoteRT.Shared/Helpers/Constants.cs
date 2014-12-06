using System;
using System.Collections.Generic;
using System.Text;

namespace XBMCRemoteRT.Helpers
{
    public static class EventCategories
    {
        public const string UIInteraction = "UIInteraction";
        public const string Programmatic = "Programmatic";
        public const string VoiceCommand = "VoiceCommand";
    }

    public static class EventActions
    {
        public const string Click = "Click";
        public const string Play = "Play";
        public const string VoiceCommand = "VoiceCommand";
    }

    public static class EventNames
    {
        public const string PlayMovie = "PlayMovie";
        public const string PlayEpisode = "PlayEpisode";
        public const string PlayArtist = "PlayArtist";
        public const string PlayAlbum = "PlayAlbum";
        public const string PlaySong = "PlaySong";
    }

    public static class TimingCategories
    {
        public const string LoadTime = "LoadTime";
    }
}
