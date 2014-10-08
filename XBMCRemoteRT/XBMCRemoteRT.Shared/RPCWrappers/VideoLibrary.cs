using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.Models.Video;

namespace XBMCRemoteRT.RPCWrappers
{
    public class VideoLibrary
    {
        public static async Task<List<Episode>> GetRecentlyAddedEpisodes(Limits limits = null, JObject sort = null)
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("title", "plot", "votes", "rating", "writer", "firstaired", "playcount", "runtime", "director", "productioncode", "season", "episode", "originaltitle", "showtitle", "streamdetails", "lastplayed", "fanart", "thumbnail", "file", "resume", "tvshowid", "dateadded", "uniqueid", "art")
                                    ));

            if (limits != null)
            {
                parameters["limits"] = new JObject(
                                            new JProperty("start", limits.Start),
                                            new JProperty("end", limits.End));
            }

            if (sort != null)
            {
                parameters["sort"] = sort;
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("VideoLibrary.GetRecentlyAddedEpisodes", parameters);
            JArray episodeListObject = (JArray)responseObject["result"]["episodes"];
            List<Episode> listToReturn = episodeListObject != null ? episodeListObject.ToObject<List<Episode>>() : new List<Episode>();
            return listToReturn;
        }

        public static async Task<List<Movie>> GetRecentlyAddedMovies(Limits limits = null, JObject sort = null)
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("title", "genre", "year", "rating", "director", "trailer", "tagline", "plot", "plotoutline", "originaltitle", "lastplayed", "playcount", "writer", "studio", "mpaa", "cast", "country", "imdbnumber", "runtime", "set", "showlink", "streamdetails", "top250", "votes", "fanart", "thumbnail", "file", "sorttitle", "resume", "setid", "dateadded", "tag", "art")
                                    ));

            if (limits != null)
            {
                parameters["limits"] = new JObject(
                                            new JProperty("start", limits.Start),
                                            new JProperty("end", limits.End));
            }

            if (sort != null)
            {
                parameters["sort"] = sort;
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("VideoLibrary.GetRecentlyAddedMovies", parameters);
            JArray movieListObject = (JArray)responseObject["result"]["movies"];
            List<Movie> listToReturn = movieListObject != null ? movieListObject.ToObject<List<Movie>>() : new List<Movie>();
            return listToReturn;
        }

        public static async Task<List<TVShow>> GetTVShows(Limits limits = null, JObject filter = null, JObject sort = null)
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("title", "genre", "year", "rating", "plot", "studio", "mpaa", "cast", "playcount", "episode", "imdbnumber", "premiered", "votes", "lastplayed", "fanart", "thumbnail", "file", "originaltitle", "sorttitle", "episodeguide", "season", "watchedepisodes", "dateadded", "tag", "art")
                                    ));

            if (limits != null)
            {
                parameters["limits"] = new JObject(
                                            new JProperty("start", limits.Start),
                                            new JProperty("end", limits.End));
            }

            if (filter != null)
            {
                parameters["filter"] = filter;
            }

            if (sort != null)
            {
                parameters["sort"] = sort;
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("VideoLibrary.GetTVShows", parameters);
            JArray tvShowsListObject = (JArray)responseObject["result"]["tvshows"];

            List<TVShow> listToReturn = tvShowsListObject != null ? tvShowsListObject.ToObject<List<TVShow>>() : new List<TVShow>();
            return listToReturn;
        }

        public static async Task<List<Movie>> GetMovies(Limits limits = null, JObject filter = null, JObject sort = null)
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("title", "genre", "year", "rating", "director", "trailer", "tagline", "plot", "plotoutline", "originaltitle", "lastplayed", "playcount", "writer", "studio", "mpaa", "cast", "country", "imdbnumber", "runtime", "set", "showlink", "streamdetails", "top250", "votes", "fanart", "thumbnail", "file", "sorttitle", "resume", "setid", "dateadded", "tag", "art")
                                    ));

            if (limits != null)
            {
                parameters["limits"] = new JObject(
                                            new JProperty("start", limits.Start),
                                            new JProperty("end", limits.End));
            }

            if (filter != null)
            {
                parameters["filter"] = filter;
            }

            if (sort != null)
            {
                parameters["sort"] = sort;
            }


            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("VideoLibrary.GetMovies", parameters);
            JArray movieListObject = (JArray)responseObject["result"]["movies"];

            List<Movie> listToReturn = movieListObject != null ? movieListObject.ToObject<List<Movie>>() : new List<Movie>();
            return listToReturn;
        }

        public static async Task<List<Episode>> GetEpisodes(Limits limits = null, JObject filter = null, JObject sort = null, int? tvShowID = null)
        {
            JObject parameters = new JObject(
                               new JProperty("properties",
                                   new JArray("title", "plot", "votes", "rating", "writer", "firstaired", "playcount", "runtime", "director", "productioncode", "season", "episode", "originaltitle", "showtitle", "streamdetails", "lastplayed", "fanart", "thumbnail", "file", "resume", "tvshowid", "dateadded", "uniqueid", "art")
                                   ));

            if (tvShowID != null)
            {
                parameters["tvshowid"] = tvShowID;
            }

            if (limits != null)
            {
                parameters["limits"] = new JObject(
                                            new JProperty("start", limits.Start),
                                            new JProperty("end", limits.End));
            }

            if (filter != null)
            {
                parameters["filter"] = filter;
            }

            if (sort != null)
            {
                parameters["sort"] = sort;
            }


            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("VideoLibrary.GetEpisodes", parameters);
            JArray episodeListObject = (JArray)responseObject["result"]["episodes"];

            List<Episode> listToReturn = episodeListObject != null ? episodeListObject.ToObject<List<Episode>>() : new List<Episode>();
            return listToReturn;
        }
    }
}
