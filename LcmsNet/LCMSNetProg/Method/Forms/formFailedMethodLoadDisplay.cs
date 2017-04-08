using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    /// <summary>
    /// Displays the device initialization errors.
    /// </summary>
    public partial class formFailedMethodLoadDisplay : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="deviceErrors"></param>
        public formFailedMethodLoadDisplay(Dictionary<string, List<Exception>> errors)
        {
            InitializeComponent();
            UpdateList(errors);
        }

        /// <summary>
        /// Updates the listview with the error device messages.
        /// </summary>
        /// <param name="deviceErrors"></param>
        public void UpdateList(Dictionary<string, List<Exception>> errors)
        {
            m_tree.BeginUpdate();
            m_tree.Nodes.Clear();
            foreach (var  path in errors.Keys)
            {
                var parentNode = new TreeNode();
                parentNode.Text = Path.GetFileNameWithoutExtension(path);
                var exceptions = errors[path];
                foreach (var ex in exceptions)
                {
                    var childNode = new TreeNode();
                    childNode.Text = ex.Message;
                    parentNode.Nodes.Add(childNode);
                }
                m_tree.Nodes.Add(parentNode);
            }
            m_tree.ExpandAll();
            m_tree.EndUpdate();
        }

        private void mbutton_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}