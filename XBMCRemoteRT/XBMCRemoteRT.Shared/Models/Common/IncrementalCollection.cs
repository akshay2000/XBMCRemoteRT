using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Models.Common
{
    public abstract class IncrementalCollection<T> : List<T>, ISupportIncrementalLoading, INotifyCollectionChanged
    {
        public IncrementalCollection() : base() { }

        public IncrementalCollection(IEnumerable<T> collection) : base(collection) { }

        #region ISupportIncrementalLoading

        public bool HasMoreItems
        {
            get { return HasMoreItemsImpl(); }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        #endregion

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            var moreItems = await LoadMoreItemsImplAsync(c, count);
            int baseIndex = this.Count;
            base.AddRange(moreItems);
            NotifyOfInsertedItems(baseIndex, moreItems.Count);
            return new LoadMoreItemsResult { Count = (uint)moreItems.Count };
        }       

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        void NotifyOfInsertedItems(int baseIndex, int count)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, base[i + baseIndex], i + baseIndex);
                CollectionChanged(this, args);
            }
        }

        #region Abstracts

        protected abstract Task<List<T>> LoadMoreItemsImplAsync(CancellationToken c, uint count);
        protected abstract bool HasMoreItemsImpl();

        #endregion
    }
}
