using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.Dashboard
{
    /// <summary>
    ///
    /// </summary>
    public partial class formDeviceAddForm : Form
    {
        /// <summary>
        ///
        /// </summary>
        public formDeviceAddForm()
        {
            InitializeComponent();
            mtree_availableDevices.KeyUp += mtree_availableDevices_KeyUp;
            mtree_availableDevices.NodeMouseDoubleClick +=
                mtree_availableDevices_NodeMouseDoubleClick;
            mlistbox_devices.KeyUp += mlistbox_devices_KeyUp;
        }

        /// <summary>
        /// Gets or sets the initialize on add flag.
        /// </summary>
        public bool InitializeOnAdd
        {
            get { return mcheckBox_initializeOnAdd.Checked; }
            set { mcheckBox_initializeOnAdd.Checked = value; }
        }

        void mlistbox_devices_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keys.Delete == e.KeyData)
            {
                RemoveSelectedItems();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mtree_availableDevices_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            AddSelectedNode();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mtree_availableDevices_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddSelectedNode();
            }
        }

        /// <summary>
        /// Adds the supplied plugins to the check box list.
        /// </summary>
        /// <param name="plugins"></param>
        public void AddPluginInformation(List<classDevicePluginInformation> plugins)
        {
            mtree_availableDevices.BeginUpdate();
            mtree_availableDevices.Nodes.Clear();
            foreach (var info in plugins)
            {
                TreeNode rootNode = null;
                if (!mtree_availableDevices.Nodes.ContainsKey(info.DeviceAttribute.Category))
                {
                    var newParent = new TreeNode();
                    newParent.Text = info.DeviceAttribute.Category;
                    newParent.Name = info.DeviceAttribute.Category;
                    rootNode = newParent;
                    mtree_availableDevices.Nodes.Add(newParent);
                }
                else
                {
                    rootNode = mtree_availableDevices.Nodes[info.DeviceAttribute.Category];
                }

                var node = new TreeNode();
                node.Name = info.DeviceAttribute.Name;
                node.Text = info.DeviceAttribute.Name;
                node.Tag = info;

                rootNode.Nodes.Add(node);
            }
            mtree_availableDevices.ExpandAll();
            mtree_availableDevices.EndUpdate();
        }

        public List<classDevicePluginInformation> GetSelectedPlugins()
        {
            var plugins = new List<classDevicePluginInformation>();
            foreach (var o in mlistbox_devices.Items)
            {
                var plugin = o as classDevicePluginInformation;
                if (plugin != null)
                {
                    plugins.Add(plugin);
                }
            }
            return plugins;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_add_Click(object sender, EventArgs e)
        {
            AddSelectedNode();
        }

        /// <summary>
        /// Adds the selected node to the list box of devices to be loaded.
        /// </summary>
        private void AddSelectedNode()
        {
            if (mtree_availableDevices.SelectedNode != null)
            {
                if (mtree_availableDevices.SelectedNode.Tag == null)
                    return;

                mlistbox_devices.Items.Add(mtree_availableDevices.SelectedNode.Tag);
            }
        }

        private void RemoveSelectedItems()
        {
            if (mlistbox_devices.SelectedItems != null)
            {
                mlistbox_devices.BeginUpdate();

                var pluginsToRemove = new List<object>();
                foreach (var o in mlistbox_devices.SelectedItems)
                {
                    if (o != null)
                    {
                        pluginsToRemove.Add(o);
                    }
                }
                foreach (var o in pluginsToRemove)
                {
                    mlistbox_devices.Items.Remove(o);
                }
                mlistbox_devices.EndUpdate();
            }
        }

        private void mbutton_remove_Click(object sender, EventArgs e)
        {
            RemoveSelectedItems();
        }
    }
}