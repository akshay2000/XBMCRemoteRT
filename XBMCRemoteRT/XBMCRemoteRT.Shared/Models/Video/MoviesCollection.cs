using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Models.Video
{
    public class MoviesCollection: IncrementalCollection<Movie>
    {
        private bool hasMoreItems = true;
        private FilterType filterType;
        public MoviesCollection(FilterType filterType)
        {
            this.filterType = filterType;
        }

        protected override async Task<List<Movie>> LoadMoreItemsImplAsync(CancellationToken c, uint count)
        {
            Limits limits = new Limits { Start = this.Count, End = this.Count + (int)count };
            JObject sort = new JObject(
                new JProperty("order", "ascending"),
                new JProperty("method", "label"),
                new JProperty("ignorearticle", true));

            JObject filter = new JObject(
                    new JProperty("field", "playcount"),
                    new JProperty("operator", "greater"),
                    new JProperty("value", "0"));

            List<Movie> moreMovies;
            switch(filterType)
            {
                case FilterType.New:
                    filter["operator"] = "is";
                    moreMovies = await VideoLibrary.GetMovies(limits, filter, sort);
                    break;
                case FilterType.Watched:
                    filter["operator"] = "greaterthan";
                    moreMovies = await VideoLibrary.GetMovies(limits, filter, sort);
                    break;
                case FilterType.All:
                default:
                    moreMovies = await VideoLibrary.GetMovies(limits: limits, sort: sort);
                    break;
            }
            hasMoreItems = !(moreMovies.Count < count);
            return moreMovies;
        }

        protected override bool HasMoreItemsImpl()
        {
            return hasMoreItems;
        }
    }
}
