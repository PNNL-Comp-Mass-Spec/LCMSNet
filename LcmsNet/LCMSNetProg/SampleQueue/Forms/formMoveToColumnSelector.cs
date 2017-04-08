using System;
using System.Windows.Forms;

namespace LcmsNet.SampleQueue.Forms
{
    public partial class formMoveToColumnSelector : Form
    {
        /// <summary>
        /// Value if the user never selected a column to move samples to and thus cancelled the operation.
        /// </summary>
        public const int CONST_NO_COLUMN_SELECTED = -1;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public formMoveToColumnSelector()
        {
            InitializeComponent();

            SelectedColumn = CONST_NO_COLUMN_SELECTED;
            FormClosing += formMoveToColumnSelector_FormClosing;
        }

        /// <summary>
        /// Gets or sets the column the user selected to move samples to.  This value is zero-based.
        /// </summary>
        public int SelectedColumn { get; set; }

        /// <summary>
        /// Gets or sets whether to insert into unused.
        /// </summary>
        public bool InsertIntoUnused
        {
            get { return mcheckbox_fillIn.Checked; }
            set { mcheckbox_fillIn.Checked = value; }
        }

        #region Form Event Handlers

        void formMoveToColumnSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                SelectedColumn = CONST_NO_COLUMN_SELECTED;
                DialogResult = DialogResult.Cancel;
            }
        }

        private void mbutton_cancel_Click(object sender, EventArgs e)
        {
            SelectedColumn = CONST_NO_COLUMN_SELECTED;
            DialogResult = DialogResult.Cancel;
        }

        private void mbutton_column1_Click(object sender, EventArgs e)
        {
            SelectedColumn = 0;
            DialogResult = DialogResult.OK;
        }

        private void mbutton_column2_Click(object sender, EventArgs e)
        {
            SelectedColumn = 1;
            DialogResult = DialogResult.OK;
        }

        private void mbutton_column3_Click(object sender, EventArgs e)
        {
            SelectedColumn = 2;
            DialogResult = DialogResult.OK;
        }

        private void mbutton_column4_Click(object sender, EventArgs e)
        {
            SelectedColumn = 3;
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}