using Newtonsoft.Json;
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
        private Filter filter;
        private Sort sort = new Sort { Method = "label", Order = "ascending", IgnoreArticle = true };

        public MoviesCollection(Filter filter = null, Sort sort = null)
        {
            if (filter != null)
                this.filter = filter;

            if (sort != null)
                this.sort = sort;
        }

        protected override async Task<List<Movie>> LoadMoreItemsImplAsync(CancellationToken c, uint count)
        {
            Limits limits = new Limits { Start = this.Count, End = this.Count + (int)count };
            JObject sortJson = JObject.FromObject(sort);
            JObject filterJson = null;
            if (filter != null)
            {
                filterJson = JObject.FromObject(filter);
            }

            List<Movie> moreMovies = await VideoLibrary.GetMovies(limits: limits, filter: filterJson, sort: sortJson);
            hasMoreItems = !(moreMovies.Count < count);
            return moreMovies;
        }

        protected override bool HasMoreItemsImpl()
        {
            return hasMoreItems;
        }
    }
}
