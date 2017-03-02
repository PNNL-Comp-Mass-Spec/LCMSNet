using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetSDK;

namespace LcmsNet
{
    public partial class ModelCheckListControl : UserControl
    {
        IModelCheckController controller;
        Dictionary<string, ModelCheckControl> m_checks;

        public ModelCheckListControl()
        {
            InitializeComponent();
        }

        public ModelCheckListControl(IModelCheckController cntrlr, IEnumerable<IFluidicsModelChecker> checks)
        {
            InitializeComponent();
            controller = cntrlr;
            controller.ModelCheckAdded += CheckAddedHandler;
            controller.ModelCheckRemoved += CheckRemovedHandler;
            m_checks = new Dictionary<string, ModelCheckControl>();
            foreach (var check in checks)
            {
                AddCheckerControl(check);
            }
        }

        private void EnableAllCheckHandler(object sender, EventArgs e)
        {
            var checkValue = ((CheckBox) sender).Checked;
            if (checkValue)
            {
                foreach (var key in m_checks.Keys)
                {
                    m_checks[key].Check(checkValue);
                }
                this.Refresh();
            }
        }

        private void AddCheckerControl(IFluidicsModelChecker check)
        {
            if (!(m_checks.Keys.Contains(check.Name)))
            {
                var checkerControl = new ModelCheckControl(check);
                checkerControl.CheckChanged += ModelCheckChanged;
                m_checks[check.Name] = checkerControl;
                checkerControl.Dock = DockStyle.Top;
                var checkboxPanel = groupBoxModelChecks.Controls["panelCheckBoxes"] as Panel;
                checkboxPanel.Controls.Add(checkerControl);
                checkerControl.BringToFront(); // keeps Z-order correct.
            }
        }

        ~ModelCheckListControl()
        {
            controller.ModelCheckAdded -= CheckAddedHandler;
            controller.ModelCheckRemoved -= CheckRemovedHandler;
        }

        private void ModelCheckChanged(object sender, EventArgs e)
        {
            var s = (ModelCheckControl) sender;
            if (!s.Checked)
            {
                ((CheckBox) (groupBoxModelChecks.Controls["panelEnableAll"].Controls["enableAllModelChecks"])).Checked =
                    false;
            }
        }

        private void CheckAddedHandler(object sender, ModelCheckControllerEventArgs e)
        {
            var check = e.ModelChecker;
            AddCheckerControl(check);
            this.Refresh();
        }

        private void CheckRemovedHandler(object sender, ModelCheckControllerEventArgs e)
        {
            var check = e.ModelChecker;
            if (m_checks[check.Name] == check)
            {
                groupBoxModelChecks.Controls.Remove(m_checks[check.Name]);
                m_checks.Remove(check.Name);
            }
            this.Refresh();
        }
    }
}