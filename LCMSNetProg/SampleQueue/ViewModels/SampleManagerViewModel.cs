using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LcmsNet.Data;
using LcmsNet.IO.DMS;
using LcmsNet.IO.Sequence;
using LcmsNet.SampleQueue.Views;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.SampleValidation;
using Microsoft.Win32;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleManagerViewModel : ReactiveObject
    {
        #region Events

        /// <summary>
        /// Fired when a sample run should be stopped.
        /// </summary>
        public event EventHandler Stop;

        #endregion

        #region  Members

        /// <summary>
        /// Default extension for the queue files.
        /// </summary>
        private const string CONST_DEFAULT_QUEUE_EXTENSION = ".lcms.csv";

        /// <summary>
        /// Reference to the DMS View.
        /// </summary>
        private readonly DMSDownloadViewModel dmsView;

        private string lastSavedFileName = "queue.lcms.csv";

        /// <summary>
        /// Manages adding the samples to the queue.
        /// </summary>
        private SampleQueue sampleQueue;

        private const int TIME_SYNCH_WAIT_TIME_MILLISECONDS = 2000;

        public SampleControlViewModel SampleControlViewModel { get; }

        // Disabled since the Column and Method view are only used with multiple methods/columns (usually 3 or more), and we currently have no cart configured for that, not even in the past 4 years (as of 2022)
        // They also need more optimization, since they are the primary source of non-responsiveness when changing the queue
        //public ColumnManagerViewModel ColumnManagerViewModel { get; }
        //public MethodManagerViewModel MethodManagerViewModel { get; }

        public SampleDataManager SampleDataManager { get; }

        private SynchronizationContext synchronizationContext;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Default constructor that takes cart configuration data.
        /// </summary>
        /// <param name="queue">Sample queue to provide interface to.</param>
        public SampleManagerViewModel(SampleQueue queue)
        {
            dmsView = new DMSDownloadViewModel();
            sampleQueue = queue;

            if (sampleQueue != null)
            {
                sampleQueue.SamplesWaitingToRun += m_sampleQueue_SamplesWaitingToRun;
            }

            // Load up the data to the appropriate sub-controls.
            SampleDataManager = new SampleDataManager(sampleQueue);
            SampleControlViewModel = new SampleControlViewModel(dmsView, SampleDataManager);
            // Disabled since the Column and Method view are only used with multiple methods/columns (usually 3 or more), and we currently have no cart configured for that, not even in the past 4 years (as of 2022).
            // They also need more optimization, since they are the primary source of non-responsiveness when changing the queue
            //ColumnManagerViewModel = new ColumnManagerViewModel(dmsView, SampleDataManager);
            //MethodManagerViewModel = new MethodManagerViewModel(dmsView, SampleDataManager);

            var palMethods = new List<string>();
            for (var i = 0; i < 6; i++)
            {
                SampleDataManager.AutoSamplerMethods.Add("method" + i);
                SampleDataManager.AutoSamplerTrays.Add("defaultTray0" + i);
            }

            // This is the text that is appended to the application title bar
            TitleBarTextAddition = "Sample Queue - " + LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME);

            synchronizationContext = SynchronizationContext.Current;

            UndoCommand = ReactiveCommand.Create(() => sampleQueue.Undo(), this.WhenAnyValue(x => x.SampleDataManager.CanUndo).ObserveOn(RxApp.MainThreadScheduler));
            RedoCommand = ReactiveCommand.Create(() => sampleQueue.Redo(), this.WhenAnyValue(x => x.SampleDataManager.CanRedo).ObserveOn(RxApp.MainThreadScheduler));
            RunQueueCommand = ReactiveCommand.Create(() => this.RunQueue(), this.WhenAnyValue(x => x.IsRunButtonEnabled).ObserveOn(RxApp.MainThreadScheduler));
            StopQueueCommand = ReactiveCommand.Create(() => this.StopQueue(), this.WhenAnyValue(x => x.IsStopButtonEnabled).ObserveOn(RxApp.MainThreadScheduler));
        }

        /// <summary>
        /// Default constructor for design time use.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleManagerViewModel()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to update UI column visibility based on configured devices.
        /// </summary>
        public void DevicesChanged()
        {
            if (AutoSamplers.ConnectedAutoSamplers.Count == 0)
            {
                // Disable the PAL columns by default if there is no autosampler configured
                SampleControlViewModel.PalTrayColumnVisible = false;
                SampleControlViewModel.PalVialColumnVisible = false;
                SampleControlViewModel.VolumeColumnVisible = false;
            }
            else
            {
                // Disable the PAL columns by default if there is no autosampler configured
                SampleControlViewModel.PalTrayColumnVisible = true;
                SampleControlViewModel.PalVialColumnVisible = true;
                SampleControlViewModel.VolumeColumnVisible = true;
            }
        }

        public void PreviewAvailable(object sender, Method.ViewModels.SampleProgressPreviewArgs e)
        {
            if (e?.PreviewImage == null)
                return;

            SequencePreview = e.PreviewImage;
        }

        private BitmapSource sequencePreview;

        public BitmapSource SequencePreview
        {
            get => sequencePreview;
            private set => this.RaiseAndSetIfChanged(ref sequencePreview, value);
        }

        private bool isRunButtonEnabled;
        private bool isStopButtonEnabled;
        private SolidColorBrush runButtonBackColor;
        private SolidColorBrush stopButtonBackColor;
        private string titleBarTextAddition = "";

        public bool IsRunButtonEnabled
        {
            get => isRunButtonEnabled;
            private set => this.RaiseAndSetIfChanged(ref isRunButtonEnabled, value);
        }

        public bool IsStopButtonEnabled
        {
            get => isStopButtonEnabled;
            private set => this.RaiseAndSetIfChanged(ref isStopButtonEnabled, value);
        }

        public SolidColorBrush RunButtonBackColor
        {
            get => runButtonBackColor;
            private set => this.RaiseAndSetIfChanged(ref runButtonBackColor, value);
        }

        public SolidColorBrush StopButtonBackColor
        {
            get => stopButtonBackColor;
            private set => this.RaiseAndSetIfChanged(ref stopButtonBackColor, value);
        }

        public string TitleBarTextAddition
        {
            get => titleBarTextAddition;
            private set => this.RaiseAndSetIfChanged(ref titleBarTextAddition, value);
        }

        public ReactiveCommand<Unit, Unit> UndoCommand { get; }
        public ReactiveCommand<Unit, Unit> RedoCommand { get; }
        public ReactiveCommand<Unit, Unit> RunQueueCommand { get; }
        public ReactiveCommand<Unit, Unit> StopQueueCommand { get; }

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

        private void DetermineIfShouldSetButtons(SampleQueueArgs data)
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
        private void m_sampleQueue_SamplesWaitingToRun(object sender, SampleQueueArgs data)
        {
            synchronizationContext.Post(d => DetermineIfShouldSetButtons(data), sender);
            //DetermineIfShouldSetButtons(data);
        }

        /// <summary>
        /// Supplies the list of PAL trays to the sample queue and associated objects.
        /// </summary>
        public void AutoSamplerTrayList(object sender, AutoSampleEventArgs args)
        {
            var trays = args.TrayList;

            RxApp.MainThreadScheduler.Schedule(() => SampleDataManager.AutoSamplerTrays = trays);
        }

        /// <summary>
        /// Supplies a list of instrument methods to the sample queue and associated objects.
        /// </summary>
        public void InstrumentMethodList(object sender, NetworkStartEventArgs args)
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
                ApplicationLogger.LogError(0, "The file type for exporting was not recognized.");
                return;
            }
            try
            {
                sampleQueue.SaveQueue(name, writer);
                ApplicationLogger.LogMessage(0, string.Format("The queue was exported to {0}.", name));
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Export Failed!  " + ex.Message, ex);
            }
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Imports the queue into LCMS.
        /// </summary>
        public void ImportQueue()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Load Queue",
                Filter = "LCMSNet Queue (*.lcms.csv)|*.lcms.csv|CSV file|*.csv|LCMS VB6 XML File (*.xml)|*.xml"
            };

            var result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ISampleQueueReader reader = null;
                var extension = Path.GetExtension(fileDialog.FileName).ToLower();

                switch (extension)
                {
                    case ".xml":
                        reader = new QueueImportXML();
                        break;
                    case ".csv":
                        reader = new QueueImportCSV();
                        break;
                    case CONST_DEFAULT_QUEUE_EXTENSION:
                        reader = new QueueCacheLcmsCSV();
                        break;
                }

                try
                {
                    sampleQueue.LoadQueue(fileDialog.FileName, reader);
                    ApplicationLogger.LogMessage(0, string.Format("The queue was successfully imported from {0}.", fileDialog.FileName));
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, string.Format("Could not load the queue {0}", fileDialog.FileName), ex);
                }
            }
        }

        public void ImportQueueFromClipboard()
        {
            try
            {
                var samples = QueueImportClipboard.ReadSamples();
                sampleQueue.LoadQueue(samples);
                ApplicationLogger.LogMessage(0, "The queue was successfully imported from clipboard.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not load the queue from clipboard", ex);
            }
        }

        public void ImportQueueFromCsv()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Load Queue",
                Filter = "CSV (*.csv)|*.csv|CSV text (*.txt)|*.txt"
            };

            var result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    var reader = new QueueImportCSV();
                    sampleQueue.LoadQueue(fileDialog.FileName, reader);
                    ApplicationLogger.LogMessage(0, string.Format("The queue was successfully imported from {0}.", fileDialog.FileName));
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, string.Format("Could not load the queue {0}", fileDialog.FileName), ex);
                }
            }
        }

        /// <summary>
        /// Stops the sample run.
        /// </summary>
        private void StopQueue()
        {
            // This tells anyone else using the samples to STOP!
            // For the scheduler this would tell him to stop first
            // so that he can move the samples appropriately.
            Stop?.Invoke(this, new EventArgs());

            // Moves the samples from the running queue back onto the
            // waiting queue.
            sampleQueue.StopRunningQueue();
            ToggleRunButton(true, false);
        }

        /// <summary>
        /// Example of running a sequence for testing.
        /// </summary>
        private void RunQueue()
        {
            if (sampleQueue.IsRunning)
            {
                ApplicationLogger.LogMessage(0, "Samples are already running.");
                return;
            }
            var samples = sampleQueue.GetRunningQueue();

            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            samples.RemoveAll(
                delegate (SampleData data) { return data.RunningStatus != SampleRunningStatus.WaitingToRun; });

            if (samples.Count < 1)
                return;

            // Make sure the samples pass the minimum QA/QC checks before running!
            // These checks include seeing if the sample has a valid method.
            // Seeing if the sample's method has all of the devices present in the method.
            // Later we will add to make sure none of the devices have an error that has
            // been thrown on them.
            var errors = new Dictionary<SampleData, List<SampleValidationError>>();

            foreach (var reference in SampleValidatorManager.Instance.Validators)
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

                // Of course if we have an error, we just want to display and alert the user.
                // But we don't let them continue, they must edit their queue and make it appropriate
                // before running.
                if (errors.Count > 0)
                {
                    var displayVm = new SampleValidatorErrorDisplayViewModel(errors);
                    var display = new SampleValidatorErrorDisplayWindow() { DataContext = displayVm };
                    display.ShowDialog();
                    return;
                }
            }

            // Then we also want to check for running blocks on the wrong column.
            var badBlocks = BlockValidator.ValidateBlocks(samples);
            if (badBlocks.Count > 0)
            {
                //TODO: Add a notification.
                var displayVm = new SampleBadBlockDisplayViewModel(badBlocks);
                var display = new SampleBadBlockDisplayWindow() { DataContext = displayVm };
                var result = display.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    return;
                }
            }

            //SynchronizeSystemClock();
            sampleQueue.StartSamples();
            ToggleRunButton(false, true);
        }

        /// <summary>
        /// Caches the queue to the default.
        /// </summary>
        public void SaveQueue()
        {
            try
            {
                sampleQueue.CacheQueue();
                ApplicationLogger.LogMessage(0,
                    "Queue saved \"" + LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME) + "\".");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0,
                    "Could not save queue: " + LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME) + "  " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Saves the sample queue to another file.
        /// </summary>
        public void SaveQueueAs()
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Save Queue As",
                FileName = lastSavedFileName.Replace(".xml", ".lcms.csv").Replace(".csv", ".lcms.csv"),
                Filter = "LCMSNet Queue (*.lcms.csv)|*.lcms.csv"
            };

            var result = saveDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                lastSavedFileName = saveDialog.FileName;
                sampleQueue.CacheQueue(lastSavedFileName);
                // This is the text that is appended to the application title bar
                TitleBarTextAddition = "Sample Queue - " + saveDialog.FileName;
                ApplicationLogger.LogMessage(0,
                    "Queue saved to \"" + LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME) +
                    "\" and is now the default queue.");
            }
        }

        /// <summary>
        /// Exports the queue to LCMS Version XML
        /// </summary>
        public void ExportQueueToXML()
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Export Queue to XML for LCMS VB6",
                FileName = lastSavedFileName.Replace(".lcms.csv", ".xml").Replace(".csv", ".xml"),
                Filter = "LCMS VB6 XML (*.xml)|*.xml"
            };

            var result = saveDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                lastSavedFileName = saveDialog.FileName;
                ISampleQueueWriter writer = new QueueExportXML();
                ExportQueue(saveDialog.FileName, writer);
            }
        }

        /// <summary>
        /// Exports queue to CSV.
        /// </summary>
        public void ExportQueueToCsv()
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Export Queue to CSV",
                FileName = lastSavedFileName.Replace(".xml", ".csv").Replace(".lcms.csv", ".csv"),
                Filter = "Comma Separated (*.csv)|*.csv"
            };

            var result = saveDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                lastSavedFileName = saveDialog.FileName;
                ISampleQueueWriter writer = new QueueExportCSV();
                ExportQueue(saveDialog.FileName, writer);
            }
        }

        /// <summary>
        /// Exports the sample queue to Xcalibur
        /// </summary>
        public void ExportQueueToXcalibur()
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Export Queue to XCalibur",
                FileName = lastSavedFileName.Replace(".xml", ".csv").Replace(".lcms.csv", ".csv"),
                Filter = "Comma Separated (*.csv)|*.csv"
            };

            var result = saveDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                lastSavedFileName = saveDialog.FileName;
                ISampleQueueWriter writer = new QueueExportXcaliburCSV();
                ExportQueue(saveDialog.FileName, writer);
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
    }
}
