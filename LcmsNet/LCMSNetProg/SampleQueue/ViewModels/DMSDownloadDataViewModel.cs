using LcmsNetDataClasses;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class DMSDownloadDataViewModel : ReactiveObject
    {
        private readonly ReactiveList<classSampleData> data = new ReactiveList<classSampleData>();
        private readonly ReactiveList<classSampleData> selectedData = new ReactiveList<classSampleData>();
        private classSampleData selectedItem;

        public ReactiveList<classSampleData> Data => data;
        public ReactiveList<classSampleData> SelectedData => selectedData;

        public classSampleData SelectedItem
        {
            get { return selectedItem; }
            set { this.RaiseAndSetIfChanged(ref selectedItem, value); }
        }

        public DMSDownloadDataViewModel()
        {
        }
    }
}
