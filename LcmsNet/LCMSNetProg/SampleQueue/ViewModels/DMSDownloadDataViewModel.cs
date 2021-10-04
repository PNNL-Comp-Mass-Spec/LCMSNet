using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using LcmsNet.IO.DMS;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class DMSDownloadDataViewModel : ReactiveObject
    {
        public DMSDownloadDataViewModel(bool userSortable = false)
        {
            UserSortable = userSortable;
            dataList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var dataBound).Subscribe();
            Data = dataBound;
        }

        private DmsDownloadData selectedItem;

        private readonly SourceList<DmsDownloadData> dataList = new SourceList<DmsDownloadData>();

        public ReadOnlyObservableCollection<DmsDownloadData> Data { get; }

        public ObservableCollectionExtended<DmsDownloadData> SelectedData { get; } = new ObservableCollectionExtended<DmsDownloadData>();

        public DmsDownloadData SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }

        public bool UserSortable { get; }

        public event EventHandler<EventArgs> SetSortBatchBlockRunOrder;

        public void AddSample(DmsDownloadData sample)
        {
            dataList.Add(sample);
        }

        public void AddSamples(IEnumerable<DmsDownloadData> samples)
        {
            dataList.AddRange(samples);
        }

        public void RemoveSample(DmsDownloadData sample)
        {
            dataList.Remove(sample);
        }

        public void RemoveSamples(IEnumerable<DmsDownloadData> samples)
        {
            dataList.RemoveMany(samples);
        }

        public void ClearSamples()
        {
            dataList.Clear();
        }

        /// <summary>
        /// Sorts the data by batch, block, and run order
        /// </summary>
        public void SortByBatchBlockRunOrder()
        {
            if (!UserSortable)
            {
                return;
            }

            SetSortBatchBlockRunOrder?.Invoke(this, EventArgs.Empty);
        }
    }
}
