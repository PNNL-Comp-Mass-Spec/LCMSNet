using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using LcmsNetSDK.Data;
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

        private SampleData selectedItem;

        private readonly SourceList<SampleData> dataList = new SourceList<SampleData>();

        public ReadOnlyObservableCollection<SampleData> Data { get; }

        public ObservableCollectionExtended<SampleData> SelectedData { get; } = new ObservableCollectionExtended<SampleData>();

        public SampleData SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }

        public bool UserSortable { get; }

        public event EventHandler<EventArgs> SetSortBatchBlockRunOrder;

        public void AddSample(SampleData sample)
        {
            dataList.Add(sample);
        }

        public void AddSamples(IEnumerable<SampleData> samples)
        {
            dataList.AddRange(samples);
        }

        public void RemoveSample(SampleData sample)
        {
            dataList.Remove(sample);
        }

        public void RemoveSamples(IEnumerable<SampleData> samples)
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
