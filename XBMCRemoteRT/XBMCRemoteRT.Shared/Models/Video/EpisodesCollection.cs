using System.Collections.Generic;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Models.Video
{
    class EpisodesCollection : IncrementalCollection<Episode>
    {
        private int? maxCount;
        private Filter filter;
        private Sort sort = new Sort { Method = "label", Order = "ascending", IgnoreArticle = true };
        private int? tvShowId;

        public EpisodesCollection(Filter filter = null, Sort sort = null, int? tvShowId = null)
        {
            if (filter != null)
                this.filter = filter;

            if (sort != null)
                this.sort = sort;

            if (tvShowId != null)
                this.tvShowId = tvShowId;
        }

        protected async override System.Threading.Tasks.Task<List<Episode>> LoadMoreItemsImplAsync(System.Threading.CancellationToken c, uint count)
        {
            if (maxCount == null)
            {
                maxCount = await VideoLibrary.GetEpisodesCount(filter, tvShowId);
            }

            Limits limits = new Limits { Start = this.Count, End = this.Count + (int)count };

            var moreEpisodes = await VideoLibrary.GetEpisodes(limits, filter, sort, tvShowId);
            return moreEpisodes;
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
