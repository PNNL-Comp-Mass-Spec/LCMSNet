using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSDK;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Control that allows the user to edit/create/delete their own methods.
    /// </summary>
    public partial class controlLCMethodEditor : UserControl
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
        /// Dialog for opening a LC-Method
        /// </summary>
        private readonly OpenFileDialog mdialog_openMethod;

        /// <summary>
        /// Render Update counts
        /// </summary>
        private int m_renderUpdateCount;

        /// <summary>
        /// Currently selected method.
        /// </summary>
        private classLCMethod m_currentMethod;

        /// <summary>
        /// Constructor that allows for users to edit methods.
        /// </summary>
        public controlLCMethodEditor()
        {
            InitializeComponent();

            mdialog_openMethod = new OpenFileDialog();
            mdialog_openMethod.Title = "Open LC-Method";
            var path = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH);
            if (!string.IsNullOrWhiteSpace(path))
            {
                mdialog_openMethod.InitialDirectory = Path.Combine(path,
                    classLCMethodFactory.CONST_LC_METHOD_FOLDER);
            }
            m_currentMethod = new classLCMethod();
            MethodFolderPath = CONST_METHOD_FOLDER_PATH;

            UpdateConfiguration();

            mcontrol_selectedMethods.MethodAdded +=
                mcontrol_selectedMethods_MethodAdded;
            mcontrol_selectedMethods.MethodDeleted +=
                mcontrol_selectedMethods_MethodDeleted;
            mcontrol_selectedMethods.MethodUpdated +=
                mcontrol_selectedMethods_MethodUpdated;


            mcomboBox_previewMode.Items.Add(enumLCMethodRenderMode.Column);
            mcomboBox_previewMode.Items.Add(enumLCMethodRenderMode.Time);
            mcomboBox_previewMode.SelectedIndex = 0;
            mcomboBox_previewMode.SelectedIndexChanged += mcomboBox_previewMode_SelectedIndexChanged;

            MethodPreviewOptions = new classMethodPreviewOptions();
            MethodPreviewOptions.FrameDelay = CONST_DEFAULT_RENDER_WAIT_COUNT;
            MethodPreviewOptions.Animate = false;
            MethodPreviewOptions.AnimateDelay = CONST_DEFAULT_ANIMATION_DELAY_TIME;
            m_renderUpdateCount = 0;

            mcontrol_acquisitionStage.EventChanged += mcontrol_acquisitionStage_EventChanged;
        }

        /// <summary>
        /// Gets or sets whether to animate the method optimization not.
        /// </summary>
        public bool Animate => mcheckBox_animate.Checked;

        /// <summary>
        /// Gets or sets the animation delay time in ms.
        /// </summary>
        public int AnimationDelay => Convert.ToInt32(mnum_delay.Value);

        /// <summary>
        /// Gets or sets the frame delay count.
        /// </summary>
        public int FrameDelay => Convert.ToInt32(mnum_frameCount.Value);

        /// <summary>
        /// Fired when editing a method.
        /// </summary>
        public event EventHandler<classMethodEditingEventArgs> UpdatingMethod;

        public event EventHandler EventChanged;

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            EventChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Fires an event changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mcontrol_acquisitionStage_EventChanged(object sender, EventArgs e)
        {
            OnEventChanged();
        }

        #region Column Defintion Check Event Handlers And Configuration

        /// <summary>
        /// Updates the configuration data and the user interface.
        /// </summary>
        /// <param name="configuration"></param>
        private void UpdateConfiguration()
        {
            mcontrol_acquisitionStage.UpdateConfiguration();
        }

        #endregion

        private void mcheckBox_animate_CheckedChanged(object sender, EventArgs e)
        {
            MethodPreviewOptions.Animate = Animate;
        }

        private void mnum_delay_ValueChanged(object sender, EventArgs e)
        {
            MethodPreviewOptions.AnimateDelay = AnimationDelay;
        }

        private void mnum_frameCount_ValueChanged(object sender, EventArgs e)
        {
            MethodPreviewOptions.FrameDelay = FrameDelay;
        }

        private void mbutton_open_Click(object sender, EventArgs e)
        {
            if (mdialog_openMethod.ShowDialog() == DialogResult.OK)
                OpenMethod(mdialog_openMethod.FileName);
        }

        private void mbutton_load_Click(object sender, EventArgs e)
        {
            LoadMethods();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the method animation preview options.
        /// </summary>
        public classMethodPreviewOptions MethodPreviewOptions { get; set; }

        /// <summary>
        /// Gets or sets what folder path the methods are stored in.
        /// </summary>
        public string MethodFolderPath { get; set; }

        #endregion

        #region Method Selection Events

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void mcontrol_selectedMethods_MethodDeleted(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void mcontrol_selectedMethods_MethodAdded(object sender)
        {
            RenderAlignMethods();
        }

        /// <summary>
        /// Updates the alignment preview.
        /// </summary>
        /// <param name="sender"></param>
        void mcontrol_selectedMethods_MethodUpdated(object sender)
        {
            RenderAlignMethods();
        }

        #endregion

        #region Building, Registering, Rendering Methods

        /// <summary>
        /// Builds the LC Method.
        /// </summary>
        /// <returns>A LC-Method if events are defined.  Null if events are not.</returns>
        public classLCMethod BuildMethod()
        {
            var method = classLCMethodBuilder.BuildMethod(mcontrol_acquisitionStage.LCEvents);
            if (method == null)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Cannot create the LC-method from an empty event list.  You need to add events to the method.");
                return null;
            }

            var column = mcontrol_acquisitionStage.GetColumn();
            if (column < 0)
            {
                method.IsSpecialMethod = true;
            }
            else
            {
                method.IsSpecialMethod = false;
            }
            method.Column = column;
            method.AllowPreOverlap = mcontrol_acquisitionStage.AllowPreOverlap;
            method.AllowPostOverlap = mcontrol_acquisitionStage.AllowPostOverlap;
            return method;
        }

        /// <summary>
        /// Finds the associated method from the list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private classLCMethod FindMethods(string name)
        {
            classLCMethod method = null;
            if (classLCMethodManager.Manager.Methods.ContainsKey(name))
            {
                method = classLCMethodManager.Manager.Methods[name];
            }
            return method;
        }

        /// <summary>
        /// Renders the throughput timelines.
        /// </summary>
        /// <param name="methods">Methods to render for showing throughput.</param>
        private void RenderThroughput(List<classLCMethod> methods)
        {
            if (methods == null)
                return;

            mcontrol_methodTimelineThroughput.RenderLCMethod(methods);
            mcontrol_methodTimelineThroughput.Invalidate();
        }

        /// <summary>
        /// Aligns then renders the methods selected from the user interface.
        /// </summary>
        private void RenderAlignMethods()
        {
            //
            // Align methods - blindly!
            //
            try
            {
                var optimizer = new classLCMethodOptimizer();
                optimizer.UpdateRequired +=
                    optimizer_UpdateRequired;
                var methods = mcontrol_selectedMethods.SelectedMethods;
                m_renderUpdateCount = 0;

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
        void optimizer_UpdateRequired(classLCMethodOptimizer sender)
        {
            //
            // Render the updates if we animating and past the rendering threshold.
            //
            if (MethodPreviewOptions.Animate && m_renderUpdateCount++ >= MethodPreviewOptions.FrameDelay)
            {
                RenderThroughput(sender.Methods);
                Refresh();
                Application.DoEvents();

                //
                // reset the number of update calls we have seen.
                //
                m_renderUpdateCount = 0;

                Thread.Sleep(MethodPreviewOptions.AnimateDelay);
            }
        }

        /// <summary>
        /// Builds the method based on the user interface input.
        /// </summary>
        private void Build()
        {
            //
            // Construct the method
            //
            classLCMethod method = null;
            method = BuildMethod();
            if (method == null)
                return;

            method.Name = mcontrol_acquisitionStage.TextBoxNameGetText();

            //
            // Renders the method built
            //
            m_currentMethod = method;

            //
            // Register the method
            //
            classLCMethodManager.Manager.AddMethod(method);
            OnEventChanged();
        }

        /// <summary>
        /// Loads the method into the editor.
        /// </summary>
        /// <param name="method"></param>
        private void LoadMethod(classLCMethod method)
        {
            if (method != null && m_currentMethod != method)
            {
                m_currentMethod = method;
                mcontrol_acquisitionStage.LoadMethod(method, true);

                mcontrol_acquisitionStage.TextBoxNameSetText(method.Name);
            }
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Sets the render mode of the timeline viewer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mcomboBox_previewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                mcontrol_methodTimelineThroughput.RenderMode =
                    (enumLCMethodRenderMode) mcomboBox_previewMode.SelectedItem;
                mcontrol_methodTimelineThroughput.Refresh();
            }
            catch
            {
                //TODO: What do we do here when the preview mode cannot be set?
            }
        }

        /// <summary>
        /// Handles previewing the throughput of a single run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_preview_Click(object sender, EventArgs e)
        {
            RenderAlignMethods();
        }

        private void OnEditMethod()
        {
            UpdatingMethod?.Invoke(this, new classMethodEditingEventArgs(mcontrol_acquisitionStage.TextBoxNameGetText()));
        }

        private bool IgnoreUpdates { get; set; }

        #endregion

        #region Saving and Loading

        /// <summary>
        /// Saves the given method to file.
        /// </summary>
        /// <param name="method"></param>
        public bool SaveMethod(classLCMethod method)
        {
            //
            // Method is null!!! OH MAN - that isn't my fault so we'll ignore it!
            //
            if (method == null)
                return false;

            //
            // Create a new writer.
            //
            var writer = new classLCMethodWriter();

            //
            // Construct the path
            //
            var path = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH),
                classLCMethodFactory.CONST_LC_METHOD_FOLDER);
            path = Path.Combine(path, method.Name + classLCMethodFactory.CONST_LC_METHOD_EXTENSION);

            //
            // Write the method out!
            //
            return writer.WriteMethod(path, method);
        }

        /// <summary>
        /// Opens a method.
        /// </summary>
        public void OpenMethod(string path)
        {
            var reader = new classLCMethodReader();
            var errors = new List<Exception>();
            var method = reader.ReadMethod(path, ref errors);

            if (method != null)
            {
                classLCMethodManager.Manager.AddMethod(method);
            }
        }

        /// <summary>
        /// Loads method stored in the method folder directory.
        /// </summary>
        public void LoadMethods()
        {
            var methods = Directory.GetFiles(MethodFolderPath, "*.xml");
            foreach (var method in methods)
            {
                classApplicationLogger.LogMessage(0, "Loading method " + method);
                OpenMethod(method);
            }

            classApplicationLogger.LogMessage(0, "Methods loaded.");
        }

        #endregion
    }

    /// <summary>
    /// Arguments for
    /// </summary>
    public class EditingLCMethod : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="methodName">Method currently editing.</param>
        /// <param name="modified">Flag indicating if a sample has been saved.</param>
        public EditingLCMethod(string methodName, bool modified)
        {
            MethodName = methodName;
            IsDirty = modified;
        }

        /// <summary>
        /// Gets the flag indicating if the sample method needs to be saved.
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Gets the method name being edited.
        /// </summary>
        public string MethodName { get; private set; }
    }

    public class classMethodEditingEventArgs : EventArgs
    {
        public classMethodEditingEventArgs(string methodName)
        {
            Name = methodName;
        }

        public string Name { get; private set; }
    }
}