using System;
using System.Windows.Forms;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices
{
    public partial class formDeviceManager : Form
    {
        /// <summary>
        /// Mapping between devices to their position in the listview.
        /// </summary>
        private Dictionary<IDevice, ListViewItem> mdict_deviceToItemMap;


        /// <summary>
        /// Constructor.
        /// </summary>
        public formDeviceManager()
        {
            InitializeComponent();
            TopMost = true;

            mdict_deviceToItemMap = new Dictionary<IDevice, ListViewItem>();

            string[] columnNames = new string[] {"Device", "Status", "Type"};
            foreach (string name in columnNames)
            {
                ColumnHeader header = new ColumnHeader();
                header.Text = name;
                mlistview_devices.Columns.Add(header);
            }

            if (classDeviceManager.Manager != null)
            {
                mlistview_devices.BeginUpdate();
                //
                // Add all devices to the list of available devices
                //
                foreach (IDevice device in classDeviceManager.Manager.Devices)
                {
                    AddDevice(device);
                }
                mlistview_devices.EndUpdate();

                classDeviceManager.Manager.DeviceRemoved += new DelegateDeviceUpdated(Manager_DeviceRemoved);
                classDeviceManager.Manager.DeviceAdded += new DelegateDeviceUpdated(Manager_DeviceAdded);
                classDeviceManager.Manager.DeviceRenamed += new DelegateDeviceUpdated(Manager_DeviceRenamed);
            }
            mlistview_devices.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_devices.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_devices.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// Add a device to the list of devices.
        /// </summary>
        /// <param name="device">Device to show.</param>
        private void AddDevice(IDevice device)
        {
            device.StatusUpdate += new EventHandler<classDeviceStatusEventArgs>(device_StatusUpdate);
            ListViewItem item = new ListViewItem();
            item.Text = device.Name;
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, device.Status.ToString()));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, device.DeviceType.ToString()));
            mlistview_devices.Items.Add(item);
            mdict_deviceToItemMap.Add(device, item);
            mlistview_devices.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_devices.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// Remove the device from the listview and mapping structures.
        /// </summary>
        /// <param name="device">Device to show.</param>
        private void RemoveDevice(IDevice device)
        {
            if (mdict_deviceToItemMap.ContainsKey(device))
            {
                ListViewItem item = mdict_deviceToItemMap[device];
                device.StatusUpdate -= device_StatusUpdate;
                mdict_deviceToItemMap.Remove(device);
                mlistview_devices.Items.Remove(item);
                mlistview_devices.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                mlistview_devices.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        /// <summary>
        /// Handle updating the device status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status"></param>
        void device_StatusUpdate(object sender, classDeviceStatusEventArgs args)
        {
        }

        #region Device Manager Events

        void Manager_DeviceAdded(object sender, IDevice device)
        {
            AddDevice(device);
        }

        void Manager_DeviceRemoved(object sender, IDevice device)
        {
            RemoveDevice(device);
        }

        /// <summary>
        /// Handles when a device is renamed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRenamed(object sender, IDevice device)
        {
            if (mdict_deviceToItemMap.ContainsKey(device))
            {
                ListViewItem item = mdict_deviceToItemMap[device];
                item.Text = device.Name;
            }
        }

        #endregion
    }
}