using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using LcmsNet.IO;
using LcmsNetSDK;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    /// <summary>
    /// Control that allows the user to edit/create/delete their own methods.
    /// </summary>
    public class LCMethodEditorViewModel : ReactiveObject
    {
        /// <summary>
        /// Constant defining where the LC-Methods are stored.
        /// </summary>
        private const string CONST_METHOD_FOLDER_PATH = "LCMethods";

        /// <summary>
        /// The amount of time to delay between rendering frames for an alignment.
        /// </summary>
        private const int CONST_DEFAULT_ANIMATION_DELAY_TIME = 50;

        /// <summary>
        /// The number of updates to ignore the rendering update calls.
        /// </summary>
        private const int CONST_DEFAULT_RENDER_WAIT_COUNT = 5;

        /// <summary>
        /// Render Update counts
        /// </summary>
        private int renderUpdateCount;

        private LCMethodTimelineViewModel methodTimelineThroughput;
        private LCMethodStageViewModel acquisitionStage;
        private LCMethodSelectionViewModel selectedMethods;
        private MethodPreviewOptions methodPreviewOptions;

        /// <summary>
        /// Constructor that allows for users to edit methods.
        /// </summary>
        public LCMethodEditorViewModel()
        {
            MethodTimelineThroughput = new LCMethodTimelineViewModel();
            AcquisitionStage = new LCMethodStageViewModel();
            SelectedMethods = new LCMethodSelectionViewModel();
            MethodPreviewOptions = new MethodPreviewOptions();

            MethodFolderPath = CONST_METHOD_FOLDER_PATH;

            UpdateConfiguration();

            SelectedMethods.MethodAdded += SelectedMethods_MethodAdded;
            SelectedMethods.MethodDeleted += SelectedMethods_MethodDeleted;
            SelectedMethods.MethodUpdated += SelectedMethods_MethodUpdated;

            PreviewModeComboBoxOptions = new List<LCMethodRenderMode> { LCMethodRenderMode.Column, }.AsReadOnly();

            MethodPreviewOptions = new MethodPreviewOptions
            {
                FrameDelay = CONST_DEFAULT_RENDER_WAIT_COUNT,
                Animate = false,
                AnimateDelay = CONST_DEFAULT_ANIMATION_DELAY_TIME
            };
            renderUpdateCount = 0;
        }

        /// <summary>
        /// Updates the configuration data and the user interface.
        /// </summary>
        private void UpdateConfiguration()
        {
            AcquisitionStage.UpdateConfiguration();
        }

        /// <summary>
        /// Gets or sets the method animation preview options.
        /// </summary>
        public MethodPreviewOptions MethodPreviewOptions
        {
            get => methodPreviewOptions;
            set => this.RaiseAndSetIfChanged(ref methodPreviewOptions, value);
        }

        /// <summary>
        /// Gets or sets what folder path the methods are stored in.
        /// </summary>
        public string MethodFolderPath { get; set; }

        public LCMethodTimelineViewModel MethodTimelineThroughput
        {
            get => methodTimelineThroughput;
            set => this.RaiseAndSetIfChanged(ref methodTimelineThroughput, value);
        }

        public LCMethodStageViewModel AcquisitionStage
        {
            get => acquisitionStage;
            set => this.RaiseAndSetIfChanged(ref acquisitionStage, value);
        }

        public LCMethodSelectionViewModel SelectedMethods
        {
            get => selectedMethods;
            set => this.RaiseAndSetIfChanged(ref selectedMethods, value);
        }

        public ReadOnlyCollection<LCMethodRenderMode> PreviewModeComboBoxOptions { get; }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void SelectedMethods_MethodDeleted(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void SelectedMethods_MethodAdded(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void SelectedMethods_MethodUpdated(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Builds the LC Method.
        /// </summary>
        /// <returns>A LC-Method if events are defined.  Null if events are not.</returns>
        public LCMethod BuildMethod()
        {
            var method = LCMethodBuilder.BuildMethod(AcquisitionStage.LCEvents);
            if (method == null)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_USER, "Cannot create the LC-method from an empty event list.  You need to add events to the method.");
                return null;
            }

            var column = AcquisitionStage.GetColumn();
            if (column < 0)
            {
                method.IsSpecialMethod = true;
            }
            else
            {
                method.IsSpecialMethod = false;
            }
            method.Column = column;
            method.AllowPreOverlap = AcquisitionStage.AllowPreOverlap;
            method.AllowPostOverlap = AcquisitionStage.AllowPostOverlap;
            return method;
        }

        /// <summary>
        /// Renders the throughput timelines.
        /// </summary>
        /// <param name="methods">Methods to render for showing throughput.</param>
        private void RenderThroughput(List<LCMethod> methods)
        {
            if (methods == null)
                return;

            MethodTimelineThroughput.RenderLCMethod(methods);
            MethodTimelineThroughput.Refresh();
        }

        /// <summary>
        /// Aligns then renders the methods selected from the user interface.
        /// </summary>
        private void RenderAlignMethods()
        {
            // Align methods - blindly!
            try
            {
                var optimizer = new LCMethodOptimizer();
                optimizer.UpdateRequired += optimizer_UpdateRequired;
                var methods = SelectedMethods.SelectedMethods;
                renderUpdateCount = 0;

                if (methods.Count > 0)
                    methods[0].SetStartTime(TimeKeeper.Instance.Now);

                optimizer.AlignMethods(methods);
                RenderThroughput(methods);

                optimizer.UpdateRequired -= optimizer_UpdateRequired;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// On an update, the optimizer will tell us the status of the methods here.
        /// </summary>
        void optimizer_UpdateRequired(LCMethodOptimizer sender)
        {
            // Render the updates if we animating and past the rendering threshold.
            if (MethodPreviewOptions.Animate && renderUpdateCount++ >= MethodPreviewOptions.FrameDelay)
            {
                RenderThroughput(sender.Methods);

                // reset the number of update calls we have seen.
                renderUpdateCount = 0;

                Thread.Sleep(MethodPreviewOptions.AnimateDelay);
            }
        }

        /// <summary>
        /// Saves the given method to file.
        /// </summary>
        /// <param name="method"></param>
        public bool SaveMethod(LCMethod method)
        {
            // Method is null!!! OH MAN - that isn't my fault so we'll ignore it!
            if (method == null)
                return false;

            // Construct the path
            var path = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), LCMethodFactory.CONST_LC_METHOD_FOLDER);
            path = Path.Combine(path, method.Name + LCMethodFactory.CONST_LC_METHOD_EXTENSION);

            // Write the method out!
            return LCMethodXmlFile.WriteMethod(path, method);
        }

        /// <summary>
        /// Opens a method.
        /// </summary>
        public void OpenMethod(string path)
        {
            var errors = new List<Exception>();
            var method = LCMethodXmlFile.ReadMethod(path, errors);

            if (method != null)
            {
                LCMethodManager.Manager.AddOrUpdateMethod(method);
            }
        }

        /// <summary>
        /// Loads method stored in the method folder directory.
        /// </summary>
        public void LoadMethods()
        {
            var methodPath = PersistDataPaths.GetDirectoryLoadPathCheckFiles(MethodFolderPath, "*.xml");
            var methods = Directory.GetFiles(methodPath, "*.xml");
            foreach (var method in methods)
            {
                ApplicationLogger.LogMessage(0, "Loading method " + method);
                OpenMethod(method);
            }

            ApplicationLogger.LogMessage(0, "Methods loaded.");
        }
    }

    public class MethodEditingEventArgs : EventArgs
    {
        public MethodEditingEventArgs(string methodName)
        {
            Name = methodName;
        }

        public string Name { get; }
    }
}
