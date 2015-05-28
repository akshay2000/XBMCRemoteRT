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
        private int? maxCount;
        private Filter filter;
        private Sort sort = new Sort { Method = "label", Order = "ascending", IgnoreArticle = true };

        public TVShowsCollection(Filter filter = null, Sort sort = null)
        {
            if (filter != null)
                this.filter = filter;

            if (sort != null)
                this.sort = sort;
        }

        protected override async Task<List<TVShow>> LoadMoreItemsImplAsync(System.Threading.CancellationToken c, uint count)
        {
            if (maxCount == null)
            {
                maxCount = await VideoLibrary.GetTVShowsCount(filter);
            }

            Limits limits = new Limits { Start = this.Count, End = this.Count + (int)count };

            var moreShows = await VideoLibrary.GetTVShows(limits, filter, sort);            
            return moreShows;
        }

        protected override bool HasMoreItemsImpl()
        {
            if (maxCount != null)
                return maxCount > Count;
            else
                return true;
        }
    }
}
