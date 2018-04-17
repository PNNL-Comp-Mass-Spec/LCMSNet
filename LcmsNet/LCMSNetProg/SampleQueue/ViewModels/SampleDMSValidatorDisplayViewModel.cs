using System;
using System.Collections.Generic;
using LcmsNetSDK.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleDMSValidatorDisplayViewModel : ReactiveObject
    {
        private readonly ReactiveList<SampleDMSValidationViewModel> samples = new ReactiveList<SampleDMSValidationViewModel>();

        public IReadOnlyReactiveList<SampleDMSValidationViewModel> Samples => samples;

        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleDMSValidatorDisplayViewModel()
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
                // TODO: OLD: sampleControl.EnterPressed += sampleControl_EnterPressed;
                samples.Add(new SampleDMSValidationViewModel(sample) { ID = i++ });
            }
        }

        /// <summary>
        /// Gets the flag if the samples are valid or not.
        /// </summary>
        public bool AreSamplesValid => CheckSamples();

        // TODO: OLD: /// <summary>
        // TODO: OLD: /// Moves the focus to the next control.
        // TODO: OLD: /// </summary>
        // TODO: OLD: /// <param name="sender"></param>
        // TODO: OLD: /// <param name="e"></param>
        // TODO: OLD: void sampleControl_EnterPressed(object sender, DMSValidatorEventArgs e)
        // TODO: OLD: {
        // TODO: OLD:     var validator = sender as classDMSBaseControl;
        // TODO: OLD:     if (validator != null)
        // TODO: OLD:     {
        // TODO: OLD:         var id = validator.ID;
        // TODO: OLD:
        // TODO: OLD:         if (id == 0 && e.Modifiers == Keys.Shift)
        // TODO: OLD:         {
        // TODO: OLD:             return;
        // TODO: OLD:         }
        // TODO: OLD:
        // TODO: OLD:         if (id >= m_validatorControls.Count - 1 && e.Modifiers != Keys.Shift)
        // TODO: OLD:         {
        // TODO: OLD:             return;
        // TODO: OLD:         }
        // TODO: OLD:
        // TODO: OLD:         if (e.Modifiers == Keys.Shift)
        // TODO: OLD:         {
        // TODO: OLD:             m_validatorControls[id - 1].SetFocusOn(e);
        // TODO: OLD:         }
        // TODO: OLD:         else
        // TODO: OLD:         {
        // TODO: OLD:             m_validatorControls[id + 1].SetFocusOn(e);
        // TODO: OLD:         }
        // TODO: OLD:     }
        // TODO: OLD: }

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
    }
}
