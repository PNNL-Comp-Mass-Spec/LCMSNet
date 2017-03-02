using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices
{
    public partial class formDeviceManager : Form
    {
        /// <summary>
        /// Mapping between devices to their position in the listview.
        /// </summary>
        private readonly Dictionary<IDevice, ListViewItem> m_deviceToItemMap;


        /// <summary>
        /// Constructor.
        /// </summary>
        public formDeviceManager()
        {
            InitializeComponent();
            TopMost = true;

            m_deviceToItemMap = new Dictionary<IDevice, ListViewItem>();

            var columnNames = new[] {"Device", "Status", "Type"};
            foreach (var name in columnNames)
            {
                var header = new ColumnHeader();
                header.Text = name;
                mlistview_devices.Columns.Add(header);
            }

            if (classDeviceManager.Manager != null)
            {
                mlistview_devices.BeginUpdate();
                //
                // Add all devices to the list of available devices
                //
                foreach (var device in classDeviceManager.Manager.Devices)
                {
                    AddDevice(device);
                }
                mlistview_devices.EndUpdate();

                classDeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
                classDeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
                classDeviceManager.Manager.DeviceRenamed += Manager_DeviceRenamed;
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
            device.StatusUpdate += device_StatusUpdate;
            var item = new ListViewItem();
            item.Text = device.Name;
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, device.Status.ToString()));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, device.DeviceType.ToString()));
            mlistview_devices.Items.Add(item);
            m_deviceToItemMap.Add(device, item);
            mlistview_devices.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            mlistview_devices.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// Remove the device from the listview and mapping structures.
        /// </summary>
        /// <param name="device">Device to show.</param>
        private void RemoveDevice(IDevice device)
        {
            if (m_deviceToItemMap.ContainsKey(device))
            {
                var item = m_deviceToItemMap[device];
                device.StatusUpdate -= device_StatusUpdate;
                m_deviceToItemMap.Remove(device);
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
            if (m_deviceToItemMap.ContainsKey(device))
            {
                var item = m_deviceToItemMap[device];
                item.Text = device.Name;
            }
        }

        #endregion
    }
}