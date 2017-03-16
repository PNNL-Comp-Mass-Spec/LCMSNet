//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche, Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/14/2009
//
// Updates:
// - 04/09/2009 (DAC) - Modified so Cache Ops tab isn't visible in Release mode
// - 05/14/2010 (DAC) - Added import/export of cache to SQLite and Excel formats
// - 08/31/2010 (DAC) - Changes resulting from moving part of config to LcmsNet namespace
// - 09/30/2014 (CJW) - modifications to work with refactored sample validations.
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LcmsNet.Method.Forms;
using LcmsNet.SampleQueue.IO;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
//using LcmsNet.Devices.NetworkStart;


namespace LcmsNet.SampleQueue.Forms
{
    public partial class formSampleManager : Form
    {
        #region "Events"

        /// <summary>
        /// Fired when a sample run should be stopped.
        /// </summary>
        public event EventHandler Stop;

        #endregion

        #region  Members

        /// <summary>
        /// Default extension for the queue files.
        /// </summary>
        private const string CONST_DEFAULT_QUEUE_EXTENSION = ".que";

        /// <summary>
        /// Reference to the DMS View.
        /// </summary>
        private formDMSView m_dmsView;

        /// <summary>
        /// Manages adding the samples to the queue.
        /// </summary>
        private classSampleQueue m_sampleQueue;

        /// <summary>
        /// Dialog for exporting samples to a file.
        /// </summary>
        private SaveFileDialog mdialog_exportQueue;

        /// <summary>
        /// Dialog for importing samples from a LCMS Queue file.
        /// </summary>
        private OpenFileDialog mdialog_importQueue;

        /// <summary>
        /// Dialog for export of MRM files
        /// </summary>
        private FolderBrowserDialog mdialog_exportMRMFiles;

        private const int TIME_SYNCH_WAIT_TIME_MILLISECONDS = 2000;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Default constructor that takes cart configuration data.
        /// </summary>
        /// <param name="queue">Sample queue to provide interface to.</param>
        public formSampleManager(classSampleQueue queue)
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
        public formSampleManager()
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
            m_dmsView = new formDMSView();
            m_sampleQueue = queue;
            mdialog_importQueue = new OpenFileDialog();

            if (m_sampleQueue != null)
            {
                m_sampleQueue.SamplesWaitingToRun +=
                    m_sampleQueue_SamplesWaitingToRun;
            }

            //
            // Load up the data to the appropiate sub-controls.
            //
            mcontrol_sequenceView.DMSView = m_dmsView;
            mcontrol_sequenceView.SampleQueue = m_sampleQueue;
            mcontrol_column1.DMSView = m_dmsView;
            mcontrol_column2.DMSView = m_dmsView;
            mcontrol_column3.DMSView = m_dmsView;
            mcontrol_column4.DMSView = m_dmsView;
            mcontrol_column1.SampleQueue = m_sampleQueue;
            mcontrol_column2.SampleQueue = m_sampleQueue;
            mcontrol_column3.SampleQueue = m_sampleQueue;
            mcontrol_column4.SampleQueue = m_sampleQueue;
            mcontrol_column1.Column = classCartConfiguration.Columns[0];
            mcontrol_column2.Column = classCartConfiguration.Columns[1];
            mcontrol_column3.Column = classCartConfiguration.Columns[2];
            mcontrol_column4.Column = classCartConfiguration.Columns[3];

            var palMethods = new List<string>();
            for (var i = 0; i < 6; i++)
            {
                mcontrol_column1.AutoSamplerMethods.Add("method" + i);
                mcontrol_column2.AutoSamplerMethods.Add("method" + i);
                mcontrol_column3.AutoSamplerMethods.Add("method" + i);
                mcontrol_column4.AutoSamplerMethods.Add("method" + i);
                mcontrol_sequenceView.AutoSamplerMethods.Add("method" + i);

                mcontrol_column1.AutoSamplerTrays.Add("defaultTray0" + i);
                mcontrol_column2.AutoSamplerTrays.Add("defaultTray0" + i);
                mcontrol_column3.AutoSamplerTrays.Add("defaultTray0" + i);
                mcontrol_sequenceView.AutoSamplerTrays.Add("defaultTray0" + i);
            }

            mdialog_exportQueue = new SaveFileDialog();
            mdialog_exportQueue.Title = "Export Queue";
            mdialog_exportQueue.Filter =
                "LCMSNet Queue (*.que)|LCMS VB6 XML File(*.xml)|*.xml|*.que|CSV File (*.csv)|*.csv";
            mdialog_exportQueue.FileName = "queue.que";

            mdialog_importQueue = new OpenFileDialog();
            mdialog_importQueue.Title = "Load Queue";
            mdialog_importQueue.Filter = "LCMSNet Queue (*.que)|*.que|LCMS VB6 XML File (*.xml)|*.xml";

            mdialog_exportMRMFiles = new FolderBrowserDialog();
            Text = "Sample Queue - " + classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME);
        }

        public void PreviewAvailable(object sender, SampleProgressPreviewArgs e)
        {
            if (e != null)
            {
                if (e.PreviewImage != null)
                {
                    var width = mpicture_preview.Width;
                    var height = mpicture_preview.Height;

                    if (width <= 0)
                        return;

                    if (height <= 0)
                        return;
                    if (mpicture_preview.Image != null)
                    {
                        mpicture_preview.Image.Dispose();
                    }

                    try
                    {
                        Image x = new Bitmap(width, height);
                        using (var g = Graphics.FromImage(x))
                        {
                            g.DrawImage(e.PreviewImage, 0, 0, width, height);
                            mpicture_preview.Image = x;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    e.Dispose();
                }
            }
        }

        private delegate void DelegateToggleButtons(classSampleQueueArgs args);

        /// <summary>
        ///
        /// </summary>
        /// <param name="runButtonState"></param>
        /// <param name="stopButtonState"></param>
        void ToggleRunButton(bool runButtonState, bool stopButtonState)
        {
            mbutton_run.Enabled = runButtonState;
            if (runButtonState)
            {
                mbutton_run.BackColor = Color.LimeGreen;
            }
            else
            {
                mbutton_run.BackColor = Color.White;
            }
            mbutton_stop.Enabled = stopButtonState;
            if (stopButtonState)
            {
                mbutton_stop.BackColor = Color.Tomato;
            }
            else
            {
                mbutton_stop.BackColor = Color.White;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="runButtonState"></param>
        /// <param name="stopButtonState"></param>
        private void DetermineIfShouldSetButtons(classSampleQueueArgs data)
        {
            var runningCount = data.RunningSamplePosition;
            var totalSamples = data.RunningQueueTotal;

            if (runningCount > 0 && totalSamples > 0)
            {
                ToggleRunButton(false, true);
            }
            else if (runningCount <= 0 && totalSamples > 0)
            {
                ToggleRunButton(true, false);
            }
            else
            {
                ToggleRunButton(false, false);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void m_sampleQueue_SamplesWaitingToRun(object sender, classSampleQueueArgs data)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateToggleButtons(DetermineIfShouldSetButtons), data);
            }
            else
            {
                DetermineIfShouldSetButtons(data);
            }
        }

        /// <summary>
        /// Supplies the list of PAL trays to the sample queue and associated objects.
        /// </summary>
        /// <param name="trayList">List of pal trays.</param>
        public void AutoSamplerTrayList(object sender, classAutoSampleEventArgs args)
        {
            var trays = args.TrayList;

            mcontrol_sequenceView.AutoSamplerTrays = trays;
            mcontrol_column1.AutoSamplerTrays = trays;
            mcontrol_column2.AutoSamplerTrays = trays;
            mcontrol_column3.AutoSamplerTrays = trays;
            mcontrol_column4.AutoSamplerTrays = trays;
        }

        /// <summary>
        /// Supplies a list of instrument methods to the sample queue and associated objects.
        /// </summary>
        /// <param name="methods">List of instrument MS methods.</param>
        public void InstrumentMethodList(object sender, classNetworkStartEventArgs args)
        {
            var methods = args.MethodList;

            mcontrol_sequenceView.InstrumentMethods = methods;
            mcontrol_column1.InstrumentMethods = methods;
            mcontrol_column2.InstrumentMethods = methods;
            mcontrol_column3.InstrumentMethods = methods;
            mcontrol_column4.InstrumentMethods = methods;
        }

        #endregion

        #region Exporting and Importing

        /// <summary>
        /// Handles exporting the queue to a csv file, xml file, or LCMS sample queue.
        /// </summary>
        private void ExportQueue(string name, ISampleQueueWriter writer)
        {
            if (writer == null)
            {
                classApplicationLogger.LogError(0, "The file type for exporting was not recognized.");
                return;
            }
            try
            {
                m_sampleQueue.SaveQueue(name, writer, true);
                classApplicationLogger.LogMessage(0, string.Format("The queue was exported to {0}.", name));
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Export Failed!  " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Exports the MRM Files
        /// </summary>
        private void ExportMRMFiles()
        {
            if (mdialog_exportMRMFiles.ShowDialog() == DialogResult.OK)
            {
                var mrmFilePath = mdialog_exportMRMFiles.SelectedPath;
                var mrmWriter = new classMRMFileExporter();
                m_sampleQueue.SaveQueue(mrmFilePath, mrmWriter, true);
            }
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
            if (mdialog_importQueue.ShowDialog() == DialogResult.OK)
            {
                ISampleQueueReader reader = null;
                var extension = Path.GetExtension(mdialog_importQueue.FileName);

                switch (extension)
                {
                    case ".xml":
                        reader = new classQueueImportXML();
                        break;
                    case CONST_DEFAULT_QUEUE_EXTENSION:
                        reader = new classQueueImportSQLite();
                        break;
                }

                try
                {
                    m_sampleQueue.LoadQueue(mdialog_importQueue.FileName, reader);
                    classApplicationLogger.LogMessage(0,
                        string.Format("The queue was successfully imported from {0}.", mdialog_importQueue.FileName));
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0,
                        string.Format("Could not load the queue {0}", mdialog_importQueue.FileName), ex);
                }
            }
        }

        /// <summary>
        /// Exports any MRM files associated with the waiting queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportMRMFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportMRMFiles();
        }

        /// <summary>
        /// Stops the sample run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_stop_Click(object sender, EventArgs e)
        {
            //
            // This tells anyone else using the samples to STOP!
            // For the scheduler this would tell him to stop first
            // so that he can move the samples appropiately.
            //
            Stop?.Invoke(this, e);

            //
            // Moves the samples from the running queue back onto the
            // waiting queue.
            //
            m_sampleQueue.StopRunningQueue();
            ToggleRunButton(true, false);
        }

        /// <summary>
        /// Example of running a sequence for testing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_run_Click(object sender, EventArgs e)
        {
            if (m_sampleQueue.IsRunning)
            {
                classApplicationLogger.LogMessage(0, "Samples are already running.");
                return;
            }
            var samples = m_sampleQueue.GetRunningQueue();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(
                delegate(classSampleData data) { return data.RunningStatus != enumSampleRunningStatus.WaitingToRun; });

            if (samples.Count < 1)
                return;

            //
            // Make sure the samples pass the minimum QA/QC checks before running!
            //
            // These checks include seeing if the sample has a valid method.
            // Seeing if the sample's method has all of the devices present in the method.
            //
            // Later we will add to make sure none of the devices have an error that has
            // been thrown on them.
            //
            var errors = new Dictionary<classSampleData, List<classSampleValidationError>>();

            foreach (var reference in classSampleValidatorManager.Instance.Validators)
            {
                var validator = reference.Value;
                foreach (var sample in samples)
                {
                    var error = validator.ValidateSamples(sample);
                    if (error.Count > 0)
                    {
                        errors.Add(sample, error);
                    }
                }

                //
                // Of course if we have an error, we just want to display and alert the user.
                // But we dont let them continue, they must edit their queue and make it appropiate
                // before running.
                //
                if (errors.Count > 0)
                {
                    var display = new formSampleValidatorErrorDisplay(errors);
                    display.ShowDialog();
                    return;
                }

                //
                // Then we also want to check for running blocks on the wrong column.
                //

                var badBlocks = validator.ValidateBlocks(samples);
                if (badBlocks.Count > 0)
                {
                    //TODO: Add a notification.
                    var display = new formSampleBadBlockDisplay(badBlocks);
                    var result = display.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            //
            // Then for trigger file checks, we want the sample data for DMS to be complete.
            // So here we validate all of the samples and show the user all samples before running.
            // This way they can validate if they need to all of this information.
            //
            
            var validateSamples = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_VALIDATESAMPLESFORDMS, false) &&
                                  !(string.IsNullOrWhiteSpace(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_DMSTOOL)));
            if (validateSamples)
            {
                var dmsDisplay = new formSampleDMSValidatorDisplay(samples);

                // We don't care what the result is..
                if (dmsDisplay.ShowDialog() == DialogResult.Cancel)
                    return;

                // If samples are not valid...then what?
                if (!dmsDisplay.AreSamplesValid)
                {
                    var result =
                        MessageBox.Show(
                            "Some samples do not contain all necessary DMS information.  This will affect automatic uploads.  Do you wish to continue?",
                            "DMS Sample Validation",
                            MessageBoxButtons.YesNo);

                    if (result == DialogResult.No)
                        return;
                }
                m_sampleQueue.UpdateSamples(samples);
            }
            //SynchronizeSystemClock();
            m_sampleQueue.StartSamples();
            ToggleRunButton(false, true);
        }

        /// <summary>
        /// Run windows time synchronization using w32tm.exe
        /// </summary>
        /// <remarks>This method is unused</remarks>
        private void SynchronizeSystemClock()
        {
            var synchStart = new ProcessStartInfo();
            synchStart.FileName = @"w32tm.exe";
            synchStart.Arguments = @"/resync";
            synchStart.UseShellExecute = false;
            synchStart.CreateNoWindow = true;
            synchStart.RedirectStandardOutput = true;
            synchStart.RedirectStandardError = true;
            var synch = Process.Start(synchStart);
            synch.WaitForExit(TIME_SYNCH_WAIT_TIME_MILLISECONDS);
            var output = string.Empty;
            var error = string.Empty;
            try
            {
                output = synch.StandardOutput.ReadToEnd();
                error = synch.StandardError.ReadToEnd();
            }
            catch (Exception)
            {
                // Ignore errors here
            }
            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                "Synched System Clock " + synch.ExitCode + " " + output + " " + error);
            //MessageBox.Show("Exit code: " + synch.ExitCode + " " + output + " " + error);
        }

        private void btnMRMExport_Click(object sender, EventArgs e)
        {
            ExportMRMFiles();
        }

        private void buttonLogViewer_Click(object sender, EventArgs e)
        {
            //LogViewer.formLogViewerMain formLogViewer = new LogViewer.formLogViewerMain();
            //formLogViewer.Show();
        }

        /// <summary>
        /// Caches the queue to the default.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                m_sampleQueue.CacheQueue(false);
                classApplicationLogger.LogMessage(0,
                    "Queue saved \"" + classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME) + "\".");
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "Could not save queue: " + classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME) + "  " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Saves the sample queue to another file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mdialog_exportQueue.Title = "Save Queue As";
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".xml", ".que");
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".csv", ".que");
            mdialog_exportQueue.Filter = "LCMSNet Queue (*.que)|*.que";

            if (mdialog_exportQueue.ShowDialog() == DialogResult.OK)
            {
                m_sampleQueue.CacheQueue(mdialog_exportQueue.FileName);
                Text = "Sample Queue - " + mdialog_exportQueue.FileName;
                classApplicationLogger.LogMessage(0,
                    "Queue saved to \"" + classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME) +
                    "\" and is now the default queue.");
            }
        }

        /// <summary>
        /// Exports the queue to LCMS Version XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mdialog_exportQueue.Title = "Export Queue to XML for LCMS VB6";
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".que", ".xml");
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".csv", ".xml");
            mdialog_exportQueue.Filter = "LCMS VB6 XML (*.xml)|*.xml";
            if (mdialog_exportQueue.ShowDialog() == DialogResult.OK)
            {
                ISampleQueueWriter writer = new classQueueExportXML();
                ExportQueue(mdialog_exportQueue.FileName, writer);
            }
        }

        /// <summary>
        /// Exports queue to CSV.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueAsCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mdialog_exportQueue.Title = "Export Queue to CSV";
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".xml", ".csv");
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".que", ".csv");
            mdialog_exportQueue.Filter = "Comma Separated (*.csv)|*.csv";

            if (mdialog_exportQueue.ShowDialog() == DialogResult.OK)
            {
                ISampleQueueWriter writer = new classQueueExportCSV();
                ExportQueue(mdialog_exportQueue.FileName, writer);
            }
        }

        /// <summary>
        /// Exports the sample queue to Xcalibur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueAsCSVToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            mdialog_exportQueue.Title = "Export Queue to XCalibur";
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".xml", ".csv");
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".que", ".csv");
            mdialog_exportQueue.Filter = "Comma Separated (*.csv)|*.csv";

            if (mdialog_exportQueue.ShowDialog() == DialogResult.OK)
            {
                ISampleQueueWriter writer = new classQueueExportExcalCSV();
                ExportQueue(mdialog_exportQueue.FileName, writer);
            }
        }

        private void mbutton_undo_Click(object sender, EventArgs e)
        {
            m_sampleQueue.Undo();
        }

        private void mbutton_redo_Click(object sender, EventArgs e)
        {
            m_sampleQueue.Redo();
        }

        private void mcontrol_sequenceView_Load(object sender, EventArgs e)
        {
        }

        #endregion
    }
}