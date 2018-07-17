using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using LcmsNetData.Data;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleDMSValidatorDisplayViewModel : ReactiveObject
    {
        private readonly ReactiveList<SampleDMSValidationViewModel> samples = new ReactiveList<SampleDMSValidationViewModel>();
        private SampleDMSValidationViewModel selectedItem;

        public IReadOnlyReactiveList<SampleDMSValidationViewModel> Samples => samples;

        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleDMSValidatorDisplayViewModel() : this(new List<SampleData>(1))
        {
            samples.Add(new SampleDMSValidationViewModel(new SampleData(false) { DmsData = new DMSData() { DatasetName = "Test DatasetName", RequestID = 1234567,   EMSLUsageType = "CAP_DEV", UserList = "(none1)", Experiment = "TestExp1", EMSLProposalID = "5" } }, true, false, false, true, true, true));
            samples.Add(new SampleDMSValidationViewModel(new SampleData(false) { DmsData = new DMSData() { DatasetName = "Test DatasetName", RequestID = 12345678,  EMSLUsageType = "CAP_DEV", UserList = "(none2)", Experiment = "TestExp2", EMSLProposalID = "6" } }, false, true, true, false, false, true));
            samples.Add(new SampleDMSValidationViewModel(new SampleData(false) { DmsData = new DMSData() { DatasetName = "Test DatasetName", RequestID = 123456789, EMSLUsageType = "CAP_DEV", UserList = "(none3)", Experiment = "TestExp3", EMSLProposalID = "7" } }, true, true, false, true, true, false));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="samplesList">Sample validation.</param>
        public SampleDMSValidatorDisplayViewModel(List<SampleData> samplesList)
        {
            // Create a sample validator control for each sample.
            var i = 0;
            foreach (var sample in samplesList)
            {
                samples.Add(new SampleDMSValidationViewModel(sample) { ID = i++ });
            }

            FillDownAll = ReactiveCommand.Create<object>(x => FillDown(x, false, FillDownSelection.All));
            FillDownBlank = ReactiveCommand.Create<object>(x => FillDown(x, true, FillDownSelection.All));
            FillDownUsageAll = ReactiveCommand.Create<object>(x => FillDown(x, false, FillDownSelection.EmslUsage));
            FillDownUsageBlank = ReactiveCommand.Create<object>(x => FillDown(x, true, FillDownSelection.EmslUsage));
            FillDownProposalAll = ReactiveCommand.Create<object>(x => FillDown(x, false, FillDownSelection.EmslProposal));
            FillDownProposalBlank = ReactiveCommand.Create<object>(x => FillDown(x, true, FillDownSelection.EmslProposal));
            FillDownUsersAll = ReactiveCommand.Create<object>(x => FillDown(x, false, FillDownSelection.EmslUsers));
            FillDownUsersBlank = ReactiveCommand.Create<object>(x => FillDown(x, true, FillDownSelection.EmslUsers));
            FillDownExperimentAll = ReactiveCommand.Create<object>(x => FillDown(x, false, FillDownSelection.Experiment));
            FillDownExperimentBlank = ReactiveCommand.Create<object>(x => FillDown(x, true, FillDownSelection.Experiment));
        }

        public SampleDMSValidationViewModel SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }

        public ReactiveList<SampleDMSValidationViewModel> SelectedItems { get; } = new ReactiveList<SampleDMSValidationViewModel>();

        public ReactiveCommand<object, Unit> FillDownAll { get; }
        public ReactiveCommand<object, Unit> FillDownBlank { get; }
        public ReactiveCommand<object, Unit> FillDownUsageAll { get; }
        public ReactiveCommand<object, Unit> FillDownUsageBlank { get; }
        public ReactiveCommand<object, Unit> FillDownProposalAll { get; }
        public ReactiveCommand<object, Unit> FillDownProposalBlank { get; }
        public ReactiveCommand<object, Unit> FillDownUsersAll { get; }
        public ReactiveCommand<object, Unit> FillDownUsersBlank { get; }
        public ReactiveCommand<object, Unit> FillDownExperimentAll { get; }
        public ReactiveCommand<object, Unit> FillDownExperimentBlank { get; }

        /// <summary>
        /// Gets the flag if the samples are valid or not.
        /// </summary>
        public bool AreSamplesValid => CheckSamples();

        private enum FillDownSelection
        {
            All,
            EmslUsage,
            EmslProposal,
            EmslUsers,
            Experiment,
        }

        /// <summary>
        /// Checks the sample controls to see if they are valid or not.
        /// </summary>
        /// <returns></returns>
        private bool CheckSamples()
        {
            foreach (var validator in Samples)
            {
                if (validator.IsSampleValid == false)
                    return false;
            }
            return true;
        }

        private void FillDown(object senderVm, bool blankOnly = false, FillDownSelection dataToFill = FillDownSelection.All)
        {
            // senderVm is object because we might get SampleDMSValidatorDisplayViewModel or SampleDMSValidationViewModel
            var sourceItem = senderVm as SampleDMSValidationViewModel;
            var affectableItems = new List<SampleDMSValidationViewModel>();

            if (sourceItem == null)
            {
                sourceItem = SelectedItem ?? Samples.First();
            }

            if (SelectedItems.Count <= 1)
            {
                affectableItems.AddRange(Samples.Where(x => x.ID > sourceItem.ID));
            }
            else
            {
                sourceItem = SelectedItems.OrderBy(x => x.ID).First();
                affectableItems.AddRange(SelectedItems.Where(x => x.ID > sourceItem.ID));
            }

            if (dataToFill == FillDownSelection.All)
            {
                IEnumerable<SampleDMSValidationViewModel> changeItems = affectableItems;
                if (blankOnly)
                {
                    changeItems = affectableItems.Where(x => x.IsItemBlank());
                }

                foreach (var sample in changeItems)
                {
                    //sample.Sample.DmsData.RequestID = sourceItem.Sample.DmsData.RequestID; // This number is only valid for a single sample. Do not copy it down.
                    sample.Sample.DmsData.EMSLUsageType = sourceItem.Sample.DmsData.EMSLUsageType;
                    sample.Sample.DmsData.EMSLProposalID = sourceItem.Sample.DmsData.EMSLProposalID;
                    sample.Sample.DmsData.UserList = sourceItem.Sample.DmsData.UserList;
                    sample.Sample.DmsData.Experiment = sourceItem.Sample.DmsData.Experiment;
                }
            }
            else
            {
                IEnumerable<SampleDMSValidationViewModel> changeItems = affectableItems;
                Func<SampleDMSValidationViewModel, string> selector = null;
                Action<SampleDMSValidationViewModel, SampleDMSValidationViewModel> assigner = null;

                switch (dataToFill)
                {
                    case FillDownSelection.EmslUsage:
                        selector = x => x.Sample.DmsData.EMSLUsageType;
                        assigner = (source, target) => target.Sample.DmsData.EMSLUsageType = source.Sample.DmsData.EMSLUsageType;
                        break;
                    case FillDownSelection.EmslProposal:
                        selector = x => x.Sample.DmsData.EMSLProposalID;
                        assigner = (source, target) => target.Sample.DmsData.EMSLProposalID = source.Sample.DmsData.EMSLProposalID;
                        break;
                    case FillDownSelection.EmslUsers:
                        selector = x => x.Sample.DmsData.UserList;
                        assigner = (source, target) => target.Sample.DmsData.UserList = source.Sample.DmsData.UserList;
                        break;
                    case FillDownSelection.Experiment:
                        selector = x => x.Sample.DmsData.Experiment;
                        assigner = (source, target) => target.Sample.DmsData.Experiment = source.Sample.DmsData.Experiment;
                        break;
                }

                if (blankOnly)
                {
                    changeItems = affectableItems.Where(x => x.IsItemBlank(selector));
                }

                if (assigner == null)
                {
                    return;
                }

                foreach (var sample in changeItems)
                {
                    assigner(sourceItem, sample);
                }
            }
        }
    }
}
