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
            get { return selectedItem; }
            set { this.RaiseAndSetIfChanged(ref selectedItem, value); }
        }

        public DMSDownloadDataViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(data, dataLock);
            BindingOperations.EnableCollectionSynchronization(selectedData, selectedDataLock);
        }
    }
}
