using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Models.Video
{
    class TVShowsCollection : IncrementalCollection<TVShow>
    {
        private bool hasMoreItems = true;
        protected override async Task<List<TVShow>> LoadMoreItemsImplAsync(System.Threading.CancellationToken c, uint count)
        {
            Limits limits = new Limits { Start = this.Count, End = this.Count + (int)count };
            JObject sort = new JObject(
                new JProperty("order", "ascending"),
                new JProperty("method", "label"),
                new JProperty("ignorearticle", true));

            var moreShows = await VideoLibrary.GetTVShows(limits: limits, sort: sort);
            hasMoreItems = !(moreShows.Count < count);
            return moreShows;
        }

        protected override bool HasMoreItemsImpl()
        {
            return hasMoreItems;
        }
    }
}
