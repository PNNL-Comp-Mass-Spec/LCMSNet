using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using DynamicData;
using LcmsNetSDK;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace XcaliburControl
{
    public abstract class XcaliburConfigViewModelBase : ReactiveObject, IDisposable
    {
        protected XcaliburConfigViewModelBase(XcaliburController controller)
        {
            XcaliburAcquisition = controller;

            XcaliburAcquisition.MethodNamesUpdated += OnMethodNames;

            methodComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(XcaliburAcquisition.GetMethodNames());
            });

            methodComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var methodComboBoxOptionsBound).Subscribe();
            MethodComboBoxOptions = methodComboBoxOptionsBound;

            LoadMethodsCommand = ReactiveCommand.Create(LoadMethods);
            RefreshInfoCommand = ReactiveCommand.Create(() => { XcaliburDevices = XcaliburAcquisition.GetXcaliburDeviceNames(); });
            RefreshStatusCommand = ReactiveCommand.Create(() => { XcaliburStatus = XcaliburAcquisition.GetXcaliburRunStatus(); });
            GetDeviceStatusCommand = ReactiveCommand.Create(() => { XcaliburDeviceStatus = XcaliburAcquisition.GetXcaliburDeviceStatus(); });
            GetDeviceInfoCommand = ReactiveCommand.Create(() => { XcaliburDeviceInfo = XcaliburAcquisition.GetXcaliburDeviceInfo(); });
            StartRunCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(StartRun));
            StopQueueCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => XcaliburAcquisition.StopQueue()));
            BrowseForMethodsDirectoryCommand = ReactiveCommand.Create<Window>(BrowseForMethodsDirectory);
            BrowseForDataFileDirectoryCommand = ReactiveCommand.Create<Window>(BrowseForDataFileDirectory);
            ExportMethodTextCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(WriteMethodToFile));

            templateSldFilePath = this.WhenAnyValue(x => x.XcaliburAcquisition.TemplateSldFilePath).ToProperty(this, x => x.TemplateSldFilePath);
            methodsDirectory = this.WhenAnyValue(x => x.XcaliburAcquisition.XcaliburMethodsDirectoryPath).ToProperty(this, x => x.MethodsDirectory);
            dataFileDirectory = this.WhenAnyValue(x => x.XcaliburAcquisition.XcaliburDataFileDirectoryPath).ToProperty(this, x => x.DataFileDirectory);
        }

        ~XcaliburConfigViewModelBase()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            methodComboBoxOptions?.Dispose();
            BrowseForMethodsDirectoryCommand?.Dispose();
            BrowseForDataFileDirectoryCommand?.Dispose();
            LoadMethodsCommand?.Dispose();
            RefreshInfoCommand?.Dispose();
            RefreshStatusCommand?.Dispose();
            StartRunCommand?.Dispose();
            StopQueueCommand?.Dispose();
            GetDeviceStatusCommand?.Dispose();
            GetDeviceInfoCommand?.Dispose();
            templateSldFilePath?.Dispose();
            methodsDirectory?.Dispose();
            dataFileDirectory?.Dispose();

            GC.SuppressFinalize(this);
        }

        private readonly SourceList<string> methodComboBoxOptions = new SourceList<string>();
        private readonly ObservableAsPropertyHelper<string> methodsDirectory;
        private readonly ObservableAsPropertyHelper<string> dataFileDirectory;
        private readonly ObservableAsPropertyHelper<string> templateSldFilePath;
        private string xcaliburDevices;
        private string xcaliburStatus;
        private string selectedMethod;
        private string testDataFileName;
        private string xcaliburDeviceStatus;
        private string xcaliburDeviceInfo;

        public ReadOnlyObservableCollection<string> MethodComboBoxOptions { get; }
        public ReactiveCommand<Window, Unit> BrowseForMethodsDirectoryCommand { get; }
        public ReactiveCommand<Window, Unit> BrowseForDataFileDirectoryCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadMethodsCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> StartRunCommand { get; }
        public ReactiveCommand<Unit, Unit> StopQueueCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportMethodTextCommand { get; }
        public ReactiveCommand<Unit, Unit> GetDeviceStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> GetDeviceInfoCommand { get; }

        public XcaliburController XcaliburAcquisition { get; }

        public string MethodsDirectory => methodsDirectory.Value;
        public string DataFileDirectory => dataFileDirectory.Value;
        public string TemplateSldFilePath => templateSldFilePath.Value;
        public string XcaliburDevices { get => xcaliburDevices; set => this.RaiseAndSetIfChanged(ref xcaliburDevices, value); }
        public string XcaliburStatus { get => xcaliburStatus; set => this.RaiseAndSetIfChanged(ref xcaliburStatus, value); }
        public string SelectedMethod { get => selectedMethod; set => this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        public string TestDataFileName { get => testDataFileName; set => this.RaiseAndSetIfChanged(ref testDataFileName, value); }
        public string XcaliburDeviceStatus { get => xcaliburDeviceStatus; set => this.RaiseAndSetIfChanged(ref xcaliburDeviceStatus, value); }
        public string XcaliburDeviceInfo { get => xcaliburDeviceInfo; set => this.RaiseAndSetIfChanged(ref xcaliburDeviceInfo, value); }

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract bool AllowDirectoryPathUpdate { get; }

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract bool ShowMethodExport { get; }

        /// <summary>
        /// Handles when new method names are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void OnMethodNames(object sender, IEnumerable<string> data)
        {
            methodComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(data.Select(x => x.ToString()));
            });

            // Make sure one method is selected.
            if (methodComboBoxOptions.Count > 0 && !methodComboBoxOptions.Items.Contains(SelectedMethod))
            {
                SelectedMethod = methodComboBoxOptions.Items.First();
            }
        }

        private void StartRun()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
            {
                return;
            }

            XcaliburAcquisition.StartMethod(SelectedMethod);
        }

        private void LoadMethods()
        {
            try
            {
                // The reason we don't just add stuff straight into the user interface here, is to maintain the
                // design pattern that things propagate events to us, since we are not in charge of managing the
                // data.  We will catch an event from adding a method that one was added...and thus update
                // the user interface intrinsically.
                string methodSelected = null;
                if (MethodComboBoxOptions.Count > 0)
                {
                    methodSelected = SelectedMethod;
                }

                XcaliburAcquisition.ReadMethodDirectory();

                if (methodSelected != null)
                {
                    // try to select the last selected method, if it has been loaded back in to the system.
                    SelectedMethod = MethodComboBoxOptions.Contains(methodSelected) ? methodSelected : "";
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                ApplicationLogger.LogError(0, ex.Message, ex);
            }
        }

        private void WriteMethodToFile()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
            {
                return;
            }

            var methodText = XcaliburAcquisition.GetMethodText(SelectedMethod);
            var exportDir = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), "Xcalibur_Methods");
            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            var path = Path.Combine(exportDir, $"{SelectedMethod}_export.txt");

            File.WriteAllText(path, methodText);
            ApplicationLogger.LogMessage(LogLevel.Info, $"Wrote text for method {SelectedMethod} to file '{path}'");
        }

        private void BrowseForMethodsDirectory(Window window)
        {
            var path = BrowseForDirectory("Xcalibur methods directory", MethodsDirectory, window);
            if (!string.IsNullOrWhiteSpace(path))
            {
                SetXcaliburMethodsDirectory(path);
            }
        }

        private void BrowseForDataFileDirectory(Window window)
        {
            var path = BrowseForDirectory("Xcalibur data file directory", DataFileDirectory, window);
            if (!string.IsNullOrWhiteSpace(path))
            {
                SetXcaliburDataFileDirectory(path);
            }
        }

        public void SetXcaliburMethodsDirectory(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                XcaliburAcquisition.XcaliburMethodsDirectoryPath = path;
            }
        }

        public void SetXcaliburDataFileDirectory(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                XcaliburAcquisition.XcaliburDataFileDirectoryPath = path;
            }
        }

        /// <summary>
        /// Implementation for a folder browser
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="startingDirectory"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        /// <example>Example 1: Use Windows Forms folder browser
        /// var dialog = new System.Windows.Forms.FolderBrowserDialog();
        /// dialog.Description = prompt;
        /// using (dialog)
        /// {
        ///     var result = dialog.ShowDialog();
        ///     if (result == System.Windows.Forms.DialogResult.OK &amp;&amp; !string.IsNullOrWhiteSpace(dialog.SelectedPath) &amp;&amp; Directory.Exists(dialog.SelectedPath))
        ///     {
        ///         return dialog.SelectedPath;
        ///     }
        /// }
        /// </example>
        /// <example>Example 2: Use Window API CodePack folder browser (nicer)
        /// var dialog = new CommonOpenFileDialog(prompt)
        /// {
        ///     IsFolderPicker = true,
        ///     DefaultDirectory = startingDirectory,
        ///     EnsurePathExists = true,
        ///     EnsureValidNames = true,
        ///     Title = prompt,
        /// };
        /// using (dialog)
        /// {
        ///     var result = dialog.ShowDialog(window);
        ///     if (result == CommonFileDialogResult.Ok &amp;&amp; !string.IsNullOrWhiteSpace(dialog.FileName) &amp;&amp; Directory.Exists(dialog.FileName))
        ///     {
        ///         return dialog.FileName;
        ///     }
        /// }
        /// </example>
        protected abstract string BrowseForDirectory(string prompt, string startingDirectory, Window window);

        /// <summary>
        /// Load Saved Settings
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract void LoadSavedSettings();

        /// <summary>
        /// Save the settings
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract void SaveSettings();
    }
}
