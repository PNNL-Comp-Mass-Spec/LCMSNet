using System;
using System.Windows.Forms;
using LcmsNetDataClasses;
using LcmsNetSDK;

namespace LcmsNet
{
    public partial class ModelCheckControl : UserControl
    {
        readonly IFluidicsModelChecker modelChecker;

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
            var s = (ComboBox) sender;
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