using System;
using LcmsNetDmsTools;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleDMSValidationViewModel : ReactiveObject
    {
        private readonly static ReactiveList<string> usageTypeComboBoxOptions = new ReactiveList<string>(new string[] {
            "Broken",
            "Cap_Dev",
            "Maintenance",
            "User"});


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

            // TODO: OLD: mcomboBox_usageType.TextChanged += mtextbox_usageType_TextChanged;
            // TODO: OLD: mtextbox_proposalID.TextChanged += mtextbox_proposalID_TextChanged;
            // TODO: OLD: mtextbox_user.TextChanged += mtextbox_user_TextChanged;
            // TODO: OLD: mnum_requestNumber.ValueChanged += mnum_requestNumber_ValueChanged;
            // TODO: OLD: mtextBox_experimentName.TextChanged += mtextBox_experimentName_TextChanged;
            // TODO: OLD: mcomboBox_usageType.KeyUp += KeyUpHandler;
            // TODO: OLD: mtextbox_proposalID.KeyUp += KeyUpHandler;
            // TODO: OLD: mtextbox_user.KeyUp += KeyUpHandler;
            // TODO: OLD: mnum_requestNumber.KeyUp += KeyUpHandler;
            // TODO: OLD: mtextBox_experimentName.KeyUp += KeyUpHandler;

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            UpdateUserInterface();

            this.WhenAnyValue(x => x.IsSampleValid).Subscribe(x => this.RaisePropertyChanged(nameof(SampleNotValid)));
            Sample.DmsData.WhenAnyValue(x => x.DatasetName, x => x.UsageType, x => x.ProposalID, x => x.UserList, x => x.Experiment)
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

        // TODO: OLD: void KeyUpHandler(object sender, KeyEventArgs e)
        // TODO: OLD: {
        // TODO: OLD:     var c = sender as Control;
        // TODO: OLD:     var isEnterType = false;
        // TODO: OLD:     var modifier = e.Modifiers;
        // TODO: OLD:     ComboBox box;
        // TODO: OLD:     NumericUpDown updown;
        // TODO: OLD:     Console.WriteLine(e.KeyCode.ToString());
        // TODO: OLD:
        // TODO: OLD:     switch (e.KeyCode)
        // TODO: OLD:     {
        // TODO: OLD:         case Keys.Enter:
        // TODO: OLD:             Console.WriteLine(@"Enter Pressed!");
        // TODO: OLD:             LcmsNetDataClasses.Logging.ApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Enter pressed!");
        // TODO: OLD:             isEnterType = true;
        // TODO: OLD:             break;
        // TODO: OLD:         case Keys.Up:
        // TODO: OLD:         case Keys.VolumeUp:
        // TODO: OLD:             modifier = Keys.Shift;
        // TODO: OLD:             isEnterType = true;
        // TODO: OLD:
        // TODO: OLD:             box = c as ComboBox;
        // TODO: OLD:             if (box != null)
        // TODO: OLD:             {
        // TODO: OLD:                 isEnterType = false;
        // TODO: OLD:             }
        // TODO: OLD:             else
        // TODO: OLD:             {
        // TODO: OLD:                 updown = c as NumericUpDown;
        // TODO: OLD:                 if (updown != null)
        // TODO: OLD:                 {
        // TODO: OLD:                     isEnterType = false;
        // TODO: OLD:                 }
        // TODO: OLD:             }
        // TODO: OLD:             break;
        // TODO: OLD:         case Keys.Down:
        // TODO: OLD:         case Keys.VolumeDown:
        // TODO: OLD:             modifier = Keys.None;
        // TODO: OLD:             isEnterType = true;
        // TODO: OLD:
        // TODO: OLD:             box = c as ComboBox;
        // TODO: OLD:             if (box != null)
        // TODO: OLD:             {
        // TODO: OLD:                 isEnterType = false;
        // TODO: OLD:             }
        // TODO: OLD:             else
        // TODO: OLD:             {
        // TODO: OLD:                 updown = c as NumericUpDown;
        // TODO: OLD:                 if (updown != null)
        // TODO: OLD:                 {
        // TODO: OLD:                     isEnterType = false;
        // TODO: OLD:                 }
        // TODO: OLD:             }
        // TODO: OLD:             break;
        // TODO: OLD:         case Keys.Right:
        // TODO: OLD:             if (c != null)
        // TODO: OLD:                 SelectNextControl(c, true, true, false, false);
        // TODO: OLD:             break;
        // TODO: OLD:         case Keys.Left:
        // TODO: OLD:             if (c != null)
        // TODO: OLD:                 SelectNextControl(c, false, true, false, false);
        // TODO: OLD:             break;
        // TODO: OLD:     }
        // TODO: OLD:
        // TODO: OLD:     if (isEnterType && c != null)
        // TODO: OLD:     {
        // TODO: OLD:         Console.WriteLine(@"isEnterType: " + c.Name);
        // TODO: OLD:         Console.WriteLine(EnterPressed != null);
        // TODO: OLD:         OnEnterPressed(this, new DMSValidatorEventArgs(c.Name, modifier));
        // TODO: OLD:     }
        // TODO: OLD: }

        #region Methods
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

                ProposalIdValid = classDMSSampleValidator.IsEMSLProposalIDValid(sample);
                IsSampleValid = IsSampleValid && ProposalIdValid;

                UsageTypeValid = classDMSSampleValidator.IsEMSLUsageTypeValid(sample);
                IsSampleValid = IsSampleValid && UsageTypeValid;

                UserListValid = classDMSSampleValidator.IsEMSLUserValid(sample);
                IsSampleValid = IsSampleValid && UserListValid;

                ExperimentValid = classDMSSampleValidator.IsExperimentNameValid(sample);
                IsSampleValid = IsSampleValid && ExperimentValid;
            }

        }
        #endregion
    }
}
