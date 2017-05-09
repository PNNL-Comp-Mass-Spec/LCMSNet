
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LcmsNet.Method.Forms;
using LcmsNet.SampleQueue.IO;
using LcmsNet.SampleQueue.ViewModels;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
//using LcmsNet.Devices.NetworkStart;


namespace LcmsNet.SampleQueue.Forms
{
    public partial class formSampleManager2 : Form
    {
        #region  Members

        private const int TIME_SYNCH_WAIT_TIME_MILLISECONDS = 2000;

        public SampleManagerViewModel SampleManagerViewModel { get; private set; }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Default constructor that takes cart configuration data.
        /// </summary>
        /// <param name="queue">Sample queue to provide interface to.</param>
        public formSampleManager2(classSampleQueue queue)
        {
            //
            // Initializes the windows form objects
            //
            InitializeComponent();
            Initialize(queue);
        }

        /// <summary>
        /// Default constructor for constructor.
        /// </summary>
        public formSampleManager2()
        {
            InitializeComponent();
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Initialization code.
        /// </summary>
        /// <param name="queue"></param>
        private void Initialize(classSampleQueue queue)
        {
            //
            // Load up the data to the appropiate sub-controls.
            //
            SampleManagerViewModel = new SampleManagerViewModel(queue);
            sampleManagerView.DataContext = SampleManagerViewModel;

            Text = SampleManagerViewModel.TitleBarTextAddition;
        }

        public void PreviewAvailable(object sender, SampleProgressPreviewArgs e)
        {
            SampleManagerViewModel.PreviewAvailable(sender, e);
        }

        /// <summary>
        /// Supplies the list of PAL trays to the sample queue and associated objects.
        /// </summary>
        public void AutoSamplerTrayList(object sender, classAutoSampleEventArgs args)
        {
            SampleManagerViewModel.AutoSamplerTrayList(sender, args);
        }

        /// <summary>
        /// Supplies a list of instrument methods to the sample queue and associated objects.
        /// </summary>
        public void InstrumentMethodList(object sender, classNetworkStartEventArgs args)
        {
            SampleManagerViewModel.InstrumentMethodList(sender, args);
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Imports the queue into LCMS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportQueue(object sender, EventArgs e)
        {
            SampleManagerViewModel.ImportQueue(sender, e);
        }

        /// <summary>
        /// Exports any MRM files associated with the waiting queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportMRMFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SampleManagerViewModel.exportMRMFilesToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Caches the queue to the default.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SampleManagerViewModel.saveToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Saves the sample queue to another file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SampleManagerViewModel.saveAsToolStripMenuItem_Click(sender, e);
            Text = SampleManagerViewModel.TitleBarTextAddition;
        }

        /// <summary>
        /// Exports the queue to LCMS Version XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SampleManagerViewModel.queueToXMLToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Exports queue to CSV.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueAsCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SampleManagerViewModel.queueAsCSVToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Exports the sample queue to Xcalibur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueAsCSVToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SampleManagerViewModel.queueAsCSVToolStripMenuItem_Click_1(sender, e);
        }

        #endregion

        public void RestoreUserUIState()
        {
            var timer = new System.Threading.Timer(FixScrollPosition, this, 50, System.Threading.Timeout.Infinite);
        }

        private void FixScrollPosition(object obj)
        {
            this.BeginInvoke(new Action(SampleManagerViewModel.RestoreUserUIState));
        }
    }
}