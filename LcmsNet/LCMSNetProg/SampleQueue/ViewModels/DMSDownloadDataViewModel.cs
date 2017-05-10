using LcmsNetDataClasses;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class DMSDownloadDataViewModel : ReactiveObject
    {
        private ReactiveList<classSampleData> data;
        private ReactiveList<classSampleData> selectedData;
        private classSampleData selectedItem;

        public ReactiveList<classSampleData> Data
        {
            get { return data; }
            private set { this.RaiseAndSetIfChanged(ref data, value); }
        }

        public ReactiveList<classSampleData> SelectedData
        {
            get { return selectedData; }
            private set { this.RaiseAndSetIfChanged(ref selectedData, value); }
        }

        public classSampleData SelectedItem
        {
            get { return selectedItem; }
            set { this.RaiseAndSetIfChanged(ref selectedItem, value); }
        }

        public DMSDownloadDataViewModel()
        {
            Data = new ReactiveList<classSampleData>();
            SelectedData = new ReactiveList<classSampleData>();
        }
    }
}
