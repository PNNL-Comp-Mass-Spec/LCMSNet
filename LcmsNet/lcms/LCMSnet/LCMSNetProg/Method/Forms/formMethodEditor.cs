using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses;
using LcmsNet.Configuration;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Form that displays a control for editing methods.
    /// </summary>
    public partial class formMethodEditor : Form
    {
        /// <summary>
        /// Dialog for opening a LC-Method
        /// </summary>
        private OpenFileDialog mdialog_openMethod;

        private string mstring_editingMethod;

        /// <summary>
        /// Constructor.
        /// </summary>
        public formMethodEditor()
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
            mstring_editingMethod = "";

            // mcontrol_methodEditor.EventChanged += new EventHandler(mcontrol_methodEditor_EventChanged);
            mcontrol_methodEditor.UpdatingMethod +=
                new EventHandler<classMethodEditingEventArgs>(mcontrol_methodEditor_UpdatingMethod);
        }

        void mcontrol_methodEditor_UpdatingMethod(object sender, classMethodEditingEventArgs e)
        {
            mstring_editingMethod = e.Name;
            UpdateName();
        }

        private void UpdateName()
        {
        }

        ///// <summary>
        ///// Tells the control to save the method
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        mcontrol_methodEditor.SaveMethod();
        //        mbool_requiresSave = false;
        //        UpdateName();
        //    }
        //    catch(Exception ex)
        //    {
        //        classApplicationLogger.LogError(0, "Could not save the current method.  " + ex.Message, ex);
        //    }
        //}
        //void mcontrol_methodEditor_EventChanged(object sender, EventArgs e)
        //{
        //    mbool_requiresSave = true;
        //    UpdateName();
        //}
        ///// <summary>
        ///// Tells the control to open a method from file.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void openToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (mdialog_openMethod.ShowDialog() == DialogResult.OK)
        //        mcontrol_methodEditor.OpenMethod(mdialog_openMethod.FileName);
        //}
        ///// <summary>
        ///// Tells the control to load the methods from a directory.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void loadMethodsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    mcontrol_methodEditor.LoadMethods();
        //}
        /// <summary>
        /// Saves all the methods to XML.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    mcontrol_methodEditor.SaveMethods();
        //    mbool_requiresSave = false;
        //    UpdateName();
        //}
        /// <summary>
        /// Loads the methods from the LC Method folder path.
        /// </summary>
        public void LoadMethods()
        {
            //TODO: BLL Add a window that shows what errors occurred when loading a method
            mcontrol_methodEditor.LoadMethods();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var options = new formMethodPreviewOptions();
            options.Animate = mcontrol_methodEditor.MethodPreviewOptions.Animate;
            options.AnimationDelay = mcontrol_methodEditor.MethodPreviewOptions.AnimateDelay;
            options.FrameDelay = mcontrol_methodEditor.MethodPreviewOptions.FrameDelay;
            if (options.ShowDialog() == DialogResult.OK)
            {
                mcontrol_methodEditor.MethodPreviewOptions.Animate = options.Animate;
                mcontrol_methodEditor.MethodPreviewOptions.AnimateDelay = options.AnimationDelay;
                mcontrol_methodEditor.MethodPreviewOptions.FrameDelay = options.FrameDelay;
            }
            options.Dispose();
        }
    }
}