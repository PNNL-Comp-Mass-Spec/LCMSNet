using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNet.Method;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Reporting.Forms
{
    public partial class formCreateErrorReport : Form
    {
        private readonly List<Form> m_forms;
        private readonly string m_logPath;
        private readonly classLCMethodManager m_manager;

        public formCreateErrorReport()
        {
            InitializeComponent();
        }


        public formCreateErrorReport(classLCMethodManager manager, string logPath, List<Form> forms)
        {
            InitializeComponent();

            m_manager = manager;
            m_manager.MethodAdded += m_manager_MethodAdded;
            m_manager.MethodRemoved += m_manager_MethodRemoved;

            m_logPath = logPath;
            m_forms = forms;

            foreach (var method in m_manager.Methods.Values)
            {
                if (!mlistbox_methods.Items.Contains(method))
                {
                    mlistbox_methods.Items.Add(method);
                    mlistbox_methods.SelectedItems.Add(method);
                }
            }
        }

        #region Button Handler Events

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_create_Click(object sender, EventArgs e)
        {
            var methods = new List<classLCMethod>();
            foreach (var o in mlistbox_methods.SelectedItems)
            {
                var method = o as classLCMethod;
                if (method != null)
                {
                    methods.Add(method);
                }
            }


            var builder = new classErrorReportBuilder();
            var path = builder.CreateReport(m_forms, methods, m_logPath, "hardwareconfig.ini");

            var cartName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);
            var errorReportPath = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_ERRORPATH);

            classErrorReportBuilder.CopyReportToServer(path, cartName, errorReportPath);
        }

        #endregion

        #region Method Manager Updates

        /// <summary>
        /// Removes the LC Method from the listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        bool m_manager_MethodRemoved(object sender, classLCMethod method)
        {
            if (!mlistbox_methods.Items.Contains(method))
                return true;

            mlistbox_methods.Items.Remove(method);
            return true;
        }

        /// <summary>
        /// Adds a new LC Method to the list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        bool m_manager_MethodAdded(object sender, classLCMethod method)
        {
            if (mlistbox_methods.Items.Contains(method))
                return true;

            mlistbox_methods.Items.Add(method);
            return true;
        }

        #endregion
    }
}