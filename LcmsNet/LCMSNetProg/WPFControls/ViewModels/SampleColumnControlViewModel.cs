using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using LcmsNet.SampleQueue;
using LcmsNetDataClasses.Method;
using ReactiveUI;

namespace LcmsNet.WPFControls.ViewModels
{
    public class SampleColumnControlViewModel : SampleControlViewModel
    {
        public override ReactiveList<SampleViewModel> Samples => FilteredSamples;

        public ReactiveList<SampleViewModel> FilteredSamples { get; private set; }

        // Local "wrapper" around the static class options, for data binding purposes
        public ReactiveList<classLCMethod> LcMethodComboBoxOptions => SampleQueueComboBoxOptions.LcMethodOptions;

        private classLCMethod selectedLCMethod;

        public classLCMethod SelectedLCMethod
        {
            get { return selectedLCMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedLCMethod, value); }
        }

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleColumnControlViewModel() : base()
        {
            FilteredSamples = new ReactiveList<SampleViewModel>();
        }

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public SampleColumnControlViewModel(formDMSView dmsView, SampleDataManager sampleDataManager) : base(dmsView, sampleDataManager)
        {
            FilteredSamples = new ReactiveList<SampleViewModel>();
            BindingOperations.EnableCollectionSynchronization(FilteredSamples, this);
            ResetFilteredSamples();

            this.WhenAnyValue(x => x.SelectedLCMethod).Throttle(TimeSpan.FromSeconds(0.25)).Subscribe(x => ResetFilteredSamplesDispatch());

            SampleDataManager.Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.Sample.LCMethod)))
                .Throttle(TimeSpan.FromSeconds(0.25))
                .Subscribe(x => ResetFilteredSamplesDispatch());

            SampleDataManager.Samples.Changed.Throttle(TimeSpan.FromSeconds(0.25)).Subscribe(x => ResetFilteredSamplesDispatch());
        }

        private void ResetFilteredSamplesDispatch()
        {
            //if (UIDispatcher == null)
            //{
            //    return;
            //}
            //if (!UIDispatcher.CheckAccess())
            //{
            //    UIDispatcher.BeginInvoke(new Action(() => ResetFilteredSamples()));
            //}
            //else
            //{
                ResetFilteredSamples();
            //}
        }

        private void ResetFilteredSamples()
        {
            using (FilteredSamples.SuppressChangeNotifications())
            {
                FilteredSamples.Clear();
                if (SelectedLCMethod == null)
                {
                    FilteredSamples.AddRange(SampleDataManager.Samples);
                    return;
                }
                FilteredSamples.AddRange(SampleDataManager.Samples.Where(x => SelectedLCMethod.Equals(x.Sample.LCMethod)));
            }
        }
    }
}
