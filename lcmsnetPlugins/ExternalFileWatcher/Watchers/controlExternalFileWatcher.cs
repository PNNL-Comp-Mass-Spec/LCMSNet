using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices;

namespace ExternalFileWatcher.Watchers
{
    public partial class controlExternalFileWatcher : controlBaseDeviceControl,  IDeviceControl
    {
        FolderBrowserDialog m_browser;

        /// <summary>
        /// Notification driver object.
        /// </summary>
        private ExternalFileWatcher mobj_driver;
        public event DelegateNameChanged NameChanged;
        public event DelegateSaveRequired SaveRequired;

        public controlExternalFileWatcher()
        {
            InitializeComponent();

            m_browser = new FolderBrowserDialog();
        }

        public void RegisterDevice(IDevice device)
        {
            mobj_driver                     = device as ExternalFileWatcher;
            mnum_minutesToWait.Value        = Convert.ToDecimal(mobj_driver.SecondsToWait / 60.0);
            mtextBox_extension.Text         = mobj_driver.FileExtension;
            mtext_directoryExtension.Text   = mobj_driver.DirectoryExtension;
            mtextBox_path.Text              = mobj_driver.WatchPath;

            SetBaseDevice(mobj_driver);
        }

        public bool Running
        {
            get;
            set;
        }

        public IDevice Device
        {
            get
            {
                return mobj_driver;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        private void mutton_browse_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(mtextBox_path.Text))
            {
                m_browser.SelectedPath = mtextBox_path.Text;
                if (m_browser.ShowDialog() == DialogResult.OK)
                {
                    mtextBox_path.Text = m_browser.SelectedPath;
                }
            }
        }

        private void mbutton_setPath_Click(object sender, EventArgs e)
        {
            this.mobj_driver.WatchPath = mtextBox_path.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mobj_driver.SecondsToWait = Convert.ToDouble(mnum_minutesToWait.Value) * 60;
        }

        private void mbutton_setExtension_Click(object sender, EventArgs e)
        {
            mobj_driver.FileExtension = mtextBox_extension.Text;
        }

        private void mbutton_setDirectoryExtension_Click(object sender, EventArgs e)
        {
            mobj_driver.DirectoryExtension = mtext_directoryExtension.Text;
        }
    }
}
