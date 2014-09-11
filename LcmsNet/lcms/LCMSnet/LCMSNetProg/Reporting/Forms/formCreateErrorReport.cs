using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Method;
using LcmsNet.Reporting;
using LcmsNet.Method;
using LcmsNetDataClasses;

namespace LcmsNet.Reporting.Forms
{
    public partial class formCreateErrorReport : Form
    {
        private classLCMethodManager m_manager;
        private string               m_logPath;
        private List<Form>           m_forms;

        public formCreateErrorReport()
        {
            InitializeComponent();
        }


        public formCreateErrorReport(classLCMethodManager manager, string logPath, List<Form> forms)
        {
            InitializeComponent();

            m_manager = manager;
            m_manager.MethodAdded   += new DelegateMethodUpdated(m_manager_MethodAdded);
            m_manager.MethodRemoved += new DelegateMethodUpdated(m_manager_MethodRemoved);

            m_logPath   = logPath;
            m_forms     = forms;

            foreach (classLCMethod method in m_manager.Methods.Values)
            {
                if (!mlistbox_methods.Items.Contains(method))
                {
                    mlistbox_methods.Items.Add(method);
                    mlistbox_methods.SelectedItems.Add(method);
                }
            }            
        }

        #region Method Manager Updates
        /// <summary>
        /// Removes the LC Method from the listbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        bool m_manager_MethodRemoved(object sender, LcmsNetDataClasses.Method.classLCMethod method)
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
        bool m_manager_MethodAdded(object sender, LcmsNetDataClasses.Method.classLCMethod method)
        {
            if (mlistbox_methods.Items.Contains(method))
                return true;

            mlistbox_methods.Items.Add(method);
            return true;
        }
        #endregion

        #region Button Handler Events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_create_Click(object sender, EventArgs e)
        {
            List<classLCMethod> methods     = new List<classLCMethod>();
            foreach(object o in mlistbox_methods.SelectedItems)
            {
                classLCMethod method = o as classLCMethod;
                if (method != null)
                {
                    methods.Add(method);
                }
            }


            classErrorReportBuilder builder = new classErrorReportBuilder();
            string path = builder.CreateReport(m_forms, methods, m_logPath, "hardwareconfig.ini");

            string cartName         = classLCMSSettings.GetParameter("CartName");
            string errorReportPath  = classLCMSSettings.GetParameter("ErrorPath");

            classErrorReportBuilder.CopyReportToServer(path, cartName, errorReportPath);
        }
        #endregion
    }
}
