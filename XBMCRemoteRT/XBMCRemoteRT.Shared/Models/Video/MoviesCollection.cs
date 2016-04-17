using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Models.Video
{
    public class MoviesCollection: IncrementalCollection<Movie>
    {
        private int? maxCount;
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
            if (maxCount == null)
            {
                maxCount = await VideoLibrary.GetMoviesCount(filter);
            }

            Limits limits = new Limits { Start = this.Count, End = this.Count + (int)count };
            
            List<Movie> moreMovies = await VideoLibrary.GetMovies(limits: limits, filter: filter, sort: sort);            
            return moreMovies;
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
