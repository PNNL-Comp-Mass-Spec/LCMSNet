using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Displays the device initialization errors.
    /// </summary>
    public partial class formFailedDevicesDisplay : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="deviceErrors"></param>
        public formFailedDevicesDisplay(List<classDeviceErrorEventArgs> deviceErrors)
        {
            InitializeComponent();


            UpdateDeviceList(deviceErrors);
        }

        /// <summary>
        /// Updates the listview with the error device messages.
        /// </summary>
        /// <param name="deviceErrors"></param>
        public void UpdateDeviceList(List<classDeviceErrorEventArgs> deviceErrors)
        {
            mlistview_failedDevices.BeginUpdate();
            mlistview_failedDevices.Items.Clear();
            foreach (classDeviceErrorEventArgs error in deviceErrors)
            {
                ListViewItem item = new ListViewItem();
                item.Text = error.Device.Name;
                string exceptionMessage = "";
                if (error.Exception != null)
                {
                    exceptionMessage = error.Exception.Message;
                }

                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, error.Error + " " + exceptionMessage));
                mlistview_failedDevices.Items.Add(item);
            }
            mlistview_failedDevices.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_failedDevices.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_failedDevices.EndUpdate();
        }

        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}