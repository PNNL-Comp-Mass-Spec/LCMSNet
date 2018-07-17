using System;
using LcmsNetDmsTools;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleDMSValidationViewModel : ReactiveObject
    {
        private readonly static ReactiveList<string> usageTypeComboBoxOptions = new ReactiveList<string>(new string[] {
            "BROKEN",
            "CAP_DEV",
            "MAINTENANCE", // TODO: PULL this information from DMS T_EUS_UsageType and cache in SQLite DB!
            "USER"});


        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleDMSValidationViewModel(SampleData sample, bool sampleIsValid, bool canEditEmslData, bool usageTypeValid, bool proposalIdValid, bool userListValid, bool experimentValid)
        {
            this.sample = sample;
            IsSampleValid = sampleIsValid;
            CanChangeEmslData = canEditEmslData;
            UsageTypeValid = usageTypeValid;
            ProposalIdValid = proposalIdValid;
            UserListValid = userListValid;
            ExperimentValid = experimentValid;
        }

        public SampleDMSValidationViewModel(SampleData sample)
        {
            // Make sure the sample is valid so we dont get an exception later when we try to edit it.
            if (sample == null)
                throw new Exception("The sample was null and cannot be displayed.");

            this.sample = sample;

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            UpdateUserInterface();

            this.WhenAnyValue(x => x.IsSampleValid).Subscribe(x => this.RaisePropertyChanged(nameof(SampleNotValid)));
            Sample.DmsData.WhenAnyValue(x => x.DatasetName, x => x.EMSLUsageType, x => x.EMSLProposalID, x => x.UserList, x => x.Experiment)
                .Subscribe(x => this.UpdateUserInterface());
        }

        #region Members

        private readonly SampleData sample;
        private bool canChangeEmslData = true;
        private bool isSampleValid = false;
        private bool usageTypeValid = true;
        private bool proposalIdValid = true;
        private bool userListValid = true;
        private bool experimentValid = true;

        #endregion

        #region Properties

        public int ID { get; set; }

        public IReadOnlyReactiveList<string> UsageTypeComboBoxOptions => usageTypeComboBoxOptions;
        public SampleData Sample => sample;

        public bool CanChangeEmslData
        {
            get { return canChangeEmslData; }
            private set { this.RaiseAndSetIfChanged(ref canChangeEmslData, value); }
        }

        public bool UsageTypeValid
        {
            get { return usageTypeValid; }
            private set { this.RaiseAndSetIfChanged(ref usageTypeValid, value); }
        }

        public bool ProposalIdValid
        {
            get { return proposalIdValid; }
            private set { this.RaiseAndSetIfChanged(ref proposalIdValid, value); }
        }

        public bool UserListValid
        {
            get { return userListValid; }
            private set { this.RaiseAndSetIfChanged(ref userListValid, value); }
        }

        public bool ExperimentValid
        {
            get { return experimentValid; }
            private set { this.RaiseAndSetIfChanged(ref experimentValid, value); }
        }

        /// <summary>
        /// Gets to the flag indicating if this sample is valid or not.
        /// </summary>
        public bool IsSampleValid
        {
            get { return isSampleValid; }
            private set { this.RaiseAndSetIfChanged(ref isSampleValid, value); }
        }

        public bool SampleNotValid
        {
            get { return !isSampleValid; }
        }

        #endregion

        #region Methods

        public bool IsItemBlank(Func<SampleDMSValidationViewModel, string> itemSelector = null)
        {
            if (Sample.DmsData.RequestID > 0)
            {
                return false;
            }

            if (itemSelector == null)
            {
                return string.IsNullOrWhiteSpace(Sample.DmsData.EMSLUsageType) && string.IsNullOrWhiteSpace(Sample.DmsData.EMSLProposalID) &&
                       string.IsNullOrWhiteSpace(Sample.DmsData.UserList) && string.IsNullOrWhiteSpace(Sample.DmsData.Experiment);
            }

            return string.IsNullOrWhiteSpace(itemSelector(this));
        }

        /// <summary>
        /// Checks the sample and updates the user interface accordingly.
        /// </summary>
        private void UpdateUserInterface()
        {
            if (sample == null)
                return;

            IsSampleValid = true;

            if (sample.DmsData.RequestID > 0)
            {
                CanChangeEmslData = false;
                UsageTypeValid = true;
                ProposalIdValid = true;
                UserListValid = true;
                ExperimentValid = true;
            }
            else
            {
                CanChangeEmslData = true;

                ProposalIdValid = DMSSampleValidator.IsEMSLProposalIDValid(sample);
                IsSampleValid = IsSampleValid && ProposalIdValid;

                UsageTypeValid = DMSSampleValidator.IsEMSLUsageTypeValid(sample);
                IsSampleValid = IsSampleValid && UsageTypeValid;

                UserListValid = DMSSampleValidator.IsEMSLUserValid(sample);
                IsSampleValid = IsSampleValid && UserListValid;

                ExperimentValid = DMSSampleValidator.IsExperimentNameValid(sample);
                IsSampleValid = IsSampleValid && ExperimentValid;
            }

        }
        #endregion
    }
}
