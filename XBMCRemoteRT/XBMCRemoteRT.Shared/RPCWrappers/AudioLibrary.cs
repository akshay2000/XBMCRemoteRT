using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Common;

namespace XBMCRemoteRT.RPCWrappers
{
    public class AudioLibrary
    {
        public static async Task<Album> GetAlbumDetails(int albumid)
        {
            JObject parameters = new JObject(
                new JProperty("albumid", albumid),
                new JProperty("properties",
                    new JArray("title", "description", "artist", "genre", "theme", "mood", "style", "type", "albumlabel", "rating", "year", "musicbrainzalbumid", "musicbrainzalbumartistid", "fanart", "thumbnail", "playcount", "genreid", "artistid", "displayartist")
                    ));

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("AudioLibrary.GetAlbumDetails", parameters);

            JObject albumJSON = (JObject)responseObject["result"]["albumdetails"];
            Album albumToRetun = albumJSON.ToObject<Album>();
            return albumToRetun;

        }

        public static async Task<List<Album>> GetRecentlyAddedAlbums(Limits limits = null, Sort sort = null)
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("title", "description", "artist", "genre", "theme", "mood", "style", "type", "albumlabel", "rating", "year", "musicbrainzalbumid", "musicbrainzalbumartistid", "fanart", "thumbnail", "playcount", "genreid", "artistid", "displayartist")
                                    ));

            if (limits != null)
            {
                parameters["limits"] = JObject.FromObject(limits);
            }

            if (sort != null)
            {
                parameters["sort"] = JObject.FromObject(sort);
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("AudioLibrary.GetRecentlyAddedAlbums", parameters);

            JArray albumListObject = (JArray)responseObject["result"]["albums"];
            List<Album> listToReturn = albumListObject != null ? albumListObject.ToObject<List<Album>>() : new List<Album>();
            return listToReturn;
        }

        public static async Task<List<Song>> GetSongs(Filter filter = null, Limits limits = null, Sort sort = null)
        {
            JObject parameters = new JObject(
                                     new JProperty("properties",
                                         new JArray("album", "albumartist", "albumartistid", "albumid", "comment", "disc", "duration", "file", "lastplayed", "lyrics", "musicbrainzartistid", "musicbrainztrackid", "playcount", "track"))
                                             );

            if (limits != null)
            {
                parameters["limits"] = JObject.FromObject(limits);
            }

            if (filter != null)
            {
                parameters["filter"] = JObject.FromObject(filter);
            }

            if (sort != null)
            {
                parameters["sort"] = JObject.FromObject(sort);
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("AudioLibrary.GetSongs", parameters);

            JArray songListObject = (JArray)responseObject["result"]["songs"];
            List<Song> listToReturn = songListObject != null ? songListObject.ToObject<List<Song>>() : new List<Song>();
            return listToReturn;
        }

        public static async Task<List<Artist>> GetArtists(Filter filter = null, Limits limits = null, Sort sort = null)
        {
            JObject parameters = new JObject(
                                     new JProperty("properties",
                                         new JArray("born", "description", "died", "disbanded", "formed", "instrument", "mood", "musicbrainzartistid", "style", "yearsactive", "thumbnail", "fanart"))
                                             );

            if (limits != null)
            {
                parameters["limits"] = JObject.FromObject(limits);
            }

            if (filter != null)
            {
                parameters["filter"] = JObject.FromObject(filter);
            }

            if (sort != null)
            {
                parameters["sort"] = JObject.FromObject(sort);
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("AudioLibrary.GetArtists", parameters);

            JArray artistListObject = (JArray)responseObject["result"]["artists"];
            List<Artist> listToReturn = artistListObject != null ? artistListObject.ToObject<List<Artist>>() : new List<Artist>();
            return listToReturn;
        }

        public static async Task<List<Album>> GetAlbums(Filter filter = null, Limits limits = null, Sort sort = null)
        {
            JObject parameters =
                                new JObject(
                                    new JProperty("properties",
                                        new JArray("title", "description", "artist", "genre", "theme", "mood", "style", "type", "albumlabel", "rating", "year", "musicbrainzalbumid", "musicbrainzalbumartistid", "fanart", "thumbnail", "playcount", "genreid", "artistid", "displayartist"))
                                            );

            if (limits != null)
            {
                parameters["limits"] = JObject.FromObject(limits);
            }

            if (filter != null)
            {
                parameters["filter"] = JObject.FromObject(filter);
            }

            if (sort != null)
            {
                parameters["sort"] = JObject.FromObject(sort);
            }


            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("AudioLibrary.GetAlbums", parameters);

            JArray albumListObject = (JArray)responseObject["result"]["albums"];
            List<Album> listToReturn = albumListObject != null ? albumListObject.ToObject<List<Album>>() : new List<Album>();
            return listToReturn;
        }

        public static async Task<List<Song>> GetAllSongs()
        {
            int stepSize = 50;
            List<Song> toReturn = new List<Song>();
            int returnedStep = stepSize;
            Limits limits = new Limits { Start = 0, End = stepSize };
            while (returnedStep == stepSize)
            {
                List<Song> currentList = await GetSongs(limits: limits);
                returnedStep = currentList.Count;
                toReturn.AddRange(currentList);
                limits.Start += stepSize;
                limits.End += stepSize;
            }
            return toReturn;
        }

        public static async void Scan()
        {
            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("AudioLibrary.Scan");
        }

        public static async void Clean()
        {
            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("AudioLibrary.Clean");
        }
    }
}
