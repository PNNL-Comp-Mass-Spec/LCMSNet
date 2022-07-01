using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using FluidicsSDK;
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
            ClearCommand = ReactiveCommand.Create(() => reports.Clear(), reports.CountChanged.Select(x => x > 0));
            reports.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var reportsBound).Subscribe();
            Reports = reportsBound;
        }

        private readonly SourceList<ModelCheckReportViewModel> reports = new SourceList<ModelCheckReportViewModel>();

        public ReadOnlyObservableCollection<ModelCheckReportViewModel> Reports { get; }

        private void StatusChangeHandler(object sender, ModelStatusChangeEventArgs e)
        {
            reports.AddRange(e.StatusList.Select(x => new ModelCheckReportViewModel(x)));
        }

        public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    }
}
