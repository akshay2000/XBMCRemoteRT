using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Video;

namespace XBMCRemoteRT.RPCWrappers
{
    public enum PlayelistType { Audio, Video, Picture}
    public class Playlist
    {
        public async static Task Add(PlayelistType playlistType, JObject item)
        {
            int playlistId = GetPlaylistId(playlistType);
            JObject parameters = new JObject(
                               new JProperty("item", item),
                               new JProperty("playlistid", playlistId));

            await ConnectionManager.ExecuteRPCRequest("Playlist.Add", parameters);
        }

        public async static Task Remove(PlayelistType playlistType, int position)
        {
            int playlistId = GetPlaylistId(playlistType);
            JObject parameters = new JObject(
                               new JProperty("position", position),
                               new JProperty("playlistid", playlistId));

            await ConnectionManager.ExecuteRPCRequest("Playlist.Remove", parameters);
        }

        public async static Task Clear(PlayelistType playlistType)
        {
            int playlistId = GetPlaylistId(playlistType);
            JObject parameters = new JObject(
                                new JProperty("playlistid", playlistId));
            await ConnectionManager.ExecuteRPCRequest("Playlist.Clear", parameters);
        }

        // Returns the current playlist for video/audio
        public async static Task<IEnumerable<Object>> GetItems(PlayelistType playlistType)
        {
            int playlistId = GetPlaylistId(playlistType);
            JObject parameters = new JObject(new JProperty("playlistid", playlistId));

            if (playlistType == PlayelistType.Audio)
                parameters.Add(new JProperty("properties", new JArray("album", "artist", "duration")));
            else if (playlistType == PlayelistType.Video)
                parameters.Add(new JProperty("properties", new JArray("runtime", "showtitle", "season", "title", "artist")));

            JObject res = await ConnectionManager.ExecuteRPCRequest("Playlist.GetItems", parameters);
            JArray itemsListObject = (JArray)res["result"]["items"];

            if (itemsListObject == null)
                return new List<Object>();

            if (playlistType == PlayelistType.Audio)
            {
                var songs = itemsListObject.Select(i => new Song()
                {
                    SongId = i["id"].ToObject<int>(),
                    Album = i["album"].ToString(),
                    AlbumArtist = i["artist"].ToObject<List<String>>(),
                    Label = i["label"].ToString(),
                    Duration = i["duration"].ToObject<int>()
                }).ToList();

                return songs;
            }

            //TODO: check the video return output -- currently in the app this is not used but should it be in the future, this will have to be checked
            if (playlistType == PlayelistType.Video)
            {
                List<Movie> listToReturn = itemsListObject.ToObject<List<Movie>>();
                return listToReturn;
            }

            return new List<Object>();
        }

        private static int GetPlaylistId(PlayelistType playlistType)
        {
            int playlistId;
            switch (playlistType)
            {
                case PlayelistType.Video:
                    playlistId = 1;
                    break;
                case PlayelistType.Picture:
                    playlistId = 2;
                    break;
                case PlayelistType.Audio:
                default:
                    playlistId = 0;
                    break;
            }
            return playlistId;
        }

        //Extra queue methods
        public static async void AddAlbum(Album album)
        {
            JObject albumItem = new JObject(new JProperty("albumid", album.AlbumId));
            await Playlist.Add(PlayelistType.Audio, albumItem);
        }

        public static async void AddArtist(Artist artist)
        {
            JObject artistItem = new JObject(new JProperty("artistid", artist.ArtistId));
            await Playlist.Add(PlayelistType.Audio, artistItem);
        }

        public static async void AddSong(Song song)
        {
            JObject songItem = new JObject(new JProperty("songid", song.SongId));
            await Playlist.Add(PlayelistType.Audio, songItem);
        }

        public static async void AddMovie(Movie movie)
        {
            JObject movieItem = new JObject(new JProperty("movieid", movie.MovieId));
            await Playlist.Add(PlayelistType.Video, movieItem);
        }
    }
}
