using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LcmsNetDataClasses;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class ModelCheckReportsViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the model check reports view control that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ModelCheckReportsViewModel()
        {
        }

        public ModelCheckReportsViewModel(IModelCheckController cntrlr)
        {
            cntrlr.ModelStatusChangeEvent += StatusChangeHandler;
            ClearCommand = ReactiveCommand.Create(() => reports.Clear(), this.WhenAnyValue(x => x.reports.Count).Select(x => x > 0));
        }

        private readonly ReactiveList<ModelCheckReportViewModel> reports = new ReactiveList<ModelCheckReportViewModel>();

        public IReadOnlyReactiveList<ModelCheckReportViewModel> Reports => reports;

        private void StatusChangeHandler(object sender, ModelStatusChangeEventArgs e)
        {
            using (reports.SuppressChangeNotifications())
            {
                reports.AddRange(e.StatusList.Select(x => new ModelCheckReportViewModel(x)));
            }
        }

        public ReactiveCommand<Unit, Unit> ClearCommand { get; private set; }
    }
}
