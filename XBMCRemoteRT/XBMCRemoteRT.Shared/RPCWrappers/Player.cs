using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Video;

namespace XBMCRemoteRT.RPCWrappers
{
    public enum Players { Audio, Video, Picture, None}
    public enum GoTo{ Previous, Next}
    public class Player
    {
        private static JObject defaultPlayerOptions = new JObject(
            new JProperty("repeat", null),
            new JProperty("resume", false),
            new JProperty("shuffled", null));

        public async static Task Open(JObject item, JObject options = null)
        {
            if (options == null)
                options = defaultPlayerOptions;
            JObject parameters = new JObject(
                                           new JProperty("item", item),
                                           new JProperty("options", options));

            await ConnectionManager.ExecuteRPCRequest("Player.Open", parameters);
        }

        public async static Task<int> PlayPause(Players player)
        {
            if (player == Players.None)
                return 0;
            int playerId = getIdFromPlayers(player);
            JObject parameters = new JObject(new JProperty("playerid", playerId));
            JObject responseObjet = await ConnectionManager.ExecuteRPCRequest("Player.PlayPause", parameters);
            int speed = (int)responseObjet["result"]["speed"];
            return speed;
        }

        public async static Task GoTo(Players player, GoTo goTo)
        {
            if (player == Players.None)
                return;
            int playerId = getIdFromPlayers(player);
            JObject parameters = new JObject(
                new JProperty("playerid", playerId),
                new JProperty("to", goTo.ToString().ToLower()));
            await ConnectionManager.ExecuteRPCRequest("Player.GoTo", parameters);
        }

        public async static Task<List<Players>> GetActivePlayers()
        {
            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Player.GetActivePlayers");
            List<Players> listToReturn = new List<Players>();
            JArray playersArray = (JArray)responseObject["result"];
            foreach (JObject t in playersArray)
            {
               listToReturn.Add(getPlayersFromId((int)t["playerid"]));
            }
            return listToReturn;
        }

        public async static Task<PlayerItem> GetItem(Players player)
        {
            if (player == Players.None)
                return new PlayerItem();
            JObject parameters = new JObject(
                new JProperty("playerid", getIdFromPlayers(player)),
                new JProperty("properties",
                    new JArray("title", "artist", "fanart", "thumbnail", "showtitle", "tagline")
                    ));
            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Player.GetItem", parameters);
            JObject itemJson = (JObject)responseObject["result"]["item"];
            PlayerItem playerItem = itemJson.ToObject<PlayerItem>();
            return playerItem;
        }

        public async static Task<PlayerProperties> GetProperties(Players player)
        {
            if (player == Players.None)
                return new PlayerProperties();
            JObject parameters = new JObject(
                new JProperty("playerid", getIdFromPlayers(player)),
                new JProperty("properties", 
                    new JArray("speed", "repeat", "shuffled", "partymode")));
            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Player.GetProperties", parameters);
            JObject propertiesJson = (JObject)responseObject["result"];
            PlayerProperties properties = propertiesJson.ToObject<PlayerProperties>();
            return properties;
        }

        public async static Task SetSpeed(Players player, int speed)
        {
            if (player == Players.None)
                return;
            JObject parameters = new JObject(
               new JProperty("playerid", getIdFromPlayers(player)),
               new JProperty("speed", speed));
            await ConnectionManager.ExecuteRPCRequest("Player.SetSpeed", parameters);
        }

        public async static Task SetPartyMode(Players player, bool partymode)
        {
            if (player == Players.None)
                return;
            JObject parameters = new JObject(
                new JProperty("playerid", getIdFromPlayers(player)),
                new JProperty("partymode", partymode));
            await ConnectionManager.ExecuteRPCRequest("Player.SetPartyMode", parameters);
        }

        public async static Task Stop(Players player)
        {
            if (player == Players.None)
                return;
            JObject parameters = new JObject(
                new JProperty("playerid", getIdFromPlayers(player)));
            await ConnectionManager.ExecuteRPCRequest("Player.Stop", parameters);
        }

        private static int getIdFromPlayers(Players player)
        {
            switch (player)
            {
                case Players.Audio:
                default:
                    return 0;
                case Players.Video:
                    return 1;
                case Players.Picture:
                    return 2;
            }
        }

        private static Players getPlayersFromId(int id)
        {
            switch (id)
            {
                case 0:
                default:
                    return Players.Audio;
                case 1:
                    return Players.Video;
                case 2:
                    return Players.Picture;
            }
        }

        //Extra player methods
        public static async void PlayArtist(Artist artist)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.Programmatic, EventActions.Play, EventNames.PlayArtist, 0);
            await Playlist.Clear(PlayelistType.Audio);
            JObject artistItem = new JObject(new JProperty("artistid", artist.ArtistId));
            await Playlist.Add(PlayelistType.Audio, artistItem);
            JObject playerItem = new JObject(new JProperty("playlistid", 0));
            await Player.Open(playerItem);
        }

        public static async void PlayAlbum(Album album)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.Programmatic, EventActions.Play, EventNames.PlayAlbum, 0);
            await Playlist.Clear(PlayelistType.Audio);
            JObject albumItem = new JObject(new JProperty("albumid", album.AlbumId));
            await Playlist.Add(PlayelistType.Audio, albumItem);
            JObject playerItem = new JObject(new JProperty("playlistid", 0));
            await Player.Open(playerItem);
        }

        public static async void PlayMovie(Movie movie)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.Programmatic, EventActions.Play, EventNames.PlayMovie, 0);
            JObject playerItem = new JObject(new JProperty("movieid", movie.MovieId));
            await Player.Open(playerItem);
        }

        public static async void PlayEpidose(Episode episode)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.Programmatic, EventActions.Play, EventNames.PlayEpisode, 0);
            JObject episodeToOpen = new JObject(new JProperty("episodeid", episode.EpisodeId));
            await Player.Open(episodeToOpen);
        }

        public static async void PlaySong(Song song)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.Programmatic, EventActions.Play, EventNames.PlaySong, 0);
            JObject songToOpen = new JObject(new JProperty("songid", song.SongId));
            await Player.Open(songToOpen);
        }
    }
}
