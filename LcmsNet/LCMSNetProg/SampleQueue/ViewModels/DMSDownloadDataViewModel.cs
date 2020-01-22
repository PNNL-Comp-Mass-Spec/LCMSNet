using System;
using System.Windows.Data;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class DMSDownloadDataViewModel : ReactiveObject
    {
        private readonly ReactiveList<SampleData> data = new ReactiveList<SampleData>();
        private readonly ReactiveList<SampleData> selectedData = new ReactiveList<SampleData>();
        private SampleData selectedItem;
        private object dataLock = new object();
        private object selectedDataLock = new object();

        public ReactiveList<SampleData> Data => data;
        public ReactiveList<SampleData> SelectedData => selectedData;

        public SampleData SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }

        public bool UserSortable { get; }

        public DMSDownloadDataViewModel(bool userSortable = false)
        {
            UserSortable = userSortable;
            BindingOperations.EnableCollectionSynchronization(data, dataLock);
            BindingOperations.EnableCollectionSynchronization(selectedData, selectedDataLock);
        }

        public event EventHandler<EventArgs> ClearViewSort;

        /// <summary>
        /// Sorts the data by batch, block, and run order
        /// </summary>
        public void SortByBatchBlockRunOrder()
        {
            if (!UserSortable)
            {
                return;
            }

            Data.Sort((x, y) =>
            {
                var result = x.DmsData.Batch.CompareTo(y.DmsData.Batch);
                if (result != 0)
                {
                    return result;
                }

                result = x.DmsData.Block.CompareTo(y.DmsData.Block);
                if (result != 0)
                {
                    return result;
                }

                result = x.DmsData.RunOrder.CompareTo(y.DmsData.RunOrder);
                if (result != 0)
                {
                    return result;
                }

                return x.DmsData.RequestID.CompareTo(y.DmsData.RequestID);
            });

            ClearViewSort?.Invoke(this, EventArgs.Empty);
        }
    }
}
