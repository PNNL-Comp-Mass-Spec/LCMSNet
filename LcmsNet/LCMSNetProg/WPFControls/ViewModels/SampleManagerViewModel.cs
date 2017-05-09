using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LcmsNet.Method.Forms;
using LcmsNet.SampleQueue;
using LcmsNet.SampleQueue.Forms;
using LcmsNet.SampleQueue.IO;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
using ReactiveUI;

namespace LcmsNet.WPFControls.ViewModels
{
    public class SampleManagerViewModel : ReactiveObject
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

        private SampleControlViewModel sampleControlViewModel;
        private ColumnManagerViewModel columnManagerViewModel;
        private MethodManagerViewModel methodManagerViewModel;
        private SampleDataManager sampleDataManager;

        public SampleControlViewModel SampleControlViewModel
        {
            get { return sampleControlViewModel; }
            private set { this.RaiseAndSetIfChanged(ref sampleControlViewModel, value); }
        }

        public ColumnManagerViewModel ColumnManagerViewModel
        {
            get { return columnManagerViewModel; }
            private set { this.RaiseAndSetIfChanged(ref columnManagerViewModel, value); }
        }

        public MethodManagerViewModel MethodManagerViewModel
        {
            get { return methodManagerViewModel; }
            private set { this.RaiseAndSetIfChanged(ref methodManagerViewModel, value); }
        }

        public SampleDataManager SampleDataManager
        {
            get { return sampleDataManager; }
            private set { this.RaiseAndSetIfChanged(ref sampleDataManager, value); }
        }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Default constructor that takes cart configuration data.
        /// </summary>
        /// <param name="queue">Sample queue to provide interface to.</param>
        public SampleManagerViewModel(classSampleQueue queue)
        {
            Initialize(queue);
        }

        /// <summary>
        /// Default constructor for design time use.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleManagerViewModel()
        {
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
            SetupCommands();

            if (m_sampleQueue != null)
            {
                m_sampleQueue.SamplesWaitingToRun +=
                    m_sampleQueue_SamplesWaitingToRun;
            }

            //
            // Load up the data to the appropiate sub-controls.
            //
            SampleDataManager = new SampleDataManager(m_sampleQueue);
            SampleControlViewModel = new SampleControlViewModel(m_dmsView, SampleDataManager);
            ColumnManagerViewModel = new ColumnManagerViewModel(m_dmsView, SampleDataManager);
            MethodManagerViewModel = new MethodManagerViewModel(m_dmsView, SampleDataManager);

            var palMethods = new List<string>();
            for (var i = 0; i < 6; i++)
            {
                SampleDataManager.AutoSamplerMethods.Add("method" + i);
                SampleDataManager.AutoSamplerTrays.Add("defaultTray0" + i);
            }

            mdialog_exportQueue = new SaveFileDialog
            {
                Title = "Export Queue",
                Filter = "LCMSNet Queue (*.que)|LCMS VB6 XML File(*.xml)|*.xml|*.que|CSV File (*.csv)|*.csv",
                FileName = "queue.que"
            };

            mdialog_importQueue = new OpenFileDialog
            {
                Title = "Load Queue",
                Filter = "LCMSNet Queue (*.que)|*.que|LCMS VB6 XML File (*.xml)|*.xml"
            };

            mdialog_exportMRMFiles = new FolderBrowserDialog();
            // TODO: // This is the text that is appended to the application title bar
            TitleBarTextAddition = "Sample Queue - " + classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME);
        }

        public void PreviewAvailable(object sender, SampleProgressPreviewArgs e)
        {
            if (e?.PreviewImage == null)
                return;

            try
            {
                SequencePreview = e.PreviewImage.ToBitmapImage();
            }
            catch (Exception)
            {
                // Ignore exceptions here
            }
            e.Dispose();
        }

        private BitmapImage sequencePreview;

        public BitmapImage SequencePreview
        {
            get { return sequencePreview; }
            set { this.RaiseAndSetIfChanged(ref sequencePreview, value); }
        }

        private delegate void DelegateToggleButtons(classSampleQueueArgs args);

        private bool isRunButtonEnabled;
        private bool isStopButtonEnabled;
        private SolidColorBrush runButtonBackColor;
        private SolidColorBrush stopButtonBackColor;
        private string titleBarTextAddition = "";

        public bool IsRunButtonEnabled
        {
            get { return isRunButtonEnabled; }
            set { this.RaiseAndSetIfChanged(ref isRunButtonEnabled, value); }
        }

        public bool IsStopButtonEnabled
        {
            get { return isStopButtonEnabled; }
            set { this.RaiseAndSetIfChanged(ref isStopButtonEnabled, value); }
        }

        public SolidColorBrush RunButtonBackColor
        {
            get { return runButtonBackColor; }
            set { this.RaiseAndSetIfChanged(ref runButtonBackColor, value); }
        }

        public SolidColorBrush StopButtonBackColor
        {
            get { return stopButtonBackColor; }
            set { this.RaiseAndSetIfChanged(ref stopButtonBackColor, value); }
        }

        public string TitleBarTextAddition
        {
            get { return titleBarTextAddition; }
            set { this.RaiseAndSetIfChanged(ref titleBarTextAddition, value); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="runButtonState"></param>
        /// <param name="stopButtonState"></param>
        void ToggleRunButton(bool runButtonState, bool stopButtonState)
        {
            IsRunButtonEnabled = runButtonState;
            if (runButtonState)
            {
                RunButtonBackColor = Brushes.LimeGreen;
            }
            else
            {
                RunButtonBackColor = Brushes.White;
            }
            IsStopButtonEnabled = stopButtonState;
            if (stopButtonState)
            {
                StopButtonBackColor = Brushes.Tomato;
            }
            else
            {
                StopButtonBackColor = Brushes.White;
            }
        }

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
        internal void m_sampleQueue_SamplesWaitingToRun(object sender, classSampleQueueArgs data)
        {
            DetermineIfShouldSetButtons(data);
        }

        /// <summary>
        /// Supplies the list of PAL trays to the sample queue and associated objects.
        /// </summary>
        public void AutoSamplerTrayList(object sender, classAutoSampleEventArgs args)
        {
            var trays = args.TrayList;

            SampleDataManager.AutoSamplerTrays = trays;
        }

        /// <summary>
        /// Supplies a list of instrument methods to the sample queue and associated objects.
        /// </summary>
        public void InstrumentMethodList(object sender, classNetworkStartEventArgs args)
        {
            var methods = args.MethodList;

            SampleDataManager.InstrumentMethods = methods;
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
        internal void ImportQueue(object sender, EventArgs e)
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
        internal void exportMRMFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportMRMFiles();
        }

        /// <summary>
        /// Stops the sample run.
        /// </summary>
        private void StopQueue()
        {
            //
            // This tells anyone else using the samples to STOP!
            // For the scheduler this would tell him to stop first
            // so that he can move the samples appropiately.
            //
            Stop?.Invoke(this, new EventArgs());

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
        private void RunQueue()
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
                delegate (classSampleData data) { return data.RunningStatus != enumSampleRunningStatus.WaitingToRun; });

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
                // But we don't let them continue, they must edit their queue and make it appropiate
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
        /// Caches the queue to the default.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void saveToolStripMenuItem_Click(object sender, EventArgs e)
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
        internal void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mdialog_exportQueue.Title = "Save Queue As";
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".xml", ".que");
            mdialog_exportQueue.FileName = mdialog_exportQueue.FileName.Replace(".csv", ".que");
            mdialog_exportQueue.Filter = "LCMSNet Queue (*.que)|*.que";

            if (mdialog_exportQueue.ShowDialog() == DialogResult.OK)
            {
                m_sampleQueue.CacheQueue(mdialog_exportQueue.FileName);
                // TODO: // This is the text that is appended to the application title bar
                TitleBarTextAddition = "Sample Queue - " + mdialog_exportQueue.FileName;
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
        internal void queueToXMLToolStripMenuItem_Click(object sender, EventArgs e)
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
        internal void queueAsCSVToolStripMenuItem_Click(object sender, EventArgs e)
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
        internal void queueAsCSVToolStripMenuItem_Click_1(object sender, EventArgs e)
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

        #endregion

        public void RestoreUserUIState()
        {
            var timer = new System.Threading.Timer(FixScrollPosition, this, 50, System.Threading.Timeout.Infinite);
        }

        private void FixScrollPosition(object obj)
        {
            SampleControlViewModel.RestoreUserUIState();
        }

        public ReactiveCommand UndoCommand { get; private set; }
        public ReactiveCommand RedoCommand { get; private set; }
        public ReactiveCommand RunQueueCommand { get; private set; }
        public ReactiveCommand StopQueueCommand { get; private set; }

        private void SetupCommands()
        {
            UndoCommand = ReactiveCommand.Create(() =>
            {
                using (SampleControlViewModel.Samples.SuppressChangeNotifications())
                {
                    this.m_sampleQueue.Undo();
                }
            });
            RedoCommand = ReactiveCommand.Create(() =>
            {
                using (SampleControlViewModel.Samples.SuppressChangeNotifications())
                {
                    this.m_sampleQueue.Redo();
                }
            });
            RunQueueCommand = ReactiveCommand.Create(() => this.RunQueue());
            StopQueueCommand = ReactiveCommand.Create(() => this.StopQueue());
        }
    }
}
