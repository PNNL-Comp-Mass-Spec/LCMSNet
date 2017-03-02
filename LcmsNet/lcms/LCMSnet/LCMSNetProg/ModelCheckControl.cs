using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetSDK;

namespace LcmsNet
{
    public partial class ModelCheckControl : UserControl
    {
        IFluidicsModelChecker modelChecker;

        public ModelCheckControl(IFluidicsModelChecker check)
        {
            InitializeComponent();
            modelChecker = check;
            chkboxModel.Text = modelChecker.Name;
            chkboxModel.Checked = check.IsEnabled;
            chkboxModel.CheckedChanged += CheckBoxCheckChanged;
            comboCategories.DataSource = Enum.GetValues(typeof (ModelStatusCategory));
        }


        public bool Checked
        {
            get { return chkboxModel.Checked; }
        }

        public event EventHandler CheckChanged;

        public void Check(bool checkValue)
        {
            chkboxModel.Checked = checkValue;
        }

        private void chkboxModel_CheckedChanged(object sender, EventArgs e)
        {
            modelChecker.IsEnabled = ((CheckBox) sender).Checked;
        }

        private void comboCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox) sender;
            modelChecker.Category = (ModelStatusCategory) s.SelectedItem;
        }

        private void CheckBoxCheckChanged(object sender, EventArgs e)
        {
            if (CheckChanged != null)
            {
                CheckChanged(this, new EventArgs());
            }
        }
    }
}