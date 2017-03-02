using System;
using System.Windows.Forms;
using System.Collections.Generic;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Data;

namespace LcmsNet.SampleQueue.Forms
{
    public partial class formSampleDMSValidatorDisplay : Form
    {
        /// <summary>
        /// List of validator controls so we can check after editing if the samples are valid.
        /// </summary>
        private readonly List<classDMSBaseControl> m_validatorControls;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="samples">Sample validation.</param>
        public formSampleDMSValidatorDisplay(List<classSampleData> samples)
        {
            InitializeComponent();

            //
            // Always scroll if we have too many items to display.
            //
            AutoScroll = true;

            m_validatorControls = new List<classDMSBaseControl>();

            //
            // Create a sample validator control for each sample.
            //
            var i = 0;
            foreach (var sample in samples)
            {
                try
                {
                    var sampleControl =
                        Activator.CreateInstance(
                            LcmsNetSDK.classDMSToolsManager.Instance.Validator.DMSValidatorControl, sample) as
                            classDMSBaseControl; //new controlDMSValidator(sample);
                    if (sampleControl == null)
                    {
                        throw new InvalidOperationException("Sample control instantiation failed!");
                    }
                    sampleControl.Dock = DockStyle.Top;
                    sampleControl.ID = i++;
                    sampleControl.EnterPressed += new EventHandler<DMSValidatorEventArgs>(sampleControl_EnterPressed);
                    m_validatorControls.Add(sampleControl);
                    panel1.Controls.Add(sampleControl);

                    sampleControl.BringToFront();
                }
                catch (Exception ex)
                {
                    // We get this if the sample is null.  Just pass up the chain for now.
                    throw ex;
                }
            }

            FormClosing += new FormClosingEventHandler(formSampleDMSValidatorDisplay_FormClosing);
        }

        /// <summary>
        /// Gets the flag if the samples are valid or not.
        /// </summary>
        public bool AreSamplesValid
        {
            get { return CheckSamples(); }
        }

        /// <summary>
        /// Moves the focus to the next control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sampleControl_EnterPressed(object sender, DMSValidatorEventArgs e)
        {
            var validator = sender as classDMSBaseControl;
            if (validator != null)
            {
                var id = validator.ID;

                if (id == 0 && e.Modifiers == Keys.Shift)
                {
                    return;
                }

                if (id >= m_validatorControls.Count - 1 && e.Modifiers != Keys.Shift)
                {
                    return;
                }

                if (e.Modifiers == Keys.Shift)
                {
                    m_validatorControls[id - 1].SetFocusOn(e);
                }
                else
                {
                    m_validatorControls[id + 1].SetFocusOn(e);
                }
            }
        }

        /// <summary>
        /// Checks the sample controls to see if they are valid or not.
        /// </summary>
        /// <returns></returns>
        private bool CheckSamples()
        {
            foreach (var validator in m_validatorControls)
            {
                if (validator.IsSampleValid == false)
                    return false;
            }
            return true;
        }

        #region Form Event Handlers

        /// <summary>
        /// Prevents the dispose method from being called if we are going to use results from
        /// the sample list objects to see if they are valid at a later time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formSampleDMSValidatorDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// Hide the form so we don't dispose of it yet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Hide the form so we don't dispose of it yet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        #endregion
    }
}